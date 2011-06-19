using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public class EndpointsMapping
    {
        private List<EndpointDirection> _endpoints = new List<EndpointDirection>();

        public List<EndpointDirection> Endpoints
        {
            get { return _endpoints; }
        }

        public void Map(Func<Type, Boolean> typeChecker, String queueName, IQueueProvider queueProvider)
        {
            var endpoint = new EndpointDirection
            {
                QueueName = new QueueName(queueName),
                TypeChecker = typeChecker,
                QueueProvider = queueProvider
            };

            _endpoints.Add(endpoint);
        }

        public List<EndpointDirection> GetEndpoints(Type type)
        {
            var endpoints = new List<EndpointDirection>();

            foreach (var endpoint in _endpoints)
            {
                if (endpoint.TypeChecker(type))
                    endpoints.Add(endpoint);
            }

            return endpoints;
        }
    }
}
