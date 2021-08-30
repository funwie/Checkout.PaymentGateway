using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Checkout.ApiFaker
{
    public class ApiServerBuilder
    {
        private readonly Dictionary<string, Dictionary<HttpMethod,Action<HttpRequest, HttpResponse>>> _endpoints;

        public ApiServerBuilder()
        {
            _endpoints = new Dictionary<string, Dictionary<HttpMethod,Action<HttpRequest, HttpResponse>>>();
        }

        public ApiServerBuilder WithEndpoint(string uri, HttpMethod httpMethod, HttpStatusCode httpStatusCode, string contentType, string content, IDictionary<string, string> responseHeaders = null)
        {
            return WithEndpoint(new Endpoint(uri,httpMethod,httpStatusCode,contentType,content, responseHeaders: responseHeaders));
        }

        public ApiServerBuilder WithEndpoint(Endpoint endpoint)
        {
            if (_endpoints.ContainsKey(endpoint.Uri))
            {
                _endpoints[endpoint.Uri].Add(endpoint.Method, (request, response) =>
                {
                    string requestBody;
                    using (var sr = new StreamReader(request.Body))
                    {
                        requestBody = sr.ReadToEnd();
                    }
                    var endpointResponse = endpoint.EndpointResponseResolver?.Invoke(new EndpointRequest(new Uri(request.GetDisplayUrl()), request.Method, requestBody)) ?? endpoint.EndpointResponse;

                    if (endpointResponse.Headers != null)
                    {
                        foreach (var header in endpointResponse.Headers)
                        {
                            response.Headers.Add(header.Key, header.Value);
                        }
                    }

                    response.StatusCode = (int)endpointResponse.HttpStatusCode;
                    response.ContentType = endpointResponse.ContentType;
                    response.WriteAsync(endpointResponse.Content);
                });
            }
            else
            {
                _endpoints.Add(endpoint.Uri, new Dictionary<HttpMethod, Action<HttpRequest, HttpResponse>>()
                {
                    {
                        endpoint.Method, (request, response) =>
                        {
                            string requestBody;
                            using (var sr = new StreamReader(request.Body))
                            {
                                requestBody = sr.ReadToEnd();
                            }

                            var endpointResponse = endpoint.EndpointResponseResolver?.Invoke(new EndpointRequest(new Uri(request.GetDisplayUrl()), request.Method, requestBody)) ?? endpoint.EndpointResponse;

                            if (endpointResponse.Headers != null)
                            {
                                foreach (var header in endpointResponse.Headers)
                                {
                                    response.Headers.Add(header.Key, header.Value);
                                }
                            }

                            response.StatusCode = (int) endpointResponse.HttpStatusCode;
                            response.ContentType = endpointResponse.ContentType;
                            response.WriteAsync(endpointResponse.Content);

                            endpoint.ResponseSentCallback?.Invoke(endpoint);
                        }
                    }
                });
            }

            return this;
        }

        public ApiServer Build()
        {
            return new ApiServer(_endpoints);
        }
    }
}
