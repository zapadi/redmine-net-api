using System.Net;

namespace Redmine.Net.Api;

/// <summary>
/// 
/// </summary>
public interface IRedmineAuthentication
{
    /// <summary>
    /// 
    /// </summary>
    string AuthenticationType { get; }

    /// <summary>
    /// 
    /// </summary>
    string Token { get; }

    ///<summary>
    /// 
    ///</summary>
    ICredentials Credentials { get; }
}