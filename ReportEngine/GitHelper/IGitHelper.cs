using System.Threading.Tasks;

namespace ReportEngine.GitHelper
{
    public interface IGitHelper
    {
        string GetTag(string path);
        Task SendFileAsync(string fileName, string content);
    }
}