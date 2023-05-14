using TestEngineering.Models;

namespace TestEngineering.Interfaces;

public interface IFileProcessor
{
    public IEnumerable<FileTestReport> LoadFiles();
    public string MoveFile(FileTestReport testReport);
    public string CopyFile(FileTestReport testReport);
    public void DeleteFile(FileTestReport testReport);
}
