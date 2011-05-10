using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Paralect.ServiceBus.Serialization
{
    /// <summary>
    /// Supposed to be used by MsmqMessageFormatter, but not used as of right now
    /// </summary>
    public class MessageJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string typeName = GetTypeName(value.GetType());
            var jobj = JObject.FromObject(value);
            jobj.AddFirst(new JProperty("$type", typeName));
            jobj.WriteTo(writer);
        }

        private string GetTypeName(Type mappedType)
        {
            return string.Format("{0}, {1}", mappedType.FullName, mappedType.Assembly.GetName().Name);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobject = JObject.Load(reader);
            var typeName = jobject.Value<string>("$type");
            var type = Type.GetType(typeName);
            var instance = Activator.CreateInstance(type); // _messageMapper.CreateInstance(type);

            serializer.Populate(jobject.CreateReader(), instance);

            return instance;
        }

        public override bool CanConvert(Type objectType)
        {
            return true; // typeof(IMessage).IsAssignableFrom(objectType);
        }
    }
}
