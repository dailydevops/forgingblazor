var builder = DistributedApplication.CreateBuilder(args);

_ = builder.AddProject<Projects.Xample>("xample");

await builder.Build().RunAsync().ConfigureAwait(false);
