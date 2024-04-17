using K8sServer.Hubs;
using K8sServer.Watchers;
using K8sServer.Watchers.Implementations;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Services
{
    public class WatcherService
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private readonly Dictionary<string, List<IResourceWatcher>> _watchers = new Dictionary<string, List<IResourceWatcher>>();

        public WatcherService(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
        {
            _kubernetesClientService = kubernetesClientService;
            _hubContext = hubContext;
        }

        public async Task ActiveWathcersForNamespace(string namespaceName)
        {
            var cts = new CancellationTokenSource();
            var watchers = new List<IResourceWatcher>
            {
                WatcherFactory.CreateWatcher(ResourceType.Pod, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.Deployment, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.Job, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.CronJob, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.ConfigMap, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.DaemonSet, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.Ingress, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.LimitRange, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.NetworkPolicy, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.ReplicaSet, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.ResourceQuota, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.Secret, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.Service, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.StatefulSet, _kubernetesClientService, _hubContext),
                WatcherFactory.CreateWatcher(ResourceType.PersistentVolumeClaim, _kubernetesClientService, _hubContext),
            };
            foreach (var watcher in watchers)
            {
                await watcher.StartWatchingAsync( cts.Token, namespaceName);
            }

            _watchers[namespaceName] = watchers;
        }

        public async Task ActiveNamespacesWatcher()
        {
            var namespacesWatcher = new NamespacesWatcher(_kubernetesClientService, _hubContext);
            await namespacesWatcher.StartWatchingAsync(CancellationToken.None);
        }

        public void StopWatchers(string namespaceName)
        {
            if(_watchers.TryGetValue(namespaceName, out var watchers))
            {
                foreach (var watcher in watchers)
                {
                    watcher.StopWatching();
                }
                _watchers.Remove(namespaceName);
            }
        }
    }
}



