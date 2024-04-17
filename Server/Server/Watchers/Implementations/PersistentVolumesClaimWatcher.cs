using k8s;
using k8s.Models;
using K8sServer.Hubs;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Watchers.Implementations
{
    public class PersistentVolumesClaimWatcher : IResourceWatcher
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private CancellationTokenSource _cancellationTokenSource;
        public PersistentVolumesClaimWatcher(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
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
                var persistentVolumeClaimlistResp = _kubernetesClientService.GetClient().CoreV1.ListNamespacedPersistentVolumeClaimWithHttpMessagesAsync(namespaceName, watch: true, cancellationToken: cancellationToken);
                await foreach (var (type, item) in persistentVolumeClaimlistResp.WatchAsync<V1PersistentVolumeClaim, V1PersistentVolumeClaimList>())
                {
                    await _hubContext.Clients.Group(namespaceName).SendAsync("ReceivePersistentVolumesClaimUpdate" ,new { Type = type.ToString(), resourceTypeFromServer = "PersistentVolumesClaim".ToString(), item });
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
