using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;
using Microsoft.AspNetCore.Hosting;
using System.Timers;

namespace IdentityServer.Extensions
{
    public class HeadlessChromeManager
    {
        private readonly IWebHostEnvironment _environment;
        private Browser browser;
        private object _locker;

        public HeadlessChromeManager(IWebHostEnvironment environment)
        {
            _environment = environment;
            _locker = new object();
            browser = null;
        }

        private async Task LaunchBrowserAsync()
        {
            await new BrowserFetcher(options: new BrowserFetcherOptions() { Path = $@"{_environment.ContentRootPath}\.local-chromium" }).DownloadAsync(BrowserFetcher.DefaultRevision);
            var browserTask = Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            browser = await browserTask;

            //每分钟检查一次，如果没有任何打开的标签页就关闭浏览器
            _ = Task.Run(() =>
            {
                var timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
                timer.AutoReset = true;
                timer.Elapsed += async (sender, e) =>
                {
                    var pages = await browser.PagesAsync(); 
                    if (pages.Length == 0)
                    {
                        lock (_locker)
                        {
                            timer.Stop();
                            timer.Dispose();
                            browser.CloseAsync();
                            browser.Dispose();
                            browser = null;

                            return;
                        }
                    }
                };

                timer.Start();
            });
        }

        public Page GetNewPage()
        {
            lock (_locker)
            {
                if (browser == null)
                {
                    LaunchBrowserAsync().Wait();
                    //把启动时默认创建的空白页标签页关掉
                    browser.PagesAsync().Result[0].CloseAsync().Wait();
                }
                return browser.NewPageAsync().Result;
            }
        }
    }
}
