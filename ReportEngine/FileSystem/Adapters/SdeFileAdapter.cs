using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ReportEngine.filesystem.adapters
{
    public class SdeFileAdapter : IAdapter
    {
        private readonly ILogger<SdeFileAdapter> _logger;

        public SdeFileAdapter(ILogger<SdeFileAdapter> logger)
        {
            _logger = logger;
        }

        public Dictionary<string, List<string>> ReadValues(string filePath)
        {
            if (!filePath.Contains(".mdl")) return new Dictionary<string, List<string>>();
            var indexOfLastSlash = filePath.LastIndexOf("/", StringComparison.Ordinal);
            var resultFilePath = filePath.Insert(indexOfLastSlash, "/output").Replace(".mdl", ".txt");
            string[] lines;
            try
            {
                lines = File.ReadAllLines(resultFilePath);
            }
            catch (Exception e)
            {
                return new Dictionary<string, List<string>>();
            }

            string[] keys = { };
            var readValues = new Dictionary<string, List<string>>();
            for (var i = 0; i < lines.Length; i++)
            {
                var values = lines[i].Split('\t');
                if (i == 0)
                {
                    keys = values;
                    foreach (var key in keys)
                    {
                        readValues.Add(key, new List<string>());
                    }

                    continue;
                }

                for (var j = 0; j < values.Length; j++)
                {
                    readValues.TryGetValue(keys[j], out var listOfValues);
                    listOfValues?.Add(values[j]);
                }
            }

            return new Dictionary<string, List<string>>();
            return readValues;
        }
    }
}