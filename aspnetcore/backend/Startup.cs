using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Backend {
    public class Startup {
        private static readonly PathString _path = new PathString("/ingest/data");
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public void Configure(IApplicationBuilder app) {
            app.Run(async context => 
            {
                if (context.Request.Path.StartsWithSegments(_path, StringComparison.Ordinal))
                {
                    Payload payload;
                    using (var reader = new JsonTextReader(new StreamReader(context.Request.Body))) {
                        payload = _jsonSerializer.Deserialize<Payload>(reader);
                    }
                }
                else {
                    context.Response.StatusCode = 404;
                }
            });
        }

        public static void Main(string[] args) {
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
