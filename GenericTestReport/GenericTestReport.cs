using GenericTestReport.Interfaces;

namespace GenericTestReport
{
    public class LogFile : ILogFile<TestStep>
    {
        #region Public properties
        public string Workstation { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsFirstPass { get; set; }
        public bool FalseCall { get; set; }
        public string? FixtureSocket { get; set; }
        public string Failure { get; set; } = string.Empty;
        public string? TestProgramFilePath { get; set; }
        public List<TestStep>? TestSteps { get; set; }
        public TimeSpan? TestingTime { get; set; }
        public DateTime TestDateTimeStarted { get; set; }


        #endregion

        #region Constructor

        /// <summary>
        /// Create LogFile instance using given path.
        /// </summary>
        /// <param name="path">Path to standard txt log file.</param>
        public LogFile(string path)
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
                TestProgramFilePath = GetTestProgramFilePath(text);
            }
        }

        public LogFile()
        {

        }
        #endregion

        #region Private methods

        /// <summary>
        /// Get serial number of tested board. This method looks for 'ImageBarcode' field in txt file and returns its value.
        /// </summary>
        /// <returns>Value of 'ImageBarcode' field in log file.</returns>
        private static string GetSerialNumber(IEnumerable<string> text)
        {
            foreach (string line in text)
            {
                // Look for 'PanelBarcode' field
                if (line.Contains("PanelBarcode:"))
                {
                    // Split string into 2 substrings basing on tab separator and return second substring
                    string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                    return SplittedLine[1].Trim();
                }

                //If there is no 'PanelBarcode' field - look for 'ImageBarcode' field
                if (line.Contains("ImageBarcode:"))
                {
                    string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                    return SplittedLine[1].Trim();
                }
            }
            throw new Exception("Serial number is missing!");
        }

