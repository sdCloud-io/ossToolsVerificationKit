namespace ReportEngine.network.interfaces
{
    public interface IGitLoader
    {
        void DownloadRepository(string repositoryUrl, string instrumentRepositoryUrl, string instrumentPath);
    }
}