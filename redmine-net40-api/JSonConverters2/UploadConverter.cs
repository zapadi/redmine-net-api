using Newtonsoft.Json.Linq;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters2
{
    public class UploadConverter : IJsonConverter
    {
        public void Serialize(Newtonsoft.Json.JsonWriter writer, object obj, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (obj == null) return;

            var item = (Upload)obj;

            writer.WriteProperty(RedmineKeys.CONTENT_TYPE, item.ContentType);
            writer.WriteProperty(RedmineKeys.FILENAME, item.FileName);
            writer.WriteProperty(RedmineKeys.TOKEN, item.Token);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, item.Description);
        }

        public object Deserialize(JObject obj, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (obj == null) return null;

            var upload = new Upload
            {
                ContentType = obj.Value<string>(RedmineKeys.CONTENT_TYPE),
                FileName = obj.Value<string>(RedmineKeys.FILENAME),
                Token = obj.Value<string>(RedmineKeys.TOKEN),
                Description = obj.Value<string>(RedmineKeys.DESCRIPTION)
            };
    
            return upload;
        }
    }
}