        /// <summary>
        /// Get list of test steps as a list of TestStep objects
        /// </summary>
        /// <returns>List of TestStep objects</returns>
        private static List<TestStep> GetTestSteps(string path)
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
                    if (splittedTestCase[i].Contains("Date:"))
                        logFileData[0] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                    if (splittedTestCase[i].Contains("Time:"))
                        logFileData[1] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                    if (splittedTestCase[i].Contains("TestName:"))
                        logFileData[2] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                    if (splittedTestCase[i].Contains("TestType:"))
                        logFileData[3] = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim();
                    if (splittedTestCase[i].Contains("Result:"))
                    {
                        string bufor = splittedTestCase[i].Split("\t", StringSplitOptions.None)[1].Trim().ToLower();
                        logFileData[4] = string.Concat(bufor[0].ToString().ToUpper(), bufor.AsSpan(1));
                    }
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
        /// Get test status of the board from txt file. Loop over every test step and check if every test has passed status.
        /// </summary>
        /// <returns>Test status of the board.</returns>
        private string GetStatus()
        {
            int passedTests = 0;
            if (TestSteps == null) return "Failed";
            foreach (ITestStep testStep in TestSteps)
            {
                // If line contains specific "Pass" somwhere in test result string...
                if (testStep.TestStatus.Contains("pass", StringComparison.OrdinalIgnoreCase))
                    passedTests++;
            }
            if (passedTests == this.TestSteps.Count && passedTests != 0)
                return "Passed";

            if (this.TestSteps.Count == 0)
                throw new Exception("Log file has no test steps!");

            return "Failed";
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
        /// Retruns DateTime object of least value from all of test steps which represents beginning of test.
        /// </summary>
        /// <returns>DateTime object of least value from all of test steps.</returns>
        private DateTime GetTestDateAndTime()
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
        private string GetFailedStepData()
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
        private static string GetStationName(IEnumerable<string> text)
        {
            foreach (string line in text)
            {
                // Look for specific field in log file
                if (line.Contains("Operator:"))
                {
                    // Split string into 2 substrings basing on tab separator and return second substring
                    string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                    return SplittedLine[1];
                }
            }
            throw new Exception("Operator field is missing!");
        }

        /// <summary>
        /// Gets testing time of the board being difference between first and last test step date and time.
        /// </summary>
        /// <returns>Testing time.</returns>
        private TimeSpan? GetBoardTestingTime()
        {
            if (TestSteps == null) return null;
            var minTime = TestSteps.Min(x => x.TestDateTimeFinish);
            var maxTime = TestSteps.Max(x=> x.TestDateTimeFinish);

            try
            {
                return maxTime-minTime;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets number of fixture socket which board was tested on.
        /// </summary>
        private static string GetTestSocket(IEnumerable<string> text)
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

        private static string GetTestProgramFilePath(IEnumerable<string> text)
        {
            foreach (string line in text)
            {
                // Look for specific field in log file
                if (line.Contains("TestProgram:"))
                {
                    // Split string into 2 substrings basing on tab separator and return second substring
                    string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                    return SplittedLine[1];
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Checks if all general data necessary for log file creation is present.
        /// </summary>
        /// <returns>True if data is ok.</returns>
        private bool CheckIfGeneralDataExists()
        {
            var allDataExists = true;

            if (String.IsNullOrEmpty(SerialNumber)) allDataExists = false;
            if (String.IsNullOrEmpty(Workstation)) allDataExists = false;
            if (this.TestSteps?.Count == 0 || this.TestSteps == null) allDataExists = false;

            return allDataExists;
        }

        /// <summary>
        /// Checks if all test step data necessary for log file creation is present.
        /// </summary>
        /// <param name="ts">TestStep object.</param>
        /// <returns>True if data is ok.</returns>
        private static bool CheckIfStepDataExists(ITestStep ts)
        {
            var allDataExists = true;

            if (String.IsNullOrEmpty(ts.TestName)) allDataExists = false;
            if (ts.TestDateTimeFinish == new DateTime(0)) allDataExists = false;
            if (String.IsNullOrEmpty(ts.TestStatus)) allDataExists = false;

            return allDataExists;
        }

        #endregion

        #region Public methods
        public string SaveLogFile(string path)
        {
            if (!CheckIfGeneralDataExists()) throw new Exception("Check if general data is ok!");

            var logFileName = $"{TestDateTimeStarted.Month:00}{TestDateTimeStarted.Day:00}{TestDateTimeStarted.Year}_{TestDateTimeStarted.Hour:00}{TestDateTimeStarted.Minute:00}{TestDateTimeStarted.Second:00}_{SerialNumber}.txt";
            var buffor = new List<string>
            {
                $"PanelBarcode:\t{SerialNumber}",
                $"TestProgram:\t{TestProgramFilePath}",
                $"TestProgramVer:\t{"1.0"}",
                $"Operator:\t{Workstation}",
                $"ImageBarcode:\t{SerialNumber}",
                $""
            };

            foreach (var testStep in this.TestSteps!)
            {
                if (!CheckIfStepDataExists(testStep)) throw new Exception("Check if test step data is ok!");

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

            var logFilePath = Path.Combine(path, logFileName);

            File.WriteAllLines(logFilePath, buffor);
            //File.AppendAllLines(logFilePath, buffor);

            return logFilePath;
        }
        #endregion
    }

    public class TestStep : ITestStep
    {
        #region Properties

        public string TestName { get; private set; } = string.Empty;
        public string TestType { get; private set; } = string.Empty;
        public DateTime TestDateTimeFinish { get; private set; }
        public string TestStatus { get; private set; } = string.Empty;
        public string TestValue { get; private set; } = string.Empty;
        public string ValueUnit { get; private set; } = string.Empty;
        public string TestLowerLimit { get; private set; } = string.Empty;
        public string TestUpperLimit { get; private set; } = string.Empty;
        public bool IsNumeric { get; private set; }
        public string Failure { get; private set; } = string.Empty;

        #endregion

        #region Constructor

        public TestStep(string testName, DateTime testDateTime, string testStatus, string testType = "", string testValue = "", string valueUnit = "", string testLL = "", string testUL = "", string failure = "")
        {
            // Assign parameters to properties of class
            this.TestName = testName;
            this.TestType = testType;
            this.TestDateTimeFinish = testDateTime;
            this.TestStatus = testStatus;
            this.TestValue = testValue;
            this.ValueUnit = valueUnit;
            this.TestLowerLimit = testLL;
            this.TestUpperLimit = testUL;
            this.Failure = failure;

            // Evaluate test type
            this.IsNumeric = EvaluateTestType();
        }

        public TestStep()
        {

        }

        #endregion

        #region Private methods

        /// <summary>
        /// Check if test step is of numeric type.
        /// </summary>
        /// <returns>True if test step is numeric and can be used for MSA. Returns false if test step is of pass/fail or string type.</returns>
        private bool EvaluateTestType()
        {
            return float.TryParse(this.TestValue, out _) && (this.TestLowerLimit != this.TestUpperLimit);
        }

        #endregion

        #region Public methods

        public void SetTestName(string testName){this.TestName = testName;}
        public void SetTestType(string testType){this.TestType = testType;}
        public void SetTestDateTime(DateTime testDT){this.TestDateTimeFinish = testDT;}
        public void SetTestStatus(string testStatus){this.TestStatus = testStatus;}
        public void SetTestValue(string testValue){this.TestValue = testValue;}
        public void SetValueUnit(string valueUnit){this.ValueUnit = valueUnit;}
        public void SetTestLowLimit(string LL){this.TestLowerLimit = LL;}
        public void SetTestHighLimit(string UL){this.TestUpperLimit = UL;}
        public void SetFailure(string fail){this.Failure = fail;}
        
        #endregion
    }
}
