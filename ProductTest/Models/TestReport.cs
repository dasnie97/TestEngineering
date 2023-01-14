﻿using ProductTest.Common;
using ProductTest.Interfaces;

namespace ProductTest.Models;

public class TestReport : TestReportBase, ITestReport
{
    public static TestReport CreateFromFile(string path)
    {
        if (File.Exists(path))
        {
            return new TestReport(path);
        }
        else
        {
            throw new FileNotFoundException("File not found", path);
        }
    }

    private TestReport(string path) : base()
    {
        var linesOfText = File.ReadAllLines(path);
        SetSerialNumber(linesOfText);
        SetTestSteps(path);
        SetStatus();
        SetTestDateAndTime();
        SetFailedStepData();
        SetStationName(linesOfText);
        SetBoardTestingTime();
        SetTestSocket();
    }

    public static TestReport Create(string serialNumber,
                                    string status,
                                    string workstation,
                                    DateTime testStarted,
                                    List<TestStepBase> testSteps)
    {
        return new TestReport(serialNumber, status, workstation, testStarted, testSteps);
    }

    private TestReport(string serialNumber,
                        string status,
                        string workstation,
                        DateTime testStarted,
                        List<TestStepBase> testSteps
    ) : base(serialNumber, status, workstation, testStarted, testSteps)
    { }

    public FileInfo SaveReport(string directoryPath)
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
            buffor.Add($"TestName:\t{testStep.Name}");
            if (!string.IsNullOrEmpty(testStep.Type))
                buffor.Add($"TestType:\t{testStep.Type}");
            buffor.Add($"Date:\t{testStep.DateTimeFinish.Month:00}/{testStep.DateTimeFinish.Day:00}/{testStep.DateTimeFinish.Year}");
            buffor.Add($"Time:\t{testStep.DateTimeFinish.Hour:00}:{testStep.DateTimeFinish.Minute:00}:{testStep.DateTimeFinish.Second:00}");
            buffor.Add($"Result:\t{testStep.Status}");
            if (!string.IsNullOrEmpty(testStep.Value))
                buffor.Add($"Value:\t{testStep.Value}");
            if (!string.IsNullOrEmpty(testStep.Unit))
                buffor.Add($"Units:\t{testStep.Unit}");
            if (!string.IsNullOrEmpty(testStep.LowerLimit))
                buffor.Add($"LowerLimit:\t{testStep.LowerLimit}");
            if (!string.IsNullOrEmpty(testStep.UpperLimit))
                buffor.Add($"UpperLimit:\t{testStep.UpperLimit}");
            if (!string.IsNullOrEmpty(testStep.Failure))
                buffor.Add($"FailDesc:\t{testStep.Failure}");
            buffor.Add("~#~");
        }

        var logFilePath = Path.Combine(directoryPath, logFileName);
        File.WriteAllLines(logFilePath, buffor);
        return new FileInfo(logFilePath);
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
    private void SetTestSteps(string path)
    {
        var testSteps = new List<TestStep>();
        string logFileText = File.ReadAllText(path);
        string[] splittedText = logFileText.Split("~#~");
        foreach (string testCase in splittedText)
        {
            string name = string.Empty;
            string date = string.Empty;
            string time = string.Empty;
            string status = string.Empty;
            string type = string.Empty;
            string value = string.Empty;
            string unit = string.Empty;
            string lowerlimit = string.Empty;
            string upperlimit = string.Empty;

            string[] line = testCase.Split("\n");
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Contains("Date:")) date = GetFieldValue(line[i]);
                if (line[i].Contains("Time:")) time = GetFieldValue(line[i]);
                if (line[i].Contains("TestName:")) name = GetFieldValue(line[i]);
                if (line[i].Contains("TestType:")) type = GetFieldValue(line[i]);
                if (line[i].Contains("Result:"))
                {
                    string bufor = GetFieldValue(line[i]).ToLower();
                    status = string.Concat(bufor[0].ToString().ToUpper(), bufor.AsSpan(1));
                }
                if (line[i].Contains("Value:")) value = GetFieldValue(line[i]);
                if (line[i].Contains("Units:")) unit = GetFieldValue(line[i]);
                if (line[i].Contains("LowerLimit:")) lowerlimit = GetFieldValue(line[i]);
                if (line[i].Contains("UpperLimit:")) upperlimit = GetFieldValue(line[i]);

                if (i == line.Length - 1)
                {
                    DateTime datetime = ConvertDateAndTime(date, time, testCase);

                    var testStep = TestStep.Create(name, datetime, status, type, value, unit, lowerlimit, upperlimit);
                    if (testStep.Name != string.Empty && testStep.Status != string.Empty)
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
    private static DateTime ConvertDateAndTime(string date, string time, string cc)
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
    private void SetStatus()
    {
        int passedTests = 0;
        foreach (TestStepBase testStep in TestSteps)
        {
            if (testStep.Status.Contains("pass", StringComparison.OrdinalIgnoreCase))
                passedTests++;
        }
        if (passedTests == TestSteps.Count() && passedTests != 0)
            Status = TestStatus.Passed;
        else
            Status = TestStatus.Failed;
    }
    private void SetTestDateAndTime()
    {
        try
        {
            var min = TestSteps.First().DateTimeFinish;
            foreach (var testStep in TestSteps!)
            {
                if (testStep.DateTimeFinish < min)
                    min = testStep.DateTimeFinish;
            }
            TestDateTimeStarted = min;
        }
        catch
        {
            TestDateTimeStarted = DateTime.MinValue;
        }
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
        if (workstation == string.Empty) throw new Exception("Operator field is missing!");
        else Workstation = new Workstation(workstation);
    }
}