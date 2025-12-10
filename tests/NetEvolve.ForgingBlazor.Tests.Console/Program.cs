using NetEvolve.ForgingBlazor;

var builder = ForgingBlazorApplicationBuilder.CreateBuilder(args);

var app = builder.Build();

var exitCode = await app.RunAsync().ConfigureAwait(false);

Environment.ExitCode = exitCode;
