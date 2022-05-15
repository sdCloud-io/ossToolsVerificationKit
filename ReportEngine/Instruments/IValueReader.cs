using System.Collections.Generic;

namespace ReportEngine.Instruments
{
    public interface IValueReader
    {
        Dictionary<string, string> ReadValues(string filePath);
    }
}