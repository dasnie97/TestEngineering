using TestEngineering.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

public class MESService
{
    public MESService()
    {

    }

    private async Task<bool> AppearedInMES (TestReport testReport)
    {
        var appeared = false;
        await Task.Run(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                if (ExistsInMES(testReport))
                {
                    appeared = true;
                    break;
                }
                else
                {
                    appeared = false;
                    Task.Delay(60000).Wait();
                }
            }
        });
        return appeared;
    }

    private bool ExistsInMES(TestReport testReport)
    {
        String sql = GetSQLQuery("MESQuery.txt");
        sql = sql.Replace("UUTSerialNumber", testReport.SerialNumber);
        
        SqlConnection connection;
        connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MES"].ConnectionString);
        connection.Open();

        SqlCommand command = new SqlCommand(sql, connection);
        var dataPresentInSystem = false;
        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string serialNumber = reader.GetString(0);
                string operation = reader.GetString(1);
                DateTime timeStarted = reader.GetDateTime(2).ToLocalTime();
                string testOperator = reader.GetString(3);
                TestStatus status = Enum.Parse<TestStatus>(reader.GetString(4));

                if (timeStarted == testReport.TestDateTimeStarted && status == testReport.Status)
                    dataPresentInSystem = true;
            }
        }
        connection.Close();
        return dataPresentInSystem;
    }

    private string GetSQLQuery(string fileName)
    { 
        // Make sure to set Build Action of target file to Embedded resource
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = assembly.GetManifestResourceNames()
                            .Single(str => str.EndsWith(fileName));

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            string result = reader.ReadToEnd();
            return result;
        }
    }
}