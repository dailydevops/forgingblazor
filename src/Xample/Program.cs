using NetEvolve.ForgingBlazor;
using NetEvolve.Xample.Components;
using NetEvolve.Xample.Components.Pages;

var builder = ForgingBlazorApplication
    .CreateDefaultBuilder(args)
    .AddRouting<Home>(root => root.WithAlias("/en", true).WithPagination());

var app = builder.Build();

await app.RunAsync<App>().ConfigureAwait(false);
