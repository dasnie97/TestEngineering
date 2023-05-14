using FluentFTP;
using Microsoft.Extensions.Configuration;
using TestEngineering.Interfaces;
using TestEngineering.Settings;

namespace TestEngineering.Services;

public class FTPService : IFTP
{
    private readonly IConfiguration _configuration;
    private readonly string _host;
    private readonly string _user;
    private readonly string _pass;

    public FTPService(string host, string user, string pass)
    {
        _host = host;
        _user = user;
        _pass = pass;
    }

    public FTPService(IConfiguration configuration)
    {
        _configuration = configuration;
        _host = _configuration[FtpSettingsKeys.Host];
        _user = _configuration[FtpSettingsKeys.User];
        _pass = _configuration[FtpSettingsKeys.Password];
    }

    public void Upload(string filePath)
    {
        using (FtpClient _client = InitializeConnection())
        {
            var status = _client.UploadFile(filePath, $"/{Path.GetFileName(filePath)}", FtpRemoteExists.Overwrite, false, FtpVerify.Retry);
            if (status != FtpStatus.Success)
            {
                throw new Exception($"There was an error processing file {filePath}");
            }
        }
    }

    private FtpClient InitializeConnection()
    {
        var ftpServer = _host;
        var ftpUsername = _user;
        var ftpPassword = _pass;

        var _client = new FtpClient(ftpServer, ftpUsername, ftpPassword);
        _client.AutoConnect();
        return _client;
    }
}