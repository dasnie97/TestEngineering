using TestEngineering.Models;

namespace TestEngineering.Interfaces;

public interface IFileLoader
{
    IEnumerable<string> GetFiles(string inputDirectoryPath);
    IEnumerable<FileTestReport> GetTestReportFiles(string inputDirectoryPath);
}
