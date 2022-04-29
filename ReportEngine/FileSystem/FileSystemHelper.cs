using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;

namespace ReportEngine.filesystem
{
    public class FileSystemHelper : IFileSystemHelper
    {
        private readonly Configuration _configuration;

        public FileSystemHelper(IOptions<Configuration> configuration)
        {
            _configuration = configuration.Value;
        }

        public T ReadFromJsonFile<T>(string filePath)
        {
            using var jsonFile = new StreamReader(filePath);
            return JsonSerializer.Deserialize<T>(jsonFile.ReadToEnd());
        }

        public void ChangeDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            Directory.SetCurrentDirectory(dirPath);
        }

        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public void CreateSymbolicLinkDirectory(string source, string destination)
        {
            var sdeInitProcess = new Process
            {
                StartInfo = new ProcessStartInfo("ln")
                {
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    Arguments = $"-s {source} {destination}",
                    UseShellExecute = true,
                }
            };
            sdeInitProcess.Start();
            sdeInitProcess.WaitForExit();
        }

        public void CopyFile(string filePathSource, string filePathDestination)
        {
            File.Copy(filePathSource, filePathDestination, true);
        }

        public void SetPermissionExecute(string filePath)
        {
            Process.Start("chmod", $"+x {filePath}");
        }

        public void WriteJsonInFile<T>(T t, string filePath)
        {
            var json = JsonSerializer.Serialize(t);
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                var fileStream = File.Create("testReport.json");
                fileStream.Close();
                File.WriteAllText("testReport.json", json);
            }
        }

        public List<string> GetFilePathsByExtensions(string fileExts)
        {
            var result = new List<string>();
            foreach (var path in _configuration.Models.Select(model => model.Path))
            {
                result.AddRange(Directory.GetFiles(path, fileExts, SearchOption.AllDirectories));
            }

            return result;
        }
    }
}