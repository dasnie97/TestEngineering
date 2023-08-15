using TestEngineering.Models;
using TestEngineering.Other;
using Xunit;

namespace TestEngineering.Tests;

public class TestReportTest
{
    [Fact]
    public void CheckTestReportCreationFromFileWithBadArgument_ThrowsAnException()
    {
        Action action = () => FileTestReport.CreateFromFile("");

        Assert.Throws<FileNotFoundException>(action);
    }

    [Fact]
    public void TestIfTestReportFileIsCreatedProperly()
    {
        string currentDir = Directory.GetCurrentDirectory();
        TestReport fakeTestReport = TestReportGenerator.GenerateFakeTestReport();
        TestStep fakeTestStep = fakeTestReport.TestSteps.First();
        TestStep testStep = new TestStep(fakeTestStep.Name, fakeTestStep.DateTimeFinish, fakeTestStep.Status);
        testStep.UpperLimit = fakeTestStep.UpperLimit;
        testStep.LowerLimit = fakeTestStep.LowerLimit;
        testStep.Failure = fakeTestStep.Failure;
        testStep.Value = fakeTestStep.Value;
        testStep.Unit = fakeTestStep.Unit;
        testStep.Type = fakeTestStep.Type;
        TestReport testReport = new TestReport(
            fakeTestReport.SerialNumber,
            fakeTestReport.Workstation,
            new List<TestStep>
            {
                testStep
            });
        FileTestReport fileTestReport = FileTestReport.Create(
            testReport.SerialNumber,
            testReport.Workstation,
            testReport.TestSteps);

        fileTestReport.SaveReport(currentDir);
        FileTestReport testReportCreatedFromFile = FileTestReport.CreateFromFile(fileTestReport.FilePath);

        Assert.True(File.Exists(fileTestReport.FilePath));
        Assert.Equal(testReport.SerialNumber.ToUpperInvariant(), testReportCreatedFromFile.SerialNumber);
        Assert.Equal(testReport.Workstation.Name, testReportCreatedFromFile.Workstation.Name);
        Assert.Equal(testReport.Failure, testReportCreatedFromFile.Failure);
        Assert.Equal(testReport.FixtureSocket, testReportCreatedFromFile.FixtureSocket);
        Assert.Equal(testReport.TestDateTimeStarted, testReportCreatedFromFile.TestDateTimeStarted, TimeSpan.FromSeconds(1));
        Assert.Equal(testReport.TestSteps.First().Name, testReportCreatedFromFile.TestSteps.First().Name);
        Assert.Equal(testReport.TestSteps.First().Type, testReportCreatedFromFile.TestSteps.First().Type);
        Assert.Equal(testReport.TestSteps.First().DateTimeFinish, testReportCreatedFromFile.TestSteps.First().DateTimeFinish, TimeSpan.FromSeconds(1));
        Assert.Equal(testReport.TestSteps.First().Status, testReportCreatedFromFile.TestSteps.First().Status);
        Assert.Equal(testReport.TestSteps.First().Value, testReportCreatedFromFile.TestSteps.First().Value);
        Assert.Equal(testReport.TestSteps.First().Unit, testReportCreatedFromFile.TestSteps.First().Unit);
        Assert.Equal(testReport.TestSteps.First().LowerLimit, testReportCreatedFromFile.TestSteps.First().LowerLimit);
        Assert.Equal(testReport.TestSteps.First().UpperLimit, testReportCreatedFromFile.TestSteps.First().UpperLimit);
        Assert.Equal(testReport.TestSteps.First().IsNumeric, testReportCreatedFromFile.TestSteps.First().IsNumeric);
        Assert.Equal(testReport.TestSteps.First().Failure, testReportCreatedFromFile.TestSteps.First().Failure);

        File.Delete(fileTestReport.FilePath);
    }

    [Theory]
    [InlineData(TestStatus.Failed, TestStatus.Failed)]
    [InlineData(TestStatus.Terminated, TestStatus.Failed)]
    [InlineData(TestStatus.Error, TestStatus.Failed)]
    [InlineData(TestStatus.Notset, TestStatus.Failed)]
    [InlineData(TestStatus.Passed, TestStatus.Passed)]
    public void WhenStatusOtherThanPassed_ShouldBeWrittenAsFailedInFileTestReport(TestStatus status, TestStatus expectedStatus)
    {
        string currentDir = Directory.GetCurrentDirectory();
        TestReport fakeTestReport = TestReportGenerator.GenerateFakeTestReport();
        TestStep fakeTestStep = fakeTestReport.TestSteps.First();
        TestReport testReport = new TestReport(
            fakeTestReport.SerialNumber,
            fakeTestReport.Workstation,
            new List<TestStep>
            {
                new TestStep(fakeTestStep.Name, fakeTestStep.DateTimeFinish, status)
            });
        FileTestReport fileTestReport = FileTestReport.Create(
            testReport.SerialNumber,
            testReport.Workstation,
            testReport.TestSteps);

        fileTestReport.SaveReport(currentDir);
        FileTestReport testReportCreatedFromFile = FileTestReport.CreateFromFile(fileTestReport.FilePath);

        Assert.True(testReportCreatedFromFile.Status == expectedStatus);

        File.Delete(fileTestReport.FilePath);
    }
}