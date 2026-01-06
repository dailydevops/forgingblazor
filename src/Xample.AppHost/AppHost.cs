using NetEvolve.Xample.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

_ = builder.AddProject<Projects.Xample>(ProjectNames.Xample);

await builder.Build().RunAsync().ConfigureAwait(false);
