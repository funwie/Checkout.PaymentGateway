using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Checkout.ApiFaker
{
    public class Endpoint
    {
        public Endpoint(string uri, HttpMethod httpMethod, HttpStatusCode httpStatusCode, string contentType, string content, Action<Endpoint> responseSentCallback = null, IDictionary<string, string> responseHeaders = null)
        {
            Uri = uri;
            Method = httpMethod;
            EndpointResponse = new EndpointResponse(httpStatusCode, contentType, content, responseHeaders);
            ResponseSentCallback = responseSentCallback;
        }
        public Endpoint(string uri, HttpMethod httpMethod, Func<EndpointRequest, EndpointResponse> endpointResponseResolver, Action<Endpoint> responseSentCallback = null)
        {
            Uri = uri;
            Method = httpMethod;
            EndpointResponseResolver = endpointResponseResolver;
            ResponseSentCallback = responseSentCallback;
        }

        public string Uri { get; }
        public HttpMethod Method { get; }
        public EndpointResponse EndpointResponse { get; set; }
        public Func<EndpointRequest, EndpointResponse> EndpointResponseResolver { get; set; }
        public Action<Endpoint> ResponseSentCallback { get; }
    }
}
