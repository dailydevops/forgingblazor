using NetEvolve.ForgingBlazor;
using NetEvolve.ForgingBlazor.Logging;

var arguments = args;

if (arguments.Length == 0)
{
    arguments = ["build", "--log-level", "trace"];
}

var builder = ApplicationBuilder.CreateDefaultBuilder(arguments).WithLogging();

_ = builder.AddBlogSegment("posts");

var app = builder.Build();

var exitCode = await app.RunAsync().ConfigureAwait(false);

Environment.ExitCode = exitCode;
