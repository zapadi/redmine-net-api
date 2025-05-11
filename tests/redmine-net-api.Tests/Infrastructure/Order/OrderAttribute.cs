namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure.Order
{
    public sealed class OrderAttribute : Attribute
    {
        public OrderAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; private set; }
    }
}