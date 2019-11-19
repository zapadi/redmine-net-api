using System;

namespace redmine.net.api.Tests.Infrastructure
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