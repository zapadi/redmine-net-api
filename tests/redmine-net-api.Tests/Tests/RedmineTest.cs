using Padi.RedmineApi.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests
{
    [Trait("Redmine-api", "Credentials")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    [Order(1)]
    public sealed class RedmineTest
    {
        private static readonly RedmineCredentials Credentials;


        static RedmineTest()
        {
            Credentials = TestHelper.GetApplicationConfiguration();
        }

        [Fact]
        public void Should_Throw_Redmine_Exception_When_Host_Is_Null()
        {
            Assert.Throws<RedmineException>(() => new RedmineManager(null, Credentials.Username, Credentials.Password));
        }

	    [Fact]
	    public void Should_Throw_Redmine_Exception_When_Host_Is_Empty()
	    {
		    Assert.Throws<RedmineException>(() => new RedmineManager(string.Empty, Credentials.Username, Credentials.Password));
	    }

	    [Fact]
        public void Should_Throw_Redmine_Exception_When_Host_Is_Invalid()
        {
            Assert.Throws<RedmineException>(() => new RedmineManager("invalid<>", Credentials.Username, Credentials.Password));
        }

        [Fact]
        public void Should_Connect_With_Username_And_Password()
        {
            var a = new RedmineManager(Credentials.Uri, Credentials.Username, Credentials.Password);
            var currentUser = a.GetCurrentUser();
            Assert.NotNull(currentUser);
            Assert.True(currentUser.Login.Equals(Credentials.Username), "usernames not equals.");
        }

        [Fact]
        public void Should_Connect_With_Api_Key()
        {
            var a = new RedmineManager(Credentials.Uri, Credentials.ApiKey);
            var currentUser = a.GetCurrentUser();
            Assert.NotNull(currentUser);
            Assert.True(currentUser.ApiKey.Equals(Credentials.ApiKey),"api keys not equals.");
        }
    }
}
