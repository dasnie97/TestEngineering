namespace GenericTestReport
{
    /// <summary>
    /// Provides API essential for handling standard log files data flow on production floor. It is the base for data control and production supervision.
    /// </summary>
    public class LogFile
    {
        #region Public properties

        /// <summary>
        /// Path to standard txt log file.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// List of test steps in standard txt log file.
        /// </summary>
        public List<TestStep>? TestSteps { get; private set; }

        /// <summary>
        /// Test date and time in DateTime format.
        /// </summary>
        public DateTime TestDateAndTime { get; private set; }

        /// <summary>
        /// Board serial number.
        /// </summary>
        public string? BoardSerialNumber { get; private set; }

        /// <summary>
        /// Number of fixture socket on which board was tested.
        /// </summary>
        public string? TestSocketNumber { get; private set; }

        /// <summary>
        /// Test status of the board.
        /// </summary>
        public string? BoardStatus { get; private set; }

        /// <summary>
        /// If test status is failed, represents name of the failed step.
        /// </summary>
        public string? FailedStep { get; private set; }

        /// <summary>
        /// Test date and time of string format.
        /// </summary>
        public string? DateAndTimeOfTest { get; private set; }

        /// <summary>
        /// Test station on which standard txt log file was created.
        /// </summary>
        public string? Station { get; private set; }

        /// <summary>
        /// Number of lines of standard txt log file.
        /// </summary>
        public int NumberOfLines { get; private set; }

        /// <summary>
        /// Board testing time. Time difference between first and last test step.
        /// </summary>
        public TimeSpan? TestingTime { get; private set; }

        /// <summary>
        /// Path to test program application file.
        /// </summary>
        public string? TestProgramFilePath { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Create LogFile instance using given path.
        /// </summary>
        /// <param name="path">Path to standard txt log file.</param>
        public LogFile(string path)
        {
            this.Path = string.Empty;

            if (Directory.Exists(path))
                this.Path = path;

            if (File.Exists(path))
            {
                this.Path = path;
                this.BoardSerialNumber = GetSerialNumber();
                this.TestSteps = GetTestSteps();
                this.BoardStatus = GetStatus();
                this.TestDateAndTime = GetTestDateAndTime();
                this.FailedStep = GetFailedStepData();
                this.DateAndTimeOfTest = GetDateAndTimeString();
                this.Station = GetStationName();
                this.NumberOfLines = GetNumberOfLines();
                this.TestingTime = GetBoardTestingTime();
                this.TestSocketNumber = GetTestSocket();
            }
        }

        /// <summary>
        /// Create empty LogFile object.
        /// </summary>
        public LogFile()
        {
            this.Path = String.Empty;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Get serial number of tested board. This method looks for 'ImageBarcode' field in txt file and returns its value.
        /// </summary>
        /// <returns>Value of 'ImageBarcode' field in log file.</returns>
        private string? GetSerialNumber()
        {
            foreach (string line in File.ReadLines(this.Path))
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
            // If there is no specific string present in file return null
            return null;
        }

        /// <summary>
        /// Get list of test steps as a list of TestStep objects
        /// </summary>
        /// <returns>List of TestStep objects</returns>
        private List<TestStep> GetTestSteps()
        {
            var testSteps = new List<TestStep>();
            string logFileText = File.ReadAllText(this.Path);

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
                        var testStep = new TestStep(logFileData[2], logFileData[3], dt, logFileData[4], logFileData[5], logFileData[6], logFileData[7], logFileData[8]);

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
        private string? GetStatus()
        {
            int passedTests = 0;
            foreach (TestStep testStep in this.TestSteps)
            {
                // If line contains specific "Pass" somwhere in test result string...
                if (testStep.TestStatus.Contains("pass", StringComparison.OrdinalIgnoreCase))
                    passedTests++;
            }
            if (passedTests == this.TestSteps.Count && passedTests != 0)
                return "Passed";

            if (passedTests == 0)
                return null;

            return "Failed";
        }

        /// <summary>
        /// This method converts string array of date and time to DateTime object.
        /// </summary>
        /// <param name="dt">String array of date and time.</param>
        /// <returns>DateTime object representation of string date and time.</returns>
        private DateTime ConvertDateAndTime(string[] dt)
        {
            //Determine if first element of array is date or time and convert it to DateTime object
            if (dt[0]?.Length == 10 && dt[1]?.Length == 8)
            {
                // If date is first...
                var year = Int32.Parse(dt[0].Substring(6, 4));
                var month = Int32.Parse(dt[0].Substring(0, 2));
                var day = Int32.Parse(dt[0].Substring(3, 2));
                var hour = Int32.Parse(dt[1].Substring(0, 2));
                var minute = Int32.Parse(dt[1].Substring(3, 2));
                var second = Int32.Parse(dt[1].Substring(6, 2));

                var Converted = new DateTime(year, month, day, hour, minute, second);
                return Converted;
            }
            else if (dt[0]?.Length == 8 && dt[1]?.Length == 10)
            {
                // If time is first...
                var year = Int32.Parse(dt[1].Substring(6, 4));
                var month = Int32.Parse(dt[1].Substring(0, 2));
                var day = Int32.Parse(dt[1].Substring(3, 2));
                var hour = Int32.Parse(dt[0].Substring(0, 2));
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
                var min = TestSteps.First().TestDateTime;
                foreach (var testStep in this.TestSteps)
                {
                    if (testStep.TestDateTime < min)
                        min = testStep.TestDateTime;
                }
                return min;
            }
            catch
            {
                return DateTime.MinValue;
            }

        }

        /// <summary>
        /// Converts TestDateAndTime property of log file to string.
        /// </summary>
        /// <returns>String representation of TestDateAndTime property.</returns>
        private string GetDateAndTimeString()
        {
            return this.TestDateAndTime.ToString();
        }

        /// <summary>
        /// Returns measured value, test limits and name of test with failes status in form of one string.
        /// </summary>
        /// <returns>String with failure details.</returns>
        private string GetFailedStepData()
        {
            var failDetails = "";
            foreach (var test in this.TestSteps)
            {
                // If line contains specific "Fail" somwhere in test result string...
                if (test.TestStatus.Contains("fail", StringComparison.OrdinalIgnoreCase))
                    failDetails = $"{test.TestName}\nValue measured: {test.TestValue}\nLower limit: {test.TestLL}\nUpper limit: {test.TestUL}";
            }
            return failDetails;
        }

        /// <summary>
        /// Gets value of 'Operator' field in log file.
        /// </summary>
        /// <returns>Value of 'Operator' field.</returns>
        private string? GetStationName()
        {
            foreach (string line in File.ReadLines(this.Path))
            {
                // Look for specific field in log file
                if (line.Contains("Operator:"))
                {
                    // Split string into 2 substrings basing on tab separator and return second substring
                    string[] SplittedLine = line.Split("\t", StringSplitOptions.None);
                    return SplittedLine[1];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets number of lines in file.
        /// </summary>
        /// <returns>Number of lines in file.</returns>
        private int GetNumberOfLines()
        {
            return File.ReadLines(this.Path).Count();
        }

        /// <summary>
        /// Gets testing time of the board being difference between first and last test step date and time.
        /// </summary>
        /// <returns>Testing time.</returns>
        private TimeSpan? GetBoardTestingTime()
        {
            try
            {
                return this.TestSteps.Last().TestDateTime - this.TestDateAndTime;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets number of fixture socket which board was tested on.
        /// </summary>
        private string GetTestSocket()
        {
            // This flag is set when current line read is test socket test
            bool dataAhead = false;

            foreach (string line in File.ReadLines(this.Path))
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

        /// <summary>
        /// Checks if all general data necessary for log file creation is present.
        /// </summary>
        /// <returns>True if data is ok.</returns>
        private bool checkIfGeneralDataExists()
        {
            var allDataExists = true;

            if (String.IsNullOrEmpty(this.BoardSerialNumber)) allDataExists = false;
            if (String.IsNullOrEmpty(this.TestProgramFilePath)) allDataExists = false;
            if (String.IsNullOrEmpty(this.Station)) allDataExists = false;
            if (this.TestSteps.Count == 0 || this.TestSteps == null) allDataExists = false;

            return allDataExists;
        }

        /// <summary>
        /// Checks if all test step data necessary for log file creation is present.
        /// </summary>
        /// <param name="ts">TestStep object.</param>
        /// <returns>True if data is ok.</returns>
        private bool checkIfStepDataExists(TestStep ts)
        {
            var allDataExists = true;

            if (String.IsNullOrEmpty(ts.TestName)) allDataExists = false;
            if (ts.TestDateTime == new DateTime(0)) allDataExists = false;
            if (String.IsNullOrEmpty(ts.TestStatus)) allDataExists = false;

            return allDataExists;
        }

        #endregion

        #region Public methods

        public void SetTestSteps(List<TestStep> TS){this.TestSteps = TS;}
        public void SetTestDateAndTime(DateTime dt){this.TestDateAndTime = dt;}
        public void SetBoardSerialNumber(string SN){this.BoardSerialNumber = SN;}
        public void SetTestSocket(string sock){this.TestSocketNumber = sock;}
        public void SetBoardStatus(string status){this.BoardStatus = status;}
        public void SetFailureCause(string failure){this.FailedStep = failure;}
        public void SetStationName(string station){this.Station = station;}
        public void SetTestProgramPath(string path) { this.TestProgramFilePath = path; }

        /// <summary>
        /// Saves standard log file.
        /// </summary>
        /// <exception cref="Exception">Throws exception if some critical data is null or empty.</exception>
        public string SaveLogFile()
        {
            if (!checkIfGeneralDataExists()) throw new Exception("Check if general data is ok!");

            var logFileName = $"{this.TestDateAndTime.Month.ToString("00")}{this.TestDateAndTime.Day.ToString("00")}{this.TestDateAndTime.Year}_{this.TestDateAndTime.Hour.ToString("00")}{this.TestDateAndTime.Minute.ToString("00")}{this.TestDateAndTime.Second.ToString("00")}_{this.BoardSerialNumber}.txt";
            var buffor = new List<string>();

            buffor.Add($"PanelBarcode:\t{this.BoardSerialNumber}");
            buffor.Add($"TestProgram:\t{this.TestProgramFilePath}");
            buffor.Add($"TestProgramVer:\t{"1.0"}");
            buffor.Add($"Operator:\t{this.Station}");
            buffor.Add($"ImageBarcode:\t{this.BoardSerialNumber}");
            buffor.Add($"");

            foreach (var testStep in this.TestSteps)
            {
                if (!checkIfStepDataExists(testStep)) throw new Exception("Check if test step data is ok!");

                buffor.Add($"TestName:\t{testStep.TestName}");
                if (testStep.TestType != "") 
                    buffor.Add($"TestType:\t{testStep.TestType}");
                buffor.Add($"Date:\t{testStep.TestDateTime.Month.ToString("00")}/{testStep.TestDateTime.Day.ToString("00")}/{testStep.TestDateTime.Year}");
                buffor.Add($"Time:\t{testStep.TestDateTime.Hour.ToString("00")}:{testStep.TestDateTime.Minute.ToString("00")}:{testStep.TestDateTime.Second.ToString("00")}");
                buffor.Add($"Result:\t{testStep.TestStatus}");
                if (testStep.TestValue != "")
                    buffor.Add($"Value:\t{testStep.TestValue}");
                if (testStep.ValueUnit != "")
                    buffor.Add($"Units:\t{testStep.ValueUnit}");
                if (testStep.TestLL != "")
                    buffor.Add($"LowerLimit:\t{testStep.TestLL}");
                if (testStep.TestUL != "")
                    buffor.Add($"UpperLimit:\t{testStep.TestUL}");
                if (testStep.failure != "")
                    buffor.Add($"FailDesc:\t{testStep.failure}");
                buffor.Add("~#~");
            }

            var logFilePath = System.IO.Path.Combine(this.Path, logFileName);

            File.AppendAllLines(logFilePath, buffor);

            return logFilePath;
        }

        #endregion
    }

    /// <summary>
    /// Describes single test step. E.g.: test status, lower limit, value.
    /// </summary>
    public class TestStep
    {
        #region Properties

        public string TestName { get; private set; } = string.Empty;
        public string TestType { get; private set; } = string.Empty;
        public DateTime TestDateTime { get; private set; }
        public string TestStatus { get; private set; } = string.Empty;
        public string TestValue { get; private set; } = string.Empty;
        public string ValueUnit { get; private set; } = string.Empty;
        public string TestLL { get; private set; } = string.Empty;
        public string TestUL { get; private set; } = string.Empty;
        public bool isNumeric { get; private set; }
        public string failure { get; private set; } = string.Empty;

        #endregion

        #region Constructor

        public TestStep(string testName, string testType, DateTime testDateTime, string testStatus, string testValue, string valueUnit, string testLL, string testUL)
        {
            // Assign parameters to properties of class
            this.TestName = testName;
            this.TestType = testType;
            this.TestDateTime = testDateTime;
            this.TestStatus = testStatus;
            this.TestValue = testValue;
            this.ValueUnit = valueUnit;
            this.TestLL = testLL;
            this.TestUL = testUL;

            // Evaluate test type
            this.isNumeric = EvaluateTestType();
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
            // Try to convert TestValue to float and check if limits are different.
            // If conversion succeed and limits are different - return true. If conversion fails or limits are the same - return false.
            return float.TryParse(this.TestValue, out _) && (this.TestLL != this.TestUL);
        }

        #endregion

        #region Public methods

        public void SetTestName(string testName){this.TestName = testName;}
        public void SetTestType(string testType){this.TestType = testType;}
        public void SetTestDateTime(DateTime testDT){this.TestDateTime = testDT;}
        public void SetTestStatus(string testStatus){this.TestStatus = testStatus;}
        public void SetTestValue(string testValue){this.TestValue = testValue;}
        public void SetValueUnit(string valueUnit){this.ValueUnit = valueUnit;}
        public void SetTestLowLimit(string LL){this.TestLL = LL;}
        public void SetTestHighLimit(string UL){this.TestUL = UL;}
        public void SetFailure(string fail){this.failure = fail;}

        #endregion
    }
}
