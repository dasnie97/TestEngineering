using ProductTest.Common;
using ProductTest.Interfaces;
using ProductTest.Models;
using System.Security.Cryptography;

namespace ProductTestTest;

public class TestReportTest
{
    [Fact]
    public void CheckTestReportCreationFromFileWithBadArgument_ThrowsAnException()
    {
        Action action = () => FileTestReport.CreateFromFile("");

        Assert.Throws<FileNotFoundException>(action);
    }

    [Fact]
    public void TestIfTestReportIsCreatedProperly()
    {
        FileTestReportCreator creator = new FileTestReportCreator();
        FileTestReport testReport = creator.Create();

        Assert.Equal(creator.SerialNumber, testReport.SerialNumber);
        Assert.Equal(creator.TestSteps.First().Status, testReport.Status);
        Assert.Equal(creator.Workstation.Name, testReport.Workstation.Name);
        Assert.Equal(creator.TestSteps.First().DateTimeFinish, testReport.TestDateTimeStarted);
        Assert.Equal(creator.TestSteps, testReport.TestSteps);
    }

    [Fact]
    public void TestIfTestReportFileIsCreatedProperly()
    {
        string currentDir = Directory.GetCurrentDirectory();
        FileTestReportCreator creator = new FileTestReportCreator();
        FileTestReport testReport = creator.Create();

        testReport.SaveReport(currentDir);
        FileTestReport testReportCreatedFromFile = FileTestReport.CreateFromFile(testReport.FilePath);

        Assert.True(File.Exists(testReport.FilePath));
        Assert.Equal(testReport.SerialNumber, testReportCreatedFromFile.SerialNumber);
        Assert.Equal(testReport.Status, testReportCreatedFromFile.Status);
        Assert.Equal(testReport.Workstation.Name, testReportCreatedFromFile.Workstation.Name);
        Assert.Equal(testReport.Failure, testReportCreatedFromFile.Failure);
        Assert.Equal(testReport.FixtureSocket, testReportCreatedFromFile.FixtureSocket);
        Assert.Equal(testReport.TestDateTimeStarted, testReportCreatedFromFile.TestDateTimeStarted);
        Assert.Equal(testReport.TestSteps.First().Name, testReportCreatedFromFile.TestSteps.First().Name);
        Assert.Equal(testReport.TestSteps.First().Type, testReportCreatedFromFile.TestSteps.First().Type);
        Assert.Equal(testReport.TestSteps.First().DateTimeFinish, testReportCreatedFromFile.TestSteps.First().DateTimeFinish);
        Assert.Equal(testReport.TestSteps.First().Status, testReportCreatedFromFile.TestSteps.First().Status);
        Assert.Equal(testReport.TestSteps.First().Value, testReportCreatedFromFile.TestSteps.First().Value);
        Assert.Equal(testReport.TestSteps.First().Unit, testReportCreatedFromFile.TestSteps.First().Unit);
        Assert.Equal(testReport.TestSteps.First().LowerLimit, testReportCreatedFromFile.TestSteps.First().LowerLimit);
        Assert.Equal(testReport.TestSteps.First().UpperLimit, testReportCreatedFromFile.TestSteps.First().UpperLimit);
        Assert.Equal(testReport.TestSteps.First().IsNumeric, testReportCreatedFromFile.TestSteps.First().IsNumeric);
        Assert.Equal(testReport.TestSteps.First().Failure, testReportCreatedFromFile.TestSteps.First().Failure);

        File.Delete(testReport.FilePath);
    }
}