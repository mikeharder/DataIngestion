using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace Frontend {
    public class Startup
    {
        private static readonly PathString _path = new PathString("/ingest/event");
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        private static HttpClient[] _httpClients;
        private static long _httpClientCounter = 0;

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables("DATAINGESTION_");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; private set; }

        public void Configure(IApplicationBuilder app)
        {
            var clients = int.Parse(Configuration["clients"]);
            _httpClients = new HttpClient[clients];
            for (var i = 0; i < clients; i++)
            {
                _httpClients[i] = new HttpClient();
            }

            bool tempBool;
            var expectContinue = bool.TryParse(Configuration["expectContinue"], out tempBool) ? (bool?)tempBool : null;

            Console.WriteLine($"Clients: {clients}, ExpectContinue: {expectContinue?.ToString() ?? "null"}");

            app.Run(async context =>
            {
                if (context.Request.Path.StartsWithSegments(_path, StringComparison.Ordinal))
                {
                    Payload payload;
                    using (var reader = new JsonTextReader(new StreamReader(context.Request.Body))) {
                        payload = _jsonSerializer.Deserialize<Payload>(reader);
                    }

                    using (var response = await RedirectPayload(payload, "http://aspnetcore-backend:8080/ingest/data", expectContinue))
                    {
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            });
        }

        private static HttpClient NextHttpClient()
        {
            return _httpClients[Interlocked.Increment(ref _httpClientCounter) % _httpClients.Length];
        }

        private static async Task<HttpResponseMessage> RedirectPayload(Payload payload, string url, bool? expectContinue) {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                _jsonSerializer.Serialize(writer, payload);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                var m = new HttpRequestMessage(HttpMethod.Post, url);
                m.Content = new StreamContent(stream);
                m.Headers.ExpectContinue = expectContinue;
                return await NextHttpClient().SendAsync(m);
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
