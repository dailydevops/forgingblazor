using NetEvolve.ForgingBlazor;
using NetEvolve.Xample.Components;

var builder = ForgingBlazorApplication.CreateDefaultBuilder(args);

var app = builder.Build();

await app.RunAsync<App>().ConfigureAwait(false);
