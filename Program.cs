using dart_counter;
using dart_counter.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<Head>("head::after");

builder.Services.AddSingleton<GameStateService>();
builder.Services.AddSingleton<LocalStorageService>();
builder.Services.AddSingleton<SupabaseSyncService>();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddSingleton<SoundService>();

await builder.Build().RunAsync();
