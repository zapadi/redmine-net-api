using System;
using System.Xml.Serialization;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RedmineManagerOptionsBuilder
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithPageSize(int pageSize)
        {
            this.PageSize = pageSize;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithBaseAddress(string baseAddress)
        {
            return WithBaseAddress(new Uri(baseAddress));
        }

        /// <summary>
        /// 
        /// </summary>
        public Uri BaseAddress { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithBaseAddress(Uri baseAddress)
        {
            this.BaseAddress = baseAddress;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationType"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithSerializationType(SerializationType serializationType)
        {
            this.SerializationType = serializationType;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public SerializationType SerializationType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authentication"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithAuthentication(IRedmineAuthentication authentication)
        {
            this.Authentication = authentication;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public IRedmineAuthentication Authentication { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientFunc"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithClient(Func<IRedmineApiClient> clientFunc)
        {
            this.ClientFunc = clientFunc;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public Func<IRedmineApiClient> ClientFunc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientOptions"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithClientOptions(IRedmineApiClientOptions clientOptions)
        {
            this.ClientOptions = clientOptions;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public IRedmineApiClientOptions ClientOptions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public RedmineManagerOptionsBuilder WithVersion(Version version)
        {
            this.Version = version;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public Version Version { get; set; }

        internal RedmineManagerOptionsBuilder WithVerifyServerCert(bool verifyServerCert)
        {
            this.VerifyServerCert = verifyServerCert;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool VerifyServerCert { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal RedmineManagerOptions Build()
        {
            if (Authentication == null)
            {
                throw new RedmineException("Authentication cannot be null");
            }
            
            var options = new RedmineManagerOptions()
            {
                PageSize = PageSize > 0 ? PageSize : RedmineManager.DEFAULT_PAGE_SIZE_VALUE,
                VerifyServerCert = VerifyServerCert,
                Serializer = SerializationType == SerializationType.Xml ? new XmlRedmineSerializer() : new JsonRedmineSerializer(),
                Version = Version,
                //Authentication =
                ClientOptions = ClientOptions,
                
            };

            
            return options;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static bool TryParse(string serviceName, out string parts)
        {
            parts = null;
            return false;
        }
        
    }
}