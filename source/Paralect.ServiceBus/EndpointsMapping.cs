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

        public void Map(Func<Type, Boolean> typeChecker, String queueName, IEndpointProvider endpointProvider)
        {
            var endpoint = new EndpointDirection
            {
                EndpointAddress = new EndpointAddress(queueName),
                TypeChecker = typeChecker,
                EndpointProvider = endpointProvider
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

    public class EndpointDirection
    {
        public Func<Type, Boolean> TypeChecker { get; set; }
        public EndpointAddress EndpointAddress { get; set; }

        /// <summary>
        /// Can be null
        /// </summary>
        public IEndpointProvider EndpointProvider { get; set; }
    }
}
