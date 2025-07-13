using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AshborneGame._Core.Globals.Services;

namespace AshborneWASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Detect environment and configure base URL
            bool isGithubPages = builder.HostEnvironment.BaseAddress.Contains("github.io");
            
            // Register environment configuration
            builder.Services.AddSingleton(new AppEnvironment
            {
                IsGithubPages = isGithubPages,
                BaseApiUrl = isGithubPages
                    ? "https://halfcomplete.github.io/Ashborne/"
                    : builder.HostEnvironment.BaseAddress
            });

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            await builder.Build().RunAsync();
        }
    }
}
