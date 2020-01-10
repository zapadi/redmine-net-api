using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Serialization
{
	/// <summary>
	/// 
	/// </summary>
   internal class XmlSerializerCache : IXmlSerializerCache
	{
		#if !(NET20 || NET40 || NET45 || NET451 || NET452)
        private static readonly Type[] emptyTypes = Array.Empty<Type>();
        #else
        private static readonly Type[] emptyTypes = new Type[0];
        #endif
		
		public static XmlSerializerCache Instance { get; } = new XmlSerializerCache();

		private readonly Dictionary<string, XmlSerializer> _serializers;		

		private readonly object _syncRoot;

		private XmlSerializerCache()
		{
			_syncRoot = new object();
			_serializers = new Dictionary<string, XmlSerializer>();		
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="defaultNamespace"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type, string defaultNamespace)
		{
			return GetSerializer(type, null, emptyTypes, null, defaultNamespace);
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="root"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type, XmlRootAttribute root)
		{
			return GetSerializer(type, null, emptyTypes, root, null);
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="overrides"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type, XmlAttributeOverrides overrides)
		{
			return GetSerializer(type, overrides, emptyTypes, null, null);
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type, Type[] types)
		{
			return GetSerializer(type, null, types, null, null);
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="overrides"></param>
		/// <param name="types"></param>
		/// <param name="root"></param>
		/// <param name="defaultNamespace"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type, XmlAttributeOverrides overrides, Type[] types, XmlRootAttribute root, string defaultNamespace)
		{
			var key = CacheKeyFactory.Create(type, overrides, types, root, defaultNamespace);

			XmlSerializer serializer = null;
			lock (_syncRoot)
			{
				if (_serializers.ContainsKey(key) == false)
				{
					lock (_syncRoot)
					{
						if (_serializers.ContainsKey(key) == false)
						{
							serializer = new XmlSerializer(type, overrides, types, root, defaultNamespace);
							_serializers.Add(key, serializer);
						} 
					} 
				} 
				else
				{
					serializer = _serializers[key];
				}

				Debug.Assert(serializer != null);
				return serializer;
			}
		}
	}
}