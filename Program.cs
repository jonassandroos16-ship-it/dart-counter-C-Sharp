using dart_counter;
using dart_counter.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<Head>("head::after");

builder.Services.AddSingleton<GameStateService>();
builder.Services.AddSingleton<LocalStorageService>();
builder.Services.AddSingleton<SupabaseSyncService>();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddSingleton<SoundService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
