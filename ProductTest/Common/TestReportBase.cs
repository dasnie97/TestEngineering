namespace ProductTest.Common;

public abstract class TestReportBase
{
    public string SerialNumber { get; protected set; }
    public TestStatus Status { get; protected set; }
    public Workstation Workstation { get; protected set; }
    public IEnumerable<TestStepBase> TestSteps { get; protected set; }
    public DateTime TestDateTimeStarted { get; protected set; }
    public string Failure { get; protected set; }
    public string? FixtureSocket { get; protected set; }
    public TimeSpan? TestingTime { get; protected set; }
    //public bool IsFirstPass { get; set; }
    //public bool FalseCall { get; set; }

    protected TestReportBase(string path)
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
    protected TestReportBase(
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
    /// Get serial number of tested board. This method looks for 'ImageBarcode' field in txt file and returns its value.
    /// </summary>
    /// <returns>Value of 'ImageBarcode' field in log file.</returns>
    protected virtual string GetSerialNumber(IEnumerable<string> text)
    {
        var serialNumber = string.Empty;
        foreach (string line in text)
        {
            // Look for 'PanelBarcode' field
            if (line.Contains("PanelBarcode:") || line.Contains("ImageBarcode:"))
            {
                // Split string into 2 substrings basing on tab separator and return second substring
                string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                serialNumber = SplittedLine[1].Trim();
                break;
            }
        }
        if (serialNumber == string.Empty) throw new Exception("Serial number is missing!");
        else return serialNumber;
    }

    /// <summary>
    /// Get list of test steps as a list of TestStep objects
    /// </summary>
    /// <returns>List of TestStep objects</returns>
    protected virtual List<TestStep> GetTestSteps(string path)
    {
        var testSteps = new List<TestStep>();
        string logFileText = File.ReadAllText(path);

        // Group text into array basing on test step termination character
        string[] splittedText = logFileText.Split("~#~");

        // Loop over every test case
        foreach (string testCase in splittedText)
        {
            string[] logFileData = new string[9];
            string[] splittedTestCase = testCase.Split("\n");

            // For every line of test step section
            for (int i = 0; i < splittedTestCase.Length; i++)
            {
                // For every new test fill testStep object with data. Split lines of file basing on 'tab' character and get values of fields into testStep object.
                // For date and time, gather data from log file to process it in next step and finally add it to testStep object.
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

                // End of test step section
                if (i == splittedTestCase.Length - 1)
                {
                    // Create DateTime object basing on string read from logfile
                    DateTime dt = ConvertDateAndTime(logFileData[0..2]);

                    // For each test step create new testStep object with data read from log file
                    var testStep = new TestStep(logFileData[2], dt, logFileData[4], logFileData[3], logFileData[5], logFileData[6], logFileData[7], logFileData[8]);
                    // Add TestStep object to testSteps list if it has some valid information
                    if (testStep.TestName != null && testStep.TestStatus != null)
                        testSteps.Add(testStep);
                }
            }
        }
        return testSteps;
    }

    /// <summary>
    /// This method converts string array of date and time to DateTime object.
    /// </summary>
    /// <param name="dt">String array of date and time.</param>
    /// <returns>DateTime object representation of string date and time.</returns>
    private static DateTime ConvertDateAndTime(string[] dt)
    {
        //Determine if first element of array is date or time and convert it to DateTime object
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

    /// <summary>
    /// Get test status of the board from txt file. Loop over every test step and check if every test has passed status.
    /// </summary>
    /// <returns>Test status of the board.</returns>
    protected virtual TestStatus GetStatus()
    {
        int passedTests = 0;
        if (TestSteps == null) return TestStatus.Failed;
        foreach (TestStepBase testStep in TestSteps)
        {
            // If line contains specific "Pass" somwhere in test result string...
            if (testStep.TestStatus.Contains("pass", StringComparison.OrdinalIgnoreCase))
                passedTests++;
        }
        if (passedTests == TestSteps.Count() && passedTests != 0)
            return TestStatus.Passed;

        if (TestSteps.Count() == 0)
            throw new Exception("Log file has no test steps!");

        return TestStatus.Failed;
    }

    /// <summary>
    /// Retruns DateTime object of least value from all of test steps which represents beginning of test.
    /// </summary>
    /// <returns>DateTime object of least value from all of test steps.</returns>
    protected virtual DateTime GetTestDateAndTime()
    {
        try
        {
            var min = TestSteps!.First().TestDateTimeFinish;
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

    /// <summary>
    /// Returns measured value, test limits and name of test with failes status in form of one string.
    /// </summary>
    /// <returns>String with failure details.</returns>
    protected virtual string GetFailedStepData()
    {
        var failDetails = "";
        foreach (var test in this.TestSteps!)
        {
            // If line contains specific "Fail" somwhere in test result string...
            if (test.TestStatus.Contains("fail", StringComparison.OrdinalIgnoreCase))
                failDetails = $"{test.TestName}\nValue measured: {test.TestValue}\nLower limit: {test.TestLowerLimit}\nUpper limit: {test.TestUpperLimit}";
        }
        return failDetails;
    }

    /// <summary>
    /// Gets value of 'Operator' field in log file.
    /// </summary>
    /// <returns>Value of 'Operator' field.</returns>
    protected virtual Workstation GetStationName(IEnumerable<string> text)
    {
        var workstation = string.Empty;
        foreach (string line in text)
        {
            // Look for specific field in log file
            if (line.Contains("Operator:"))
            {
                // Split string into 2 substrings basing on tab separator and return second substring
                string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                workstation = SplittedLine[1];
                break;
            }
        }
        if (workstation == string.Empty) throw new Exception("Operator field is missing!");
        else return new Workstation(workstation);
    }

    /// <summary>
    /// Gets testing time of the board being difference between first and last test step date and time.
    /// </summary>
    /// <returns>Testing time.</returns>
    protected virtual TimeSpan? GetBoardTestingTime()
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

    /// <summary>
    /// Gets number of fixture socket which board was tested on.
    /// </summary>
    protected virtual string GetTestSocket(IEnumerable<string> text)
    {
        // This flag is set when current line read is test socket test
        bool dataAhead = false;

        foreach (string line in text)
        {
            // Look for specific field in log file
            if (line.Contains("Test Socket Number"))
                dataAhead = true;

            // If searched data is found
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
    //}

}