using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;
using PuppeteerSharp.Contrib.Extensions;
using Shouldly;
using Xunit;

namespace BlazorTable.Tests
{
    public class BrowserTests : IAsyncLifetime
    {
        private string BaseAddress;

        private Browser Browser { get; set; }

        public async Task InitializeAsync()
        {
            string filename = "BrowserTestsAddress.config";

            if (File.Exists(filename))
                BaseAddress = File.ReadAllText(filename);
            else
                throw new Exception($"Missing {filename}");

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            Browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
        }

        public async Task DisposeAsync()
        {
            await Browser?.CloseAsync();
        }

        private async Task PrintPerf(Page page)
        {
            var perf = await page.EvaluateExpressionAsync<long>("window.performance.timing.domContentLoadedEventEnd - window.performance.timing.navigationStart");
            Console.WriteLine($"Load Time: {perf}ms");
        }

        [Fact]
        public async Task CheckRoot()
        {
            bool hasError = false;

            var page = await Browser.NewPageAsync();

            page.Console += Page_Console;

            void Page_Console(object sender, ConsoleEventArgs e)
            {
                if (e.Message.Type == ConsoleType.Error)
                    hasError = true;
            }

            await page.GoToAsync(BaseAddress);

            var selector = await page.WaitForSelectorAsync("div.table-responsive > table > tbody > tr:nth-child(1) > td:nth-child(3)");

            (await selector.InnerTextAsync()).ShouldBe("Astrix Mariette");

            hasError.ShouldBeFalse();

            await PrintPerf(page);
        }
    }
}