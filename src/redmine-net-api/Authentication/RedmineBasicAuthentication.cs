using System;
using System.Net;
using System.Text;
using Redmine.Net.Api.Exceptions;

namespace Redmine.Net.Api.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RedmineBasicAuthentication: IRedmineAuthentication
    {
        /// <inheritdoc />
        public string AuthenticationType { get; } = "Basic";

        /// <inheritdoc />
        public string Token { get; init; }

        /// <inheritdoc />
        public ICredentials Credentials { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public RedmineBasicAuthentication(string username, string password)
        {
            if (username == null) throw new RedmineException(nameof(username));
            if (password == null) throw new RedmineException(nameof(password));
            
            Token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        }
    }
}