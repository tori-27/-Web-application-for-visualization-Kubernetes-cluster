using k8s;
using k8s.Models;
using K8sServer.Hubs;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Watchers.Implementations
{
    public class ConfigMapsWatcher : IResourceWatcher
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private CancellationTokenSource _cancellationTokenSource;

        public ConfigMapsWatcher(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
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
                var configMaplistResp = _kubernetesClientService.GetClient().CoreV1.ListNamespacedConfigMapWithHttpMessagesAsync(namespaceName, watch: true, cancellationToken: linkedTokenSource.Token);
                await foreach (var (type, item) in configMaplistResp.WatchAsync<V1ConfigMap, V1ConfigMapList>())
                {
                 
                    await _hubContext.Clients.Group(namespaceName).SendAsync("ReceiveConfigMapsUpdate", new { Type = type.ToString(), resourceTypeFromServer = "ConfigMaps".ToString() , item});
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
