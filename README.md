# Dart Wars (C# / Blazor)

A C# Blazor WebAssembly port of the Dart Wars dart counter app.

## Tech

- C# / .NET 8
- Blazor WebAssembly (WASM)
- Supabase for cloud sync
- Deployed to GitHub Pages

## Build

```bash
dotnet build
dotnet run
```

## Deploy

Pushes to `main` trigger a GitHub Actions workflow that builds the project and deploys the `wwwroot` output to GitHub Pages.
