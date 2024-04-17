using k8s;
using k8s.Models;
using K8sServer.Hubs;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Watchers.Implementations
{
    public class NetworkPoliciesWatcher : IResourceWatcher
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private CancellationTokenSource _cancellationTokenSource;
        public NetworkPoliciesWatcher(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
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
                var networkPolicylistResp = _kubernetesClientService.GetClient().NetworkingV1.ListNamespacedNetworkPolicyWithHttpMessagesAsync(namespaceName, watch: true, cancellationToken: cancellationToken);
                await foreach (var (type, item) in  networkPolicylistResp.WatchAsync<V1NetworkPolicy, V1NetworkPolicyList>())
                {
                    await _hubContext.Clients.Group(namespaceName).SendAsync("ReceiveNetworkPoliciesUpdate" ,new { Type = type.ToString(), resourceTypeFromServer = "NetworkPolicies".ToString(), item });
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
