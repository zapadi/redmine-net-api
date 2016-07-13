using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace xUnitTestredminenet45api
{
    /// <summary>
    ///     Custom xUnit test collection orderer that uses the OrderAttribute
    /// </summary>
    public class CollectionOrderer : ITestCollectionOrderer
    {
        public const string TYPE_NAME = "xUnitTestredminenet45api.CollectionOrderer";
        public const string ASSEMBY_NAME = "xUnitTest-redmine-net45-api";

        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
        {
            return testCollections.OrderBy(GetOrder);
        }

        /// <summary>
        ///     Test collections are not bound to a specific class, however they
        ///     are named by default with the type name as a suffix. We try to
        ///     get the class name from the DisplayName and then use reflection to
        ///     find the class and OrderAttribute.
        /// </summary>
        private static int GetOrder(ITestCollection testCollection)
        {
            var i = testCollection.DisplayName.LastIndexOf(' ');
            if (i <= -1) return 0;

            var className = testCollection.DisplayName.Substring(i + 1);
            var type = Type.GetType(className);
            if (type == null) return 0;

            var attr = type.GetCustomAttribute<OrderAttribute>();
            return attr != null ? attr.Index : 0;
        }
    }
}