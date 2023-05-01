using AutoBogus;
using Bogus;
using System.Security.Cryptography.X509Certificates;
using TestEngineering.Models;

namespace TestEngineering.Other;

public class TestReportGenerator
{
	public TestReportGenerator()
	{
		Randomizer.Seed = new Random(1);
	}

	public static TestReport GenerateFakeTestReport()
	{
		return AutoFaker.Generate<TestReport>();
    }
}
