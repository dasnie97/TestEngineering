using TestEngineering.Models;
using TestEngineering.Interfaces;
using Microsoft.Extensions.Configuration;
using TestEngineering.Settings;

namespace TestEngineering.Services;

public class FileProcessorService : IFileProcessor
{
    public string InputDir { get; private set; }
    public string OutputDir { get; private set; }
    public string CopyDir { get; private set; }

    private readonly IConfiguration _configuration;
    private string dateNamedCopyDirectory;
    private bool copyingEnabled;

    public FileProcessorService(string inputDirectory, string outputDirectory, string copyDirectory)
    {
        InputDir = inputDirectory;
        OutputDir = outputDirectory;
        CopyDir = copyDirectory;
        SetupCopying();
    }

    public FileProcessorService(IConfiguration config)
    {
        _configuration = config;
        InputDir = _configuration[FileProcessorSettingsKeys.InputDir];
        OutputDir = _configuration[FileProcessorSettingsKeys.OutputDir];
        CopyDir = _configuration[FileProcessorSettingsKeys.CopyDir];
        SetupCopying();
    }

    public IEnumerable<FileTestReport> LoadFiles()
    {
        IFileLoader fileLoader = new FileLoaderService();
        IEnumerable<FileTestReport> loaded = fileLoader.GetTestReportFiles(InputDir);
        return loaded;
    }

    public string CopyFile(FileTestReport testReport)
    {
        var copiedFilePath = string.Empty;
        if (copyingEnabled)
        {
            var destinationFileName = Path.Combine(dateNamedCopyDirectory, Path.GetFileName(testReport.FilePath));
            File.Copy(testReport.FilePath, destinationFileName);
            copiedFilePath = destinationFileName;
        }
        return copiedFilePath;
    }

    public string MoveFile(FileTestReport testReport)
    {
        var destinationFilePath = Path.Combine(OutputDir, Path.GetFileName(testReport.FilePath));
        File.Move(testReport.FilePath, destinationFilePath, true);
        return destinationFilePath;
    }

    public void DeleteFile(FileTestReport testReport)
    {
        File.Delete(testReport.FilePath);
    }

    private void SetupCopying()
    {
        if (Directory.Exists(CopyDir))
        {
            copyingEnabled = true;
            var dateNamedDirectory = Path.Combine(CopyDir, $"{DateTime.Now.Year}_{DateTime.Now.Month.ToString("00")}");
            Directory.CreateDirectory(dateNamedDirectory);
            dateNamedCopyDirectory = dateNamedDirectory;
        }
    }
}
