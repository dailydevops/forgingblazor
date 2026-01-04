using Microsoft.AspNetCore.Builder;

var app = WebApplication.CreateBuilder(args).Build();
app.MapGet("/", () => "Hello World!");

await app.RunAsync().ConfigureAwait(false);
