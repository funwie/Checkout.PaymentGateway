using System.Collections.Generic;
using System.Net;

namespace Checkout.ApiFaker
{
    public class EndpointResponse
    {
        public EndpointResponse(HttpStatusCode httpStatusCode, string contentType, string content, IDictionary<string, string> headers = null)
        {
            HttpStatusCode = httpStatusCode;
            ContentType = contentType;
            Content = content;
            Headers = headers;
        }

        public string ContentType { get; }
        public HttpStatusCode HttpStatusCode { get; }
        public string Content { get; }
        public IDictionary<string, string> Headers { get; set; }
    }
}