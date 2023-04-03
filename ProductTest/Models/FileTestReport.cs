using ProductTest.Interfaces;

namespace ProductTest.Models;

public class FileTestReport : TestReport, ITestReport
{
    public string FilePath { get; protected set; }
    public string FileName { get; protected set; }
    public static FileTestReport Create(string serialNumber,
                        Common.Workstation workstation,
                        IEnumerable<Common.TestStep> testSteps,
                        string filePath = "")
    {
        return new FileTestReport(serialNumber, workstation, testSteps, filePath);
    }

    protected FileTestReport(string serialNumber,
                            Common.Workstation workstation,
                            IEnumerable<Common.TestStep> testSteps,
                            string filePath) : base(serialNumber, workstation, testSteps)
    {
        FilePath = filePath;
        GetFileName();
    }

    public static FileTestReport CreateFromFile(string path)
    {
        if (File.Exists(path))
        {
            return new FileTestReport(path);
        }
        else
        {
            throw new FileNotFoundException("File not found", path);
        }
    }

    protected FileTestReport(string path) : base()
    {
        FilePath = path;
        var linesOfText = File.ReadAllLines(path);
        SetSerialNumber(linesOfText);
        SetStationName(linesOfText);
        SetTestSteps(path);
        SetTestDateAndTime();
        SetStatus();
        SetFailedStepData();
        SetTestSocket();
        SetBoardTestingTime();
    }

    public void SaveReport(string directoryPath)
    {
        var testReport = new List<string>
        {
            $"PanelBarcode:\t{SerialNumber}",
            $"Operator:\t{Workstation.Name}",
            $"ImageBarcode:\t{SerialNumber}",
            $""
        };

        foreach (Common.TestStep testStep in TestSteps)
        {
            testReport.Add($"TestName:\t{testStep.Name}");
            if (!string.IsNullOrEmpty(testStep.Type))
                testReport.Add($"TestType:\t{testStep.Type}");
            testReport.Add($"Date:\t{testStep.DateTimeFinish.Month:00}/{testStep.DateTimeFinish.Day:00}/{testStep.DateTimeFinish.Year}");
            testReport.Add($"Time:\t{testStep.DateTimeFinish.Hour:00}:{testStep.DateTimeFinish.Minute:00}:{testStep.DateTimeFinish.Second:00}");
            testReport.Add($"Result:\t{testStep.Status}");
            if (!string.IsNullOrEmpty(testStep.Value))
                testReport.Add($"Value:\t{testStep.Value}");
            if (!string.IsNullOrEmpty(testStep.Unit))
                testReport.Add($"Units:\t{testStep.Unit}");
            if (!string.IsNullOrEmpty(testStep.LowerLimit))
                testReport.Add($"LowerLimit:\t{testStep.LowerLimit}");
            if (!string.IsNullOrEmpty(testStep.UpperLimit))
                testReport.Add($"UpperLimit:\t{testStep.UpperLimit}");
            if (!string.IsNullOrEmpty(testStep.Failure))
                testReport.Add($"FailDesc:\t{testStep.Failure}");
            testReport.Add("~#~");
        }

        var testReportPath = Path.Combine(directoryPath, FileName);
        File.WriteAllLines(testReportPath, testReport);
        FilePath = testReportPath;
    }

    private void GetFileName()
    {
        FileName = $"{TestDateTimeStarted.Month:00}{TestDateTimeStarted.Day:00}{TestDateTimeStarted.Year}_" +
                    $"{TestDateTimeStarted.Hour:00}{TestDateTimeStarted.Minute:00}{TestDateTimeStarted.Second:00}_{SerialNumber}.txt";
    }

    private void SetSerialNumber(IEnumerable<string> linesOfText)
    {
        var serialNumber = string.Empty;
        foreach (string line in linesOfText)
        {
            if (line.Contains("PanelBarcode:") || line.Contains("ImageBarcode:"))
            {
                string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                serialNumber = SplittedLine[1].Trim();
                break;
            }
        }
        if (serialNumber == string.Empty) throw new Exception("Serial number is missing!");
        else SerialNumber = serialNumber;
    }
    private void SetStationName(IEnumerable<string> linesOfText)
    {
        var workstation = string.Empty;
        foreach (string line in linesOfText)
        {
            if (line.Contains("Operator:"))
            {
                string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                workstation = SplittedLine[1];
                break;
            }
        }
        if (workstation == string.Empty)
        {
            throw new Exception("Operator field is missing!");
        }
        else
        {
            Workstation = new Workstation(workstation);
        }
    }

    private void SetTestSteps(string path)
    {
        var testSteps = new List<TestStep>();
        string testReportText = File.ReadAllText(path);
        string[] splittedText = testReportText.Split("~#~");
        foreach (string testCase in splittedText)
        {
            if (string.IsNullOrWhiteSpace(testCase))
            {
                continue;
            }

            string name = null;
            string date = null;
            string time = null;
            Common.TestStatus status = Common.TestStatus.NotSet;
            string type = null;
            string value = null;
            string unit = null;
            string lowerlimit = null;
            string upperlimit = null;

            string[] line = testCase.Split("\n");
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Contains("Date:")) date = GetFieldValue(line[i]);
                if (line[i].Contains("Time:")) time = GetFieldValue(line[i]);
                if (line[i].Contains("TestName:")) name = GetFieldValue(line[i]);
                if (line[i].Contains("TestType:")) type = GetFieldValue(line[i]);
                if (line[i].Contains("Result:"))
                {
                    // Converts string to have first letter upper-case. passed => Passed
                    string statusValue = GetFieldValue(line[i]).ToLower();
                    string capitalizedStatusValue = string.Concat(statusValue[0].ToString().ToUpper(), statusValue.AsSpan(1));
                    status = Enum.Parse<Common.TestStatus>(capitalizedStatusValue);
                }
                if (line[i].Contains("Value:")) value = GetFieldValue(line[i]);
                if (line[i].Contains("Units:")) unit = GetFieldValue(line[i]);
                if (line[i].Contains("LowerLimit:")) lowerlimit = GetFieldValue(line[i]);
                if (line[i].Contains("UpperLimit:")) upperlimit = GetFieldValue(line[i]);

                if (i == line.Length - 1)
                {
                    DateTime datetime = ConvertDateAndTime(date, time);

                    var testStep = new TestStep(name, datetime, status);
                    testStep.Type = type;
                    testStep.Value = value;
                    testStep.Unit = unit;
                    testStep.LowerLimit = lowerlimit;
                    testStep.UpperLimit = upperlimit;

                    if (testStep.Name != string.Empty && status != Common.TestStatus.NotSet)
                        testSteps.Add(testStep);
                }
            }
        }
        TestSteps = testSteps;
    }
    private static string GetFieldValue(string lineOfText)
    {
        return lineOfText.Split("\t")[1].Trim().Replace(',', '_');
    }
    private static DateTime ConvertDateAndTime(string date, string time)
    {
        if (date == string.Empty || time == string.Empty) return DateTime.MinValue;
        var year = int.Parse(date.Substring(6, 4));
        var month = int.Parse(date[..2]);
        var day = int.Parse(date.Substring(3, 2));
        var hour = int.Parse(time[..2]);
        var minute = int.Parse(time.Substring(3, 2));
        var second = int.Parse(time.Substring(6, 2));

        var Converted = new DateTime(year, month, day, hour, minute, second);
        return Converted;
    }
}