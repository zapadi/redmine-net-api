using System;

namespace Padi.RedmineApi.Tests.Infrastructure
{
    public class OrderAttribute : Attribute
    {
        public OrderAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; private set; }
    }
}