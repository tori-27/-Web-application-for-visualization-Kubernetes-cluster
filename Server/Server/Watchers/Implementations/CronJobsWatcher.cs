using k8s;
using k8s.Models;
using K8sServer.Hubs;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace K8sServer.Watchers.Implementations
{
    public class CronJobsWatcher : IResourceWatcher
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private CancellationTokenSource _cancellationTokenSource;

        public CronJobsWatcher(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
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
                var cronJoblistResp = _kubernetesClientService.GetClient().BatchV1.ListNamespacedCronJobWithHttpMessagesAsync(namespaceName, watch: true, cancellationToken: cancellationToken);
                await foreach (var (type, item) in cronJoblistResp.WatchAsync<V1CronJob, V1CronJobList>())
                {

                    await _hubContext.Clients.Group(namespaceName).SendAsync("ReceiveCronJobsUpdate", new { Type = type.ToString(), resourceTypeFromServer = "CronJobs".ToString(), item});
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
