using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public class EndpointsMapping
    {
        private List<Endpoint> _endpoints = new List<Endpoint>();

        public List<Endpoint> Endpoints
        {
            get { return _endpoints; }
        }

        public void Map(String typeName, String queueName, IQueueProvider queueProvider)
        {
            var endpoint = new Endpoint
            {
                QueueName = new QueueName(queueName),
                TypeName = typeName,
                QueueProvider = queueProvider
            };

            _endpoints.Add(endpoint);
        }

        public List<Endpoint> GetEndpoints(Type type)
        {
            var name = type.FullName;

            var endpoints = new List<Endpoint>();

            foreach (var endpoint in _endpoints)
            {
                if (name.StartsWith(endpoint.TypeName))
                    endpoints.Add(endpoint);
            }

            return endpoints;
//            throw new Exception(String.Format("No endpoint configured for type {0}.", type.FullName));
        }

        /// <summary>
        /// Should be cashed
        /// </summary>
/*        public QueueName[] GetQueues(Type type)
        {
            var name = type.FullName;

            foreach (var typeName in _typeNames)
            {
                if (name.StartsWith(typeName))
                    return _typeNamesMap[typeName];
            }

            throw new Exception(String.Format("No endpoint configured for type {0}.", type.FullName));
        }*/
    }
}
