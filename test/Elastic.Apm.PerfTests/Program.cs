﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Elastic.Apm.Logging;
using Elastic.Apm.Tests.Mocks;

namespace Elastic.Apm.PerfTests
{
	[MemoryDiagnoser]
	public class Program
	{
		public static void Main()
		{
			BenchmarkRunner.Run<Program>();
		}

		[Benchmark]
		public void SimpleTransactions10Spans()
		{
			var agent = new ApmAgent(new AgentComponents(payloadSender: new MockPayloadSender()));

			for (var i = 0; i < 100; i++)
			{
				agent.Tracer.CaptureTransaction("transaction", "perfTransaction", (transaction) =>
				{
					for (var j = 0; j < 10; j++)
					{
						transaction.CaptureSpan("span", "perfSpan", () => { });
					}
				});
			}
		}

		[Benchmark]
		public void DebugLogSimple100Transaction10Spans()
		{
			var testLogger = new PerfTestLogger(LogLevel.Debug);
			var agent = new ApmAgent(new AgentComponents(payloadSender: new MockPayloadSender(), logger: testLogger,
				configurationReader: new TestAgentConfigurationReader(testLogger, logLevel: "Debug")));


			for (var i = 0; i < 100; i++)
			{
				agent.Tracer.CaptureTransaction("transaction", "perfTransaction", (transaction) =>
				{
					for (var j = 0; j < 10; j++)
					{
						transaction.CaptureSpan("span", "perfSpan", () => { });
					}
				});
			}
		}
	}
}
