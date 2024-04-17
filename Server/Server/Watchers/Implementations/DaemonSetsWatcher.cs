using k8s;
using k8s.Models;
using K8sServer.Hubs;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Watchers.Implementations
{
    public class DaemonSetsWatcher : IResourceWatcher
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private CancellationTokenSource _cancellationTokenSource;
        public DaemonSetsWatcher(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
        {
            _kubernetesClientService = kubernetesClientService;
            _hubContext = hubContext;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartWatchingAsync(CancellationToken cancellationToken, string namespaceName)
        {
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
            try
            {
                var daemonSetlistResp = _kubernetesClientService.GetClient().AppsV1.ListNamespacedDaemonSetWithHttpMessagesAsync(namespaceName, watch: true, cancellationToken: cancellationToken);
                await foreach (var (type, item) in daemonSetlistResp.WatchAsync<V1DaemonSet, V1DaemonSetList>())
                {
         
                    await _hubContext.Clients.Group(namespaceName).SendAsync("ReceiveDaemonSetsUpdate", new { Type = type.ToString(), resourceTypeFromServer = "DaemonSets".ToString(), item });
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Watching for daemonsets in namespace '{namespaceName}' was canceled.");
            }
        }

        public void StopWatching()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
