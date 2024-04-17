namespace K8sServer.ErrorHandling
{
    public class KubernetInitializationException : Exception
    {
        public KubernetInitializationException(string message) : base(message) {  }
    }
}
