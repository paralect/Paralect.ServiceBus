using System;
using System.IO;
using System.Messaging;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;

namespace Paralect.ServiceBus
{
    public class MsmqMessageFormatter : IMessageFormatter
    {
        public object Clone()
        {
            return new MsmqMessageFormatter();
        }

        public bool CanRead(Message message)
        {
            return true;
        }

        public object Read(Message message)
        {
            JsonSerializer jsonSerializer = CreateJsonSerializer();

            JsonReader reader = CreateJsonReader(message.BodyStream);

            var messages = jsonSerializer.Deserialize<Object>(reader);

            return messages;
        }

        public void Write(Message message, object obj)
        {
            Stream stm = new MemoryStream();

            JsonSerializer jsonSerializer = CreateJsonSerializer();

            JsonWriter jsonWriter = CreateJsonWriter(stm);

            jsonSerializer.Serialize(jsonWriter, obj);

            jsonWriter.Flush();

            message.BodyStream = stm;
        }

        private JsonSerializer CreateJsonSerializer()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Objects
            };

//            serializerSettings.Converters.Add(new MessageJsonConverter());

            return JsonSerializer.Create(serializerSettings);
        }

        protected JsonWriter CreateJsonWriter(Stream stream)
        {
            var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            return new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented };
        }

        protected JsonReader CreateJsonReader(Stream stream)
        {
            var streamReader = new StreamReader(stream, Encoding.UTF8);
            return new JsonTextReader(streamReader);
        }
    }
}