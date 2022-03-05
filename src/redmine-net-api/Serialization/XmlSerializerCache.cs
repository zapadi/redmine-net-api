/*
   Copyright 2011 - 2022 Adrian Popescu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

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
        private static readonly Type[] EmptyTypes = Array.Empty<Type>();
        #else
        private static readonly Type[] EmptyTypes = new Type[0];
        #endif
		
		public static XmlSerializerCache Instance { get; } = new XmlSerializerCache();

		private readonly Dictionary<string, XmlSerializer> serializers;		

		private readonly object syncRoot;

		private XmlSerializerCache()
		{
			syncRoot = new object();
			serializers = new Dictionary<string, XmlSerializer>();		
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
			return GetSerializer(type, null, EmptyTypes, null, defaultNamespace);
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
			return GetSerializer(type, null, EmptyTypes, root, null);
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
			return GetSerializer(type, overrides, EmptyTypes, null, null);
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
			lock (syncRoot)
			{
				if (serializers.ContainsKey(key) == false)
				{
					lock (syncRoot)
					{
						if (serializers.ContainsKey(key) == false)
						{
							serializer = new XmlSerializer(type, overrides, types, root, defaultNamespace);
							serializers.Add(key, serializer);
						} 
					} 
				} 
				else
				{
					serializer = serializers[key];
				}

				Debug.Assert(serializer != null);
				return serializer;
			}
		}
	}
}