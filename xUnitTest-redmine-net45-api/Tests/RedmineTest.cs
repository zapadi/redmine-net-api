using System;
using System.Runtime.Remoting;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Xunit;

namespace xUnitTestredminenet45api.Tests
{
    [Trait("Redmine-api", "Credentials")]
    [Collection("RedmineCollection")]
    [Order(1)]
    public class RedmineTest
    {

        [Fact]
        public void Should_Throw_Redmine_Exception_When_Host_Is_Null()
        {
            Assert.Throws<RedmineException>(() => new RedmineManager(null, Helper.Username, Helper.Password));
        }

	    [Fact]
	    public void Should_Throw_Redmine_Exception_When_Host_Is_Empty()
	    {
		    Assert.Throws<RedmineException>(() => new RedmineManager(String.Empty, Helper.Username, Helper.Password));
	    }

	    [Fact]
        public void Should_Throw_Redmine_Exception_When_Host_Is_Invalid()
        {
            Assert.Throws<RedmineException>(() => new RedmineManager("invalid<>", Helper.Username, Helper.Password));
        }

        [Fact]
        public void Should_Connect_With_Username_And_Password()
        {
            var a = new RedmineManager(Helper.Uri, Helper.Username, Helper.Password);
            var currentUser = a.GetCurrentUser();
            Assert.NotNull(currentUser);
            Assert.True(currentUser.Login.Equals(Helper.Username), "usernames not equals.");
        }

        [Fact]
        public void Should_Connect_With_Api_Key()
        {
            var a = new RedmineManager(Helper.Uri, Helper.ApiKey);
            var currentUser = a.GetCurrentUser();
            Assert.NotNull(currentUser);
            Assert.True(currentUser.ApiKey.Equals(Helper.ApiKey),"api keys not equals.");
        }
    }
}
