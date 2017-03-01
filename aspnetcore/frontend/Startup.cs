using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Frontend
{
    public class Startup
    {
        private static readonly PathString _path = new PathString("/ingest/event");
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();
        private static HttpClientLoadBalancer _clientLoadBalancer;

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables("DATAINGESTION_");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; private set; }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Error);

            var clients = int.Parse(Configuration["clients"]);
            _clientLoadBalancer = new HttpClientLoadBalancer(clients);

            var clientType = String.IsNullOrEmpty(Configuration["clientType"]) ? "full" : Configuration["clientType"];

            var backendUrl = Configuration["backend"] + "/ingest/data";

            Console.WriteLine($"ClientType: {clientType}, Clients: {clients}, BackendUrl: {backendUrl}");

            app.Run(async context =>
            {
                if (context.Request.Path.StartsWithSegments(_path, StringComparison.Ordinal))
                {
                    Payload payload;
                    using (var reader = new JsonTextReader(new StreamReader(context.Request.Body))) {
                        payload = _jsonSerializer.Deserialize<Payload>(reader);
                    }

                    using (var response = await RedirectPayload(payload, backendUrl, clientType))
                    {
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            });
        }

        private static async Task<HttpResponseMessage> RedirectPayload(Payload payload, string url, string clientType) {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var content = new StreamContent(stream))
            {
                _jsonSerializer.Serialize(writer, payload);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                if (clientType.Equals("full", StringComparison.OrdinalIgnoreCase))
                {
                    return await _clientLoadBalancer.PostAsync(url, content);
                }
                else if (clientType.Equals("slim", StringComparison.OrdinalIgnoreCase))
                {
                    var responseString = await HttpClientSlim.PostAsync(url, content);
                    return new HttpResponseMessage() { Content = new StringContent(responseString) };
                }
                else if (clientType.Equals("none", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public static void Main(string[] args)
        {
            var hostBuilder = new WebHostBuilder()
                .UseUrls("http://+:8080")
                .UseKestrel()
                .UseStartup<Startup>();

            var host = hostBuilder.Build();
            host.Run();
        }
    }

    public class Payload
    {
        public string Data { get; set; }
    }
}
