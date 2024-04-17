using k8s;
using K8sServer.ErrorHandling;
using System.Data;




namespace K8sServer.Services
{
    public class KubernetesClientService
    {
        private IKubernetes _client;

        public KubernetesClientService()
        {
            InitializeInCluster();
        }

        private void InitializeInCluster()
        {
            try
            {
                _client = new Kubernetes(KubernetesClientConfiguration.InClusterConfig());
            }
            catch (Exception)
            {
                _client = null;
            }
        }

        public void InitializeWithKubeConfig(byte[] kubeConfigContent)
        {
            try
            {
                if (kubeConfigContent == null || kubeConfigContent.Length == 0)
                {
                    throw new KubernetInitializationException("KubeConfig file is empty or null.");
                }
                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(new MemoryStream(kubeConfigContent));
                _client = new Kubernetes(config);
            }
            catch (KubernetInitializationException ex)
            {
                throw; 
            }
            catch (Exception ex)
            {
                throw new KubernetInitializationException("Unexpected error during initialization.");
            }
        }

        public IKubernetes GetClient()
        {
            if (_client == null)
            {
                throw new InvalidOperationException("Kubernetes клієнт не ініціалізовано.");
            }
            return _client;
        }
    }

}
