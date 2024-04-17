using k8s;
using k8s.Models;
using K8sServer.Hubs;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace K8sServer.Watchers.Implementations
{
    public class PodsWatcher : IResourceWatcher
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private CancellationTokenSource _cancellationTokenSource;

        public PodsWatcher(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
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
                var podlistResp = _kubernetesClientService.GetClient().CoreV1.ListNamespacedPodWithHttpMessagesAsync(namespaceName, watch: true, cancellationToken: cancellationToken);
                await foreach (var (type, item) in podlistResp.WatchAsync<V1Pod, V1PodList>())
                {
                    await _hubContext.Clients.Group(namespaceName).SendAsync("ReceivePodsUpdate", new { Type = type.ToString(), resourceTypeFromServer = "Pods".ToString(), item });
                }
            }
            catch (OperationCanceledException)
            {
                System.Console.WriteLine($"Watching for namespace '{namespaceName}' was canceled.");
            }
            
        }

        public void StopWatching()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
