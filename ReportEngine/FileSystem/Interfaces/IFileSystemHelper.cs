using System.Collections.Generic;

namespace ReportEngine.filesystem.interfaces
{
    public interface IFileSystemHelper
    {
        void ChangeDirectory(string dirPath);
        string GetCurrentDirectory();
        void CreateSymbolicLinkDirectory(string source, string destination);
        void CopyFile(string filePathSource, string filePathDestination);
        void SetPermissionExecute(string filePath);
        void WriteJsonInFile<T>(T t, string filePath);
        List<string> GetModelPathsByExtension(string ext);
        T ReadFromJsonFile<T>(string filePath);
    }
}