using System.Collections.Generic;

namespace ReportEngine.filesystem.adapters
{
    public interface IAdapter
    {
        Dictionary<string, List<string>> ReadValues(string filePath);
    }
}