using ProductTest.Common;
using ProductTest.Interfaces;

namespace ProductTest;

public class TestReport : TestReportBase, ITestReport
{
    //TODO: figure out how to set one of constructors in base class.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path to existing test report file.</param>
    public TestReport(string path)
    {
        if (File.Exists(path))
        {
            var text = File.ReadAllLines(path);
            SerialNumber = GetSerialNumber(text);
            TestSteps = GetTestSteps(path);
            Status = GetStatus();
            TestDateTimeStarted = GetTestDateAndTime();
            Failure = GetFailedStepData();
            Workstation = GetStationName(text);
            TestingTime = GetBoardTestingTime();
            FixtureSocket = GetTestSocket(text);
        }
        else
        {
            throw new FileNotFoundException();
        }
    }

    public TestReport(
    string serialNumber,
    string workstation,
    DateTime testStarted,
    IEnumerable<TestStep> testSteps,
    string failure = "",
    string fixtureSocket = "",
    TimeSpan? testingTime = null
    )
    {
        SerialNumber = serialNumber;
        Workstation = new Workstation(workstation);
        TestDateTimeStarted = testStarted;
        TestSteps = testSteps;
        Failure = failure;
        FixtureSocket = fixtureSocket;
        TestingTime = testingTime;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="directoryPath">Output directory where test report should be saved.</param>
    /// <returns></returns>
    public string SaveReport(string directoryPath)
    {
        var logFileName = $"{TestDateTimeStarted.Month:00}{TestDateTimeStarted.Day:00}{TestDateTimeStarted.Year}_{TestDateTimeStarted.Hour:00}{TestDateTimeStarted.Minute:00}{TestDateTimeStarted.Second:00}_{SerialNumber}.txt";
        var buffor = new List<string>
        {
            $"PanelBarcode:\t{SerialNumber}",
            $"TestProgram:\t{""}",
            $"TestProgramVer:\t{"1.0"}",
            $"Operator:\t{Workstation.Name}",
            $"ImageBarcode:\t{SerialNumber}",
            $""
        };

        foreach (TestStepBase testStep in TestSteps)
        {
            buffor.Add($"TestName:\t{testStep.TestName}");
            if (!string.IsNullOrEmpty(testStep.TestType))
                buffor.Add($"TestType:\t{testStep.TestType}");
            buffor.Add($"Date:\t{testStep.TestDateTimeFinish.Month:00}/{testStep.TestDateTimeFinish.Day:00}/{testStep.TestDateTimeFinish.Year}");
            buffor.Add($"Time:\t{testStep.TestDateTimeFinish.Hour:00}:{testStep.TestDateTimeFinish.Minute:00}:{testStep.TestDateTimeFinish.Second:00}");
            buffor.Add($"Result:\t{testStep.TestStatus}");
            if (!string.IsNullOrEmpty(testStep.TestValue))
                buffor.Add($"Value:\t{testStep.TestValue}");
            if (!string.IsNullOrEmpty(testStep.ValueUnit))
                buffor.Add($"Units:\t{testStep.ValueUnit}");
            if (!string.IsNullOrEmpty(testStep.TestLowerLimit))
                buffor.Add($"LowerLimit:\t{testStep.TestLowerLimit}");
            if (!string.IsNullOrEmpty(testStep.TestUpperLimit))
                buffor.Add($"UpperLimit:\t{testStep.TestUpperLimit}");
            if (!string.IsNullOrEmpty(testStep.Failure))
                buffor.Add($"FailDesc:\t{testStep.Failure}");
            buffor.Add("~#~");
        }

        var logFilePath = Path.Combine(directoryPath, logFileName);

        File.WriteAllLines(logFilePath, buffor);

        return logFilePath;
    }

    private static string GetSerialNumber(IEnumerable<string> text)
    {
        var serialNumber = string.Empty;
        foreach (string line in text)
        {
            if (line.Contains("PanelBarcode:") || line.Contains("ImageBarcode:"))
            {
                string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                serialNumber = SplittedLine[1].Trim();
                break;
            }
        }
        if (serialNumber == string.Empty) throw new Exception("Serial number is missing!");
        else return serialNumber;
    }
    private static List<TestStep> GetTestSteps(string path)
    {
        var testSteps = new List<TestStep>();
        string logFileText = File.ReadAllText(path);
        string[] splittedText = logFileText.Split("~#~");
        foreach (string testCase in splittedText)
        {
            string[] logFileData = new string[9];
            string[] splittedTestCase = testCase.Split("\n");
            for (int i = 0; i < splittedTestCase.Length; i++)
            {
                if (splittedTestCase[i].Contains("Date:")) logFileData[0] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                else throw new Exception($"Date field is missing in file {path}!");
                if (splittedTestCase[i].Contains("Time:")) logFileData[1] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                else throw new Exception($"Time field is missing in file {path}!");
                if (splittedTestCase[i].Contains("TestName:")) logFileData[2] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                else throw new Exception($"Test name field is missing in file {path}!");
                if (splittedTestCase[i].Contains("TestType:"))
                    logFileData[3] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                if (splittedTestCase[i].Contains("Result:"))
                {
                    string bufor = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim().ToLower();
                    logFileData[4] = string.Concat(bufor[0].ToString().ToUpper(), bufor.AsSpan(1));
                }
                else throw new Exception($"Result field is missing in file {path}!");
                if (splittedTestCase[i].Contains("Value:"))
                    logFileData[5] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                if (splittedTestCase[i].Contains("Units:"))
                    logFileData[6] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                if (splittedTestCase[i].Contains("LowerLimit:"))
                    logFileData[7] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                if (splittedTestCase[i].Contains("UpperLimit:"))
                    logFileData[8] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();

                if (i == splittedTestCase.Length - 1)
                {
                    DateTime dt = ConvertDateAndTime(logFileData[0..2]);

                    var testStep = new TestStep(logFileData[2], dt, logFileData[4], logFileData[3], logFileData[5], logFileData[6], logFileData[7], logFileData[8]);
                    if (testStep.TestName != null && testStep.TestStatus != null)
                        testSteps.Add(testStep);
                }
            }
        }
        return testSteps;
    }
    private static DateTime ConvertDateAndTime(string[] dt)
    {
        if (dt[0]?.Length == 10 && dt[1]?.Length == 8)
        {
            // If date is first...
            var year = Int32.Parse(dt[0].Substring(6, 4));
            var month = Int32.Parse(dt[0][..2]);
            var day = Int32.Parse(dt[0].Substring(3, 2));
            var hour = Int32.Parse(dt[1][..2]);
            var minute = Int32.Parse(dt[1].Substring(3, 2));
            var second = Int32.Parse(dt[1].Substring(6, 2));

            var Converted = new DateTime(year, month, day, hour, minute, second);
            return Converted;
        }
        else if (dt[0]?.Length == 8 && dt[1]?.Length == 10)
        {
            // If time is first...
            var year = Int32.Parse(dt[1].Substring(6, 4));
            var month = Int32.Parse(dt[1][..2]);
            var day = Int32.Parse(dt[1].Substring(3, 2));
            var hour = Int32.Parse(dt[0][..2]);
            var minute = Int32.Parse(dt[0].Substring(3, 2));
            var second = Int32.Parse(dt[0].Substring(6, 2));

            var Converted = new DateTime(year, month, day, hour, minute, second);
            return Converted;
        }
        return new DateTime(0);
    }
    private TestStatus GetStatus()
    {
        int passedTests = 0;
        if (TestSteps == null) return TestStatus.Failed;
        foreach (TestStepBase testStep in TestSteps)
        {
            if (testStep.TestStatus.Contains("pass", StringComparison.OrdinalIgnoreCase))
                passedTests++;
        }
        if (passedTests == TestSteps.Count() && passedTests != 0)
            return TestStatus.Passed;

        if (TestSteps.Count() == 0)
            throw new Exception("Log file has no test steps!");

        return TestStatus.Failed;
    }
    private DateTime GetTestDateAndTime()
    {
        try
        {
            var min = TestSteps.First().TestDateTimeFinish;
            foreach (var testStep in this.TestSteps!)
            {
                if (testStep.TestDateTimeFinish < min)
                    min = testStep.TestDateTimeFinish;
            }
            return min;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
    private string GetFailedStepData()
    {
        var failDetails = "";
        foreach (var test in TestSteps)
        {
            if (test.TestStatus.Contains("fail", StringComparison.OrdinalIgnoreCase))
                failDetails = $"{test.TestName}\nValue measured: {test.TestValue}\nLower limit: {test.TestLowerLimit}\nUpper limit: {test.TestUpperLimit}";
        }
        return failDetails;
    }
    private static Workstation GetStationName(IEnumerable<string> text)
    {
        var workstation = string.Empty;
        foreach (string line in text)
        {
            if (line.Contains("Operator:"))
            {
                string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                workstation = SplittedLine[1];
                break;
            }
        }
        if (workstation == string.Empty) throw new Exception("Operator field is missing!");
        else return new Workstation(workstation);
    }
    private TimeSpan? GetBoardTestingTime()
    {
        if (TestSteps == null) return null;
        var minTime = TestSteps.Min(x => x.TestDateTimeFinish);
        var maxTime = TestSteps.Max(x => x.TestDateTimeFinish);
        try
        {
            return maxTime - minTime;
        }
        catch
        {
            return null;
        }
    }
    private static string GetTestSocket(IEnumerable<string> text)
    {
        bool dataAhead = false;

        foreach (string line in text)
        {
            if (line.Contains("Test Socket Number"))
                dataAhead = true;
            if (dataAhead)
            {
                if (line.Contains("Value"))
                {
                    string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                    return SplittedLine[1];
                }
            }
        }
        return "";
    }

    //private static string GetTestProgramFilePath(IEnumerable<string> text)
    //{
    //    foreach (string line in text)
    //    {
    //        // Look for specific field in log file
    //        if (line.Contains("TestProgram:"))
    //        {
    //            // Split string into 2 substrings basing on tab separator and return second substring
    //            string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
    //            return SplittedLine[1];
    //        }
    //    }
    //    return String.Empty;
}