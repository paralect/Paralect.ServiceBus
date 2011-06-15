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

        public void Map(Func<Type, Boolean> typeChecker, String queueName, IQueueProvider queueProvider)
        {
            var endpoint = new Endpoint
            {
                QueueName = new QueueName(queueName),
                TypeChecker = typeChecker,
                QueueProvider = queueProvider
            };

            _endpoints.Add(endpoint);
        }

        public List<Endpoint> GetEndpoints(Type type)
        {
            var endpoints = new List<Endpoint>();

            foreach (var endpoint in _endpoints)
            {
                if (endpoint.TypeChecker(type))
                    endpoints.Add(endpoint);
            }

            return endpoints;
        }
    }
}
