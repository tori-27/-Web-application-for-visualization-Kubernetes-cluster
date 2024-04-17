using K8sServer.Hubs;
using K8sServer.Watchers.Implementations;
using K8sServer.Watchers;
using Microsoft.AspNetCore.SignalR;

namespace K8sServer.Services
{
    public class WatcherFactory
    {
        public static IResourceWatcher CreateWatcher(ResourceType resourceType, KubernetesClientService kubernetesClientService, IHubContext<KubernetesHub> hubContext)
        {
            switch (resourceType)
            {
                case ResourceType.Pod:
                    return new PodsWatcher(kubernetesClientService, hubContext);
                case ResourceType.Deployment:
                    return new DeploymentsWatcher(kubernetesClientService, hubContext);
                case ResourceType.ReplicaSet:
                    return new ReplicaSetsWatcher(kubernetesClientService, hubContext);
                case ResourceType.StatefulSet:
                    return new StatefulSetsWatcher(kubernetesClientService, hubContext);
                case ResourceType.ConfigMap:
                    return new ConfigMapsWatcher(kubernetesClientService, hubContext);
                case ResourceType.CronJob:
                    return new CronJobsWatcher(kubernetesClientService, hubContext);
                case ResourceType.Job:
                    return new JobsWatcher(kubernetesClientService, hubContext);
                case ResourceType.Ingress:
                    return new IngressesWatcher(kubernetesClientService, hubContext);
                case ResourceType.LimitRange:
                    return new LimitRangesWatcher(kubernetesClientService, hubContext);
                case ResourceType.PersistentVolumeClaim:
                    return new PersistentVolumesClaimWatcher(kubernetesClientService, hubContext);
                case ResourceType.DaemonSet:
                    return new DaemonSetsWatcher(kubernetesClientService, hubContext);
                case ResourceType.NetworkPolicy:
                    return new NetworkPoliciesWatcher(kubernetesClientService, hubContext);
                case ResourceType.ResourceQuota:
                    return new ResourceQuotasWatcher(kubernetesClientService, hubContext);
                case ResourceType.Secret:
                    return new SecretsWatcher(kubernetesClientService, hubContext);
                case ResourceType.Service:
                    return new ServicesWatcher(kubernetesClientService, hubContext);
                case ResourceType.Namespace:
                    return new NamespacesWatcher(kubernetesClientService, hubContext);  
                default:
                    throw new ArgumentException("Unsupported resource type");
            }
        }
    }

    public enum ResourceType
    {
        Pod, 
        Deployment, 
        ConfigMap,
        CronJob,
        DaemonSet,
        Ingress,
        Job,
        LimitRange,
        NetworkPolicy,
        PersistentVolumeClaim,
        ReplicaSet,
        ResourceQuota,
        Secret,
        Service,
        StatefulSet,
        Namespace
    }
}
