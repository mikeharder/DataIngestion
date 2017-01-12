using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Frontend {
    public class Startup
    {
        private static readonly PathString _path = new PathString("/ingest/event");
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context => 
            {
                if (context.Request.Path.StartsWithSegments(_path, StringComparison.Ordinal))
                {
                    Payload payload;
                    using (var reader = new JsonTextReader(new StreamReader(context.Request.Body))) {
                        payload = _jsonSerializer.Deserialize<Payload>(reader);
                    }

                    using (var response = await RedirectPayload(payload, "http://aspnetcore-backend:8080/ingest/data"))
                    {
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            });
        }

        private static async Task<HttpResponseMessage> RedirectPayload(Payload payload, string url) {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                _jsonSerializer.Serialize(writer, payload);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                
                return await _httpClient.PostAsync(url, new StreamContent(stream));
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
