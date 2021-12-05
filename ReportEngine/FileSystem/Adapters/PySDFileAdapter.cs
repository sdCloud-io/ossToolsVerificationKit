using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
                Console.WriteLine(e);
                return new Dictionary<string, List<string>>();
            }

            var fileDictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(fileText);
            //var fileDictionary = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(fileText);

            return fileDictionary?.ToDictionary(column => column.Key,
                column => column.Value.Select(e => e.Value).ToList());
        }
    }

    class NumericJson
    {
        [JsonProperty("1")]
        public int Item1 { get; set; }

        [JsonProperty("2")]
        public string Item2 { get; set; }
    }
}