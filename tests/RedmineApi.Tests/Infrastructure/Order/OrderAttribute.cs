using System;

namespace Padi.RedmineAPI.Tests.Infrastructure.Order;

public sealed class OrderAttribute : Attribute
{
    public OrderAttribute(int index)
    {
        Index = index;
    }

    public int Index { get; private set; }
}