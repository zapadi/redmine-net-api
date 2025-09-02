using System;
using Xunit;

namespace Padi.RedmineAPI.Tests.Equality;

public abstract class BaseEqualityTests<T> where T : class, IEquatable<T>
    {
        protected abstract T CreateSampleInstance();
        protected abstract T CreateDifferentInstance();

        [Fact]
        public void Equals_SameReference_ReturnsTrue()
        {
            var instance = CreateSampleInstance();
            Assert.True(instance.Equals(instance));
        }

        [Fact]
        public void Equals_Null_ReturnsFalse()
        {
            var instance = CreateSampleInstance();
            Assert.False(instance.Equals(null));
        }

        [Fact]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var instance = CreateSampleInstance();
            var differentObject = new object();
            Assert.False(instance.Equals(differentObject));
        }

        [Fact]
        public void Equals_IdenticalProperties_ReturnsTrue()
        {
            var instance1 = CreateSampleInstance();
            var instance2 = CreateSampleInstance();
            Assert.True(instance1.Equals(instance2));
            Assert.True(instance2.Equals(instance1));
        }

        [Fact]
        public void Equals_DifferentProperties_ReturnsFalse()
        {
            var instance1 = CreateSampleInstance();
            var instance2 = CreateDifferentInstance();
            Assert.False(instance1.Equals(instance2));
            Assert.False(instance2.Equals(instance1));
        }

        [Fact]
        public void GetHashCode_SameProperties_ReturnsSameValue()
        {
            var instance1 = CreateSampleInstance();
            var instance2 = CreateSampleInstance();
            Assert.Equal(instance1.GetHashCode(), instance2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentProperties_ReturnsDifferentValues()
        {
            var instance1 = CreateSampleInstance();
            var instance2 = CreateDifferentInstance();
            Assert.NotEqual(instance1.GetHashCode(), instance2.GetHashCode());
        }
    }