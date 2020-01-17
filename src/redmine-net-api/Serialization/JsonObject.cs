using System;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class JsonObject : IDisposable
    {
        private readonly bool hasRoot;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="root"></param>
        public JsonObject(JsonWriter writer, string root = null)
        {
            Writer = writer;
            Writer.WriteStartObject();

            if (!root.IsNullOrWhiteSpace())
            {
                hasRoot = true;
                Writer.WritePropertyName(root);
                Writer.WriteStartObject();
            }
        }

        private JsonWriter Writer { get; }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Writer.WriteEndObject();
            if (hasRoot)
            {
                Writer.WriteEndObject();
            }
        }
    }
}