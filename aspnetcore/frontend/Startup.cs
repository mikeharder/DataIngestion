using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Frontend {
    public class Startup {
        private static readonly PathString _path = new PathString("/ingest/event");

        public void Configure(IApplicationBuilder app) {
            app.Run(async context => 
            {
                if (context.Request.Path.StartsWithSegments(_path, StringComparison.Ordinal))
                {
                    var response = $"hello, world";
                    context.Response.ContentLength = response.Length;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(response);
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
}
