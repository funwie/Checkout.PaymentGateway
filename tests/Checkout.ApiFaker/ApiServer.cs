using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Checkout.ApiFaker
{
    public class ApiServer
    {
        private IList<Request> _requests;
        private readonly Dictionary<string, Dictionary<HttpMethod, Action<HttpRequest, HttpResponse>>> _endpoints;

        public IList<Request> Requests => _requests ??= new List<Request>();

        public ApiServer()
        {
            _endpoints = new Dictionary<string, Dictionary<HttpMethod, Action<HttpRequest, HttpResponse>>>();
        }

        public ApiServer(Dictionary<string, Dictionary<HttpMethod, Action<HttpRequest, HttpResponse>>> endpoints)
        {
            _endpoints = endpoints;
        }

        public void Configuration(IApplicationBuilder appBuilder)
        {
            foreach (var endpoint in _endpoints.OrderByDescending(x => x.Key.Length))
            {
                appBuilder.Map(endpoint.Key, builder => builder.Run( httpContext =>
                  {
                      Task<string> body;
                      using (var streamReader = new StreamReader(httpContext.Request.Body))
                      {
                          body = streamReader.ReadToEndAsync();
                      }

                      byte[] requestData = Encoding.UTF8.GetBytes(body.Result);
                      httpContext.Request.Body = new MemoryStream(requestData);

                      Requests.Add(new Request(httpContext.Request.Method, new Uri(httpContext.Request.GetDisplayUrl()), body.Result, httpContext.Request.Headers.Select(h => new RequestHeader(h.Key, h.Value)).ToList()));

                      var httpMethod = new HttpMethod(httpContext.Request.Method);
                      if (endpoint.Value.ContainsKey(httpMethod))
                          return new TaskFactory().StartNew(() => endpoint.Value[httpMethod](httpContext.Request, httpContext.Response));

                      httpContext.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                      return httpContext.Response.WriteAsync("");
                  }));
            }
        }

        public IDisposable Start(string uri)
        {
            var host = WebHost
               .CreateDefaultBuilder()
               .Configure(Configuration)
               .UseUrls(uri)
               .Build();

            host.Start();

            return host;
        }
    }
}

