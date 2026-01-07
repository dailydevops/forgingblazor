using NetEvolve.ForgingBlazor;
using NetEvolve.ForgingBlazor.Routing;
using NetEvolve.Xample.Components;
using NetEvolve.Xample.Components.Pages;

var builder = ForgingBlazorApplication
    .CreateDefaultBuilder(args)
    .AddRouting<Home>(root =>
        root.WithAlias("/en", true)
            .WithPagination<Home>(paginationBuilder =>
                paginationBuilder.WithPageSize(10).WithPaginationMode(PaginationMode.Folder)
            )
            .WithPage<LegalNotice>("legal-notice")
    );

var app = builder.Build();

await app.RunAsync<App>().ConfigureAwait(false);
