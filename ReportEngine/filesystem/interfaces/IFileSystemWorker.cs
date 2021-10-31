using System.Collections.Generic;
using ReportEngine.models;

namespace ReportEngine.filesystem.interfaces
{
    public interface IFileSystemWorker
    {
        T ReadFromJsonFile<T>(string filePath);
        void DeleteDirectory(string dirPath);
        void CreateDirectory(string dirPath);
        void ChangeDirectory(string dirPath);
        void ExtractFile(string zipFilePath, string path);
        string GetCurrentDirectory();
        void MoveDirectory(string source, string destination);
        void CopyFile(string filePathSource, string filePathDestination);
        void SetPermissionExecute(string filePath);
        void WriteJsonInFile<T>(T t, string filePath);
        List<string> GetFilePathsByExtensions(List<string> path, List<string> ext);
    }
}