using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ReportEngine.FileSystem;

namespace ReportEngine.filesystem
{
    public class FileSystemHelper : IFileSystemHelper
    {
        private readonly Configuration.Configuration _configuration;

        public FileSystemHelper(IOptions<Configuration.Configuration> configuration)
        {
            _configuration = configuration.Value;
            Directory.SetCurrentDirectory(_configuration.BuildDir);
        }

        public T ReadFromJsonFile<T>(string filePath)
        {
            using var jsonFile = new StreamReader(filePath);
            return JsonSerializer.Deserialize<T>(jsonFile.ReadToEnd());
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
        
        public List<string> GetModelPathsByExtension(string fileExts)
        {
            var modelPaths = new List<string>();
            foreach (var path in _configuration.Models)
            {
                modelPaths.AddRange(Directory.GetFiles(path, fileExts, SearchOption.AllDirectories));
            }

            return modelPaths;
        }
    }
}