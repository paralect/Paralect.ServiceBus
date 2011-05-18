using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public class EndpointsMapping
    {
        private List<String> _typeNames = new List<string>();
        private Dictionary<String, QueueName[]> _typeNamesMap = new Dictionary<string, QueueName[]>();

        public void Map(String typeName, String[] queueName)
        {
            _typeNames.Add(typeName);
            _typeNamesMap.Add(typeName, queueName.Select(name => new QueueName(name)).ToArray());
        }

        /// <summary>
        /// Should be cashed
        /// </summary>
        public QueueName[] GetQueues(Type type)
        {
            var name = type.FullName;

            foreach (var typeName in _typeNames)
            {
                if (name.StartsWith(typeName))
                    return _typeNamesMap[typeName];
            }

            throw new Exception(String.Format("No endpoint configured for type {0}.", type.FullName));
        }
    }
}
