using NetEvolve.ForgingBlazor;

var arguments = args;

if (arguments.Length == 0)
{
    arguments = ["build"];
}

var builder = ForgingBlazorApplicationBuilder.CreateDefaultBuilder(arguments);

var app = builder.Build();

var exitCode = await app.RunAsync().ConfigureAwait(false);

Environment.ExitCode = exitCode;
