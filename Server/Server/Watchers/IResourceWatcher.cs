namespace K8sServer.Watchers
{
    public interface IResourceWatcher
    {
        Task StartWatchingAsync(CancellationToken cancellationToke, string? namespaceName = null);
        void StopWatching();
    }

}