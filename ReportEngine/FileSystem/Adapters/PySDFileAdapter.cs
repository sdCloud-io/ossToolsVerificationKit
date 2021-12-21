using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ReportEngine.filesystem.adapters
{
    public class PySDFileAdapter : IAdapter
    {
        public Dictionary<string, List<string>> ReadValues(string filePath)
        {
            var resultFilePath = filePath + ".result";
            string fileText = "";
            try
            {
                fileText = File.ReadAllText(resultFilePath);
            }
            catch (FileNotFoundException e)
            {
                return new Dictionary<string, List<string>>();
            }

            var fileDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(fileText);

            return fileDictionary?.ToDictionary(column => column.Key,
                column => column.Value.Select(e => e.Value).ToList());
        }
    }
}