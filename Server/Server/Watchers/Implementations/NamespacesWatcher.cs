
using k8s;
using k8s.Models;
using K8sServer.Hubs;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Watchers.Implementations
{
    public class NamespacesWatcher : IResourceWatcher
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private CancellationTokenSource _cancellationTokenSource;
        public NamespacesWatcher(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
        {
            _kubernetesClientService = kubernetesClientService;
            _hubContext = hubContext;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartWatchingAsync(CancellationToken cancellationToken, string? namespaceName = null)
        {
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
            try
            {
                var namespacesListResp = _kubernetesClientService.GetClient().CoreV1.ListNamespaceWithHttpMessagesAsync(watch: true, cancellationToken: linkedTokenSource.Token);
                await foreach (var (type, item) in namespacesListResp.WatchAsync<V1Namespace, V1NamespaceList>())
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNamespacesUpdate", new { Type = type.ToString(), Namespace = item });
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Watching for namespaces was canceled.");
            }
        }

        public void StopWatching()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
