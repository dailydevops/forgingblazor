using NetEvolve.ForgingBlazor;

var builder = ForgingBlazorApplicationBuilder.CreateDefaultBuilder(args);

var app = builder.Build();

var exitCode = await app.RunAsync().ConfigureAwait(false);

Environment.ExitCode = exitCode;
