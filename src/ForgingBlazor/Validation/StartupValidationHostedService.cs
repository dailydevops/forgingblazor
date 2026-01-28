namespace NetEvolve.ForgingBlazor.Validation;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetEvolve.ForgingBlazor.Content.Validation;
using NetEvolve.ForgingBlazor.Routing.Validation;
using NetEvolve.ForgingBlazor.Storage.Validation;

/// <summary>
/// Internal hosted service running all validations during <see cref="StartAsync"/> and throwing aggregated exceptions.
/// </summary>
internal sealed partial class StartupValidationHostedService : IHostedService
{
    private readonly ILogger<StartupValidationHostedService> _logger;
    private readonly RoutingConfigurationValidation? _routingValidation;
    private readonly ContentStructureValidation? _contentValidation;
    private readonly StorageConfigurationValidation? _storageValidation;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupValidationHostedService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="routingValidation">The routing configuration validation (optional).</param>
    /// <param name="contentValidation">The content structure validation (optional).</param>
    /// <param name="storageValidation">The storage configuration validation (optional).</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    public StartupValidationHostedService(
        ILogger<StartupValidationHostedService> logger,
        RoutingConfigurationValidation? routingValidation = null,
        ContentStructureValidation? contentValidation = null,
        StorageConfigurationValidation? storageValidation = null
    )
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _routingValidation = routingValidation;
        _contentValidation = contentValidation;
        _storageValidation = storageValidation;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        LogStartingValidation();

        var errors = new List<string>();

        // Validate routing configuration
        if (_routingValidation is not null)
        {
            try
            {
                var routingConfig = new RoutingConfiguration();
                var result = _routingValidation.Validate(null, routingConfig);
                if (result.Failed)
                {
                    errors.Add($"Routing configuration validation failed: {result.FailureMessage}");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Routing configuration validation exception: {ex.Message}");
            }
        }

        // Validate content structure
        if (_contentValidation is not null)
        {
            try
            {
                await _contentValidation.ValidateAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                errors.Add($"Content structure validation exception: {ex.Message}");
            }
        }

        // Validate storage configuration
        if (_storageValidation is not null)
        {
            try
            {
                var storageConfig = new StorageConfiguration();
                var result = _storageValidation.Validate(null, storageConfig);
                if (result.Failed)
                {
                    errors.Add($"Storage configuration validation failed: {result.FailureMessage}");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Storage configuration validation exception: {ex.Message}");
            }
        }

        // If there are any errors, throw an aggregate exception
        if (errors.Count > 0)
        {
            var errorMessage = string.Join(Environment.NewLine, errors);
            LogValidationFailed(Environment.NewLine, errorMessage);
            throw new InvalidOperationException($"Startup validation failed:{Environment.NewLine}{errorMessage}");
        }

        LogValidationCompleted();
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) =>
        // No cleanup needed
        Task.CompletedTask;

    [LoggerMessage(LogLevel.Information, "Starting startup validation...")]
    private partial void LogStartingValidation();

    [LoggerMessage(LogLevel.Error, "Startup validation failed:{NewLine}{Errors}")]
    private partial void LogValidationFailed(string newLine, string errors);

    [LoggerMessage(LogLevel.Information, "Startup validation completed successfully.")]
    private partial void LogValidationCompleted();
}
