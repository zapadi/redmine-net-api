#if !(NET20 || NET40)
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Padi.RedmineApi.Tests.Infrastructure
{
    /// <summary>
    /// Custom xUnit test case orderer that uses the OrderAttribute
    /// </summary>
    public class CaseOrderer : ITestCaseOrderer
    {
        public const string TYPE_NAME = "redmine.net.api.Tests.Infrastructure.CaseOrderer";
        public const string ASSEMBY_NAME = "redmine-net-api.Tests";

        public static readonly ConcurrentDictionary<string, ConcurrentQueue<string>> QueuedTests = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            return testCases.OrderBy(GetOrder);
        }

        private static int GetOrder<TTestCase>(TTestCase testCase)
            where TTestCase : ITestCase
        {
            // Enqueue the test name.
            QueuedTests
                .GetOrAdd(testCase.TestMethod.TestClass.Class.Name,key => new ConcurrentQueue<string>())
                .Enqueue(testCase.TestMethod.Method.Name);

            // Order the test based on the attribute.
            var attr = testCase.TestMethod.Method
                .ToRuntimeMethod()
                .GetCustomAttribute<OrderAttribute>();
            return attr != null ? attr.Index : 0;
        }
    }
}
#endif