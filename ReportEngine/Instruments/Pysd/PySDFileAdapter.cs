using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ReportEngine.Instruments.Pysd
{
    public class PySdFileValueReader : IValueReader
    {
        public Dictionary<string, string> ReadValues(string filePath)
        {
            var resultFilePath = filePath + ".result";
            string fileText = "";
            try
            {
                fileText = File.ReadAllText(resultFilePath);
            }
            catch (FileNotFoundException)
            {
                return new Dictionary<string, string>();
            }

            var fileDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(fileText);

            return fileDictionary?.ToDictionary(column => column.Key,
                column => column.Value.Select(e => e.Value).Last());
        }
    }
}