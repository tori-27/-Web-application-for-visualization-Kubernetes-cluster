using k8s;
using K8sServer.ErrorHandling;
using K8sServer.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;


namespace K8sServer.Hubs
{
    public class KubernetesHub : Hub
    {
        private readonly KubernetesClientService _kubernetesClientService;
        private readonly ResourcesService _resourcesService;
        private readonly ILogger<KubernetesHub> _logger;

        public KubernetesHub(ILogger<KubernetesHub> logger, KubernetesClientService kubernetesClientService, ResourcesService watcherService)
        {
            _kubernetesClientService = kubernetesClientService;
            _resourcesService = watcherService;
            _logger = logger;
   
        }

        public async Task JoinNamespaceGroup(string namespaceName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, namespaceName);
            await _resourcesService.GetResourcesAsync(namespaceName);
            _ = Task.Run(async () =>
            {
                try
                {
                    await _resourcesService.ActiveWatchersForNamespace(namespaceName);
                    _logger.LogInformation($"Join to namespace: {namespaceName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error activating watchers for namespace {namespaceName}: {ex.Message}");
                }
            }); 
        }

        public async Task LeaveNamespaceGroup(string namespaceName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, namespaceName);
            _resourcesService.StopWatchers(namespaceName);
            _logger.LogInformation($"Leave namespace {namespaceName}");
        }
        public async Task JoinToNamespaces()
        {
            var client = _kubernetesClientService.GetClient();
            var namespacesResponse = await client.CoreV1.ListNamespaceAsync();
            var namespaces = namespacesResponse.Items.Select(ns => ns.Metadata.Name).ToList();
            await Clients.Caller.SendAsync("ReceiveNamespaces", namespaces);
            _ = Task.Run(async () =>
            {
                try
                {
                    await _resourcesService.ActiveNamespacesWatcher();

                }catch(Exception ex)
                {
                    _logger.LogError($"Error activating watchers for namespaces: {ex.Message}");
                }
            });
        }

        public async Task UploadKubeConfig(string base64KubeConfig)
        {
            try
            {
                var base64Data = base64KubeConfig.Contains(",") ? base64KubeConfig.Split(',')[1] : base64KubeConfig;
                var kubeConfigContent = Convert.FromBase64String(base64Data);
                _kubernetesClientService.InitializeWithKubeConfig(kubeConfigContent);
                await Clients.Caller.SendAsync("ReceiveUploadStatus", new { Status = "OK", Message = "Client was succesfully intitialized" });
            }
            catch (KubernetInitializationException ex)
            {
                _logger.LogError($"Kubernet client initialization error: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveUploadStatus", new { Status = "Error", Message = $"Initialization error: {ex.Message}" });
            }
            catch(Exception ex)
            {
                _logger.LogError($"Unexpected error during Kubernetes client initialization: {ex.Message}");
                throw new KubernetInitializationException("Unexpected error during initialization.");
            }
        }

    }
} 
