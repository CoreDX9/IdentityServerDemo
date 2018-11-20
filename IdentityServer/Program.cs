#if DEBUG

#else

using System.IO;
using System.Text;

#endif

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();//.Run();
            SeedData.EnsureSeedData(host.Services);
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>();

#if DEBUG

#else

            var dir = Directory.GetCurrentDirectory();
            var urls = File.ReadAllText(dir + "/urls.txt", Encoding.UTF8);
            host.UseUrls(urls);

#endif

            return host;
        }
    }
}
