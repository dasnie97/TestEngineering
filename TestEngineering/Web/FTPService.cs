using FluentFTP;
using Microsoft.Extensions.Configuration;

namespace TestEngineering.Web;

public class FTPService : IFTPService, IDisposable
{
    private FtpClient _client;
    private bool connectionEstablished = false;

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

    public void Dispose()
    {
        CloseConnection();
    }

    public void Upload(string filePath)
    {
        if (!connectionEstablished)
        {
            InitializeConnection();
        }

        var status = _client.UploadFile(filePath, $"/{Path.GetFileName(filePath)}", verifyOptions: FtpVerify.Throw);
        if (status != FtpStatus.Success)
        {
            throw new Exception($"There was an error processing file {filePath}");
        }
    }

    private void InitializeConnection()
    {
        var ftpServer = _host;
        var ftpUsername = _user;
        var ftpPassword = _pass;

        _client = new FtpClient(ftpServer, ftpUsername, ftpPassword);
        _client.AutoConnect();
        connectionEstablished = true;
    }

    private void CloseConnection()
    {
        if (connectionEstablished)
        {
            _client.Disconnect();
            connectionEstablished = false;
        }
    }
}