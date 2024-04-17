using k8s;
using k8s.Models;
using K8sServer.Hubs;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Watchers.Implementations
{
    public class ResourceQuotasWatcher : IResourceWatcher
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private CancellationTokenSource _cancellationTokenSource;
        public ResourceQuotasWatcher(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
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
                var resourceQuotasResp = _kubernetesClientService.GetClient().CoreV1.ListNamespacedResourceQuotaWithHttpMessagesAsync(namespaceName, watch: true, cancellationToken: cancellationToken);
                await foreach (var (type, item) in resourceQuotasResp.WatchAsync<V1ResourceQuota, V1ResourceQuotaList>())
                {
                    await _hubContext.Clients.Group(namespaceName).SendAsync("ReceiveresourceQuotasRespUpdate" ,new { Type = type.ToString(), resourceTypeFromServer = "ResourceQuotas".ToString(), item });
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
