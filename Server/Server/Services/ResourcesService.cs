using k8s;
using K8sServer.Hubs;
using K8sServer.Watchers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Services
{
    public class ResourcesService
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly IHubContext<KubernetesHub> _hubContext;
        private readonly Dictionary<string, List<IResourceWatcher>> _watchers = new Dictionary<string, List<IResourceWatcher>>();
        private readonly Dictionary<string, CancellationTokenSource> _namespaceCancellations = new Dictionary<string, CancellationTokenSource>();
        private readonly ILogger<ResourcesService> _logger;


        public ResourcesService(KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext, ILogger<ResourcesService> logger)
        {
            _kubernetesClientService = kubernetesClientService;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task ActiveWatchersForNamespace(string namespaceName)
        {
            if (_namespaceCancellations.ContainsKey(namespaceName))
            {
                StopWatchers(namespaceName); // Якщо для цього namespace вже є активні вотчери, спершу зупинимо їх.
            }

            var cts = new CancellationTokenSource();
            var watchersForNamespace = new List<IResourceWatcher>();

            var resourceTypes = new List<ResourceType> 
            {
                ResourceType.Deployment,
                ResourceType.Pod,
                ResourceType.ConfigMap,
                ResourceType.CronJob,
                ResourceType.DaemonSet,
                ResourceType.Ingress,
                ResourceType.Job,
                ResourceType.LimitRange,
                ResourceType.NetworkPolicy,
                ResourceType.PersistentVolumeClaim,
                ResourceType.ReplicaSet,
                ResourceType.ResourceQuota,
                ResourceType.Secret,
                ResourceType.Service,
                ResourceType.StatefulSet,
                ResourceType.Namespace
            };

            foreach (var resourceType in resourceTypes)
            {
                var watcher = WatcherFactory.CreateWatcher(resourceType, _kubernetesClientService, _hubContext);
                watchersForNamespace.Add(watcher);
                
                _ = watcher.StartWatchingAsync(cts.Token, namespaceName);
            }

            _watchers[namespaceName] = watchersForNamespace;
            _namespaceCancellations[namespaceName] = cts;
        }
        public async Task ActiveNamespacesWatcher()
        {
            
            var namespacesWatcher = WatcherFactory.CreateWatcher(ResourceType.Namespace, _kubernetesClientService, _hubContext);
            await namespacesWatcher.StartWatchingAsync(CancellationToken.None);
        }

        public void StopWatchers(string namespaceName)
        {
            if (_namespaceCancellations.TryGetValue(namespaceName, out var cts))
            {
                cts.Cancel(); 
                _namespaceCancellations.Remove(namespaceName); 
            }

            if (_watchers.TryGetValue(namespaceName, out var watchers))
            {
                _watchers.Remove(namespaceName); 
            }
        }

        public async Task GetResourcesAsync(string namespaceName)
        {
            var client = _kubernetesClientService.GetClient();
            var allResources = new Dictionary<string, object>
            {
                //CoreV1
                ["Pods"] = await client.CoreV1.ListNamespacedPodAsync(namespaceName),
                ["ConfigMaps"] = await client.CoreV1.ListNamespacedConfigMapAsync(namespaceName),
                ["LimitRanges"] = await client.CoreV1.ListNamespacedLimitRangeAsync(namespaceName),
                ["PersistentVolumesClaim"] = await client.CoreV1.ListNamespacedPersistentVolumeClaimAsync(namespaceName),
                ["ResourceQuotas"] = await client.CoreV1.ListNamespacedResourceQuotaAsync(namespaceName),
                ["Services"] = await client.CoreV1.ListNamespacedServiceAsync(namespaceName),
                ["Secrets"] = await client.CoreV1.ListNamespacedSecretAsync(namespaceName),
                //AppsV1
                ["DaemonSets"] = await client.AppsV1.ListNamespacedDaemonSetAsync(namespaceName),
                ["Deployments"] = await client.AppsV1.ListNamespacedDeploymentAsync(namespaceName),
                ["ReplicaSests"] = await client.AppsV1.ListNamespacedReplicaSetAsync(namespaceName),
                ["StatefulSets"] = await client.AppsV1.ListNamespacedStatefulSetAsync(namespaceName),
                //BatchV1
                ["CronJobs"] = await client.BatchV1.ListNamespacedCronJobAsync(namespaceName),
                ["Jobs"] = await client.BatchV1.ListNamespacedJobAsync(namespaceName),
                //NetworkingV1
                ["Ingresses"] = await client.NetworkingV1.ListNamespacedIngressAsync(namespaceName),
                ["NetworkPolicies"] = await client.NetworkingV1.ListNamespacedNetworkPolicyAsync(namespaceName),
            };
            await _hubContext.Clients.Group(namespaceName).SendAsync("ReceiveNamespaceResources", allResources);

        }

    }
}



