using System.Collections.Generic;

namespace ReportEngine.filesystem.interfaces
{
    public interface IFileSystemHelper
    {
        T ReadFromJsonFile<T>(string filePath);
        void DeleteDirectory(string dirPath);
        void CreateDirectory(string dirPath);
        void ChangeDirectory(string dirPath);
        void ExtractFile(string zipFilePath, string path);
        string GetCurrentDirectory();
        void CreateSymbolicLinkDirectory(string source, string destination);
        void CopyFile(string filePathSource, string filePathDestination);
        void SetPermissionExecute(string filePath);
        void WriteJsonInFile<T>(T t, string filePath);
        List<string> GetFilePathsByExtensions(string ext);
    }
}