using k8s;
using k8s.Models;
using K8sServer.Hubs;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Watchers.Implementations
{
    public class ServicesWatcher : IResourceWatcher
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private CancellationTokenSource _cancellationTokenSource;
        public ServicesWatcher(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
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
                var servicelistResp = _kubernetesClientService.GetClient().CoreV1.ListNamespacedServiceWithHttpMessagesAsync(namespaceName, watch: true, cancellationToken: cancellationToken);
                await foreach (var (type, item) in servicelistResp.WatchAsync<V1Service, V1ServiceList>())
                {
                    System.Console.WriteLine($"Received {type} update for service: {item.Metadata.Name} in namespace: {namespaceName}");
                    System.Console.WriteLine($"Services");
                    await _hubContext.Clients.Group(namespaceName).SendAsync("ReceiveServicesUpdate", new { Type = type.ToString(), resourceTypeFromServer = "Services".ToString(), item });
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
