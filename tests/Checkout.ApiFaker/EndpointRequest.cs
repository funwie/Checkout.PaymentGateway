using System;

namespace Checkout.ApiFaker
{
    public class EndpointRequest
    {
        public Uri Uri { get; set; }
        public string RequestMethod { get; set; }
        public string Body { get; set; }

        public EndpointRequest(Uri uri, string requestMethod, string body)
        {
            Uri = uri;
            RequestMethod = requestMethod;
            Body = body;
        }
    }
}