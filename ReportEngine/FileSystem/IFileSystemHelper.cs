using System.Collections.Generic;

namespace ReportEngine.FileSystem
{
    public interface IFileSystemHelper
    {
        string GetCurrentDirectory();
        void CreateSymbolicLinkDirectory(string source, string destination);
        void CopyFile(string filePathSource, string filePathDestination);
        void SetPermissionExecute(string filePath);
        List<string> GetModelPathsByExtension(string ext);
        T ReadFromJsonFile<T>(string filePath);
    }
}