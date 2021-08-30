using System;
using System.Collections.Generic;

namespace Checkout.ApiFaker
{
    public class Request
    {
        public string Method { get; set; }
        public Uri Uri { get; set; }
        public string Body { get; set; }
        public List<RequestHeader> Headers { get; set; }

        public Request(string method, Uri uri, string body, List<RequestHeader> headers)
        {
            Method = method;
            Uri = uri;
            Body = body;
            Headers = headers;
        }
    }
}