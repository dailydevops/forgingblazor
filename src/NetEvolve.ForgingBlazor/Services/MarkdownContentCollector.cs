namespace NetEvolve.ForgingBlazor.Services;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetEvolve.ForgingBlazor.Configurations;
using NetEvolve.ForgingBlazor.Extensibility;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;
using YamlDotNet.Serialization;

/// <summary>
/// Provides the default implementation of <see cref="IContentCollector"/> for collecting markdown-based content in ForgingBlazor applications.
/// </summary>
/// <typeparam name="TPageType">
/// The page type that inherits from <see cref="PageBase"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// This sealed partial class implements content collection from markdown files with YAML front matter.
/// It parses markdown files, extracts front matter metadata, and registers the processed content with the content register service.
/// </para>
/// <para>
/// The class uses Markdig for markdown parsing and YamlDotNet for YAML deserialization.
/// All markdown files (*.md) in the specified segment directory are processed asynchronously.
/// </para>
/// </remarks>
/// <seealso cref="IContentCollector"/>
/// <seealso cref="PageBase"/>
internal sealed partial class MarkdownContentCollector<TPageType> : IContentCollector
    where TPageType : PageBase
{
    /// <summary>
    /// Stores the logger instance for logging collection operations.
    /// </summary>
    /// <remarks>
    /// This field holds the <see cref="ILogger{TCategoryName}"/> instance used for recording diagnostic messages
    /// during markdown content collection. It is automatically populated by the logging source generator framework
    /// to enable structured logging through the <see cref="LoggerMessageAttribute"/> methods.
    /// </remarks>
    private readonly ILogger<MarkdownContentCollector<TPageType>> _logger;

    /// <summary>
    /// Stores the options monitor for accessing current site configuration settings.
    /// </summary>
    /// <remarks>
    /// This field holds the <see cref="IOptionsMonitor{TOptions}"/> instance that provides access to the current
    /// site configuration. The options monitor allows the content collector to access configuration settings
    /// needed during the markdown content collection process.
    /// </remarks>
    private readonly IOptionsMonitor<SiteConfiguration> _optionsMonitorSiteConfiguration;

    /// <summary>
    /// Stores the markdown pipeline instance configured for markdown parsing with extensions including YAML front matter support.
    /// </summary>
    /// <remarks>
    /// This field holds the <see cref="MarkdownPipeline"/> that was configured with advanced extensions and YAML front matter support.
    /// The pipeline is used during markdown file processing to parse content and extract front matter.
    /// </remarks>
    private readonly MarkdownPipeline _pipeline;

    /// <summary>
    /// Stores the YAML deserializer instance for converting YAML front matter into strongly-typed page objects.
    /// </summary>
    /// <remarks>
    /// This field holds the <see cref="IDeserializer"/> that is configured to deserialize YAML content
    /// into page instances of type <typeparamref name="TPageType"/>.
    /// </remarks>
    private readonly IDeserializer _deserializer;

    /// <summary>
    /// Gets the priority of this content collector in the collection pipeline.
    /// </summary>
    /// <value>
    /// Returns <see cref="int.MaxValue"/>, indicating this collector has the highest priority
    /// and should be executed first among registered content collectors.
    /// </value>
    public int Priority => int.MaxValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownContentCollector{TPageType}"/> class with the specified dependencies.
    /// </summary>
    /// <param name="logger">
    /// The <see cref="ILogger{TCategoryName}"/> instance for logging collection operations.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="optionsMonitorSiteConfiguration">
    /// The <see cref="IOptionsMonitor{TOptions}"/> for accessing current site configuration settings.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="pipeline">
    /// The <see cref="MarkdownPipeline"/> instance configured for markdown parsing with extensions.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="deserializer">
    /// The <see cref="IDeserializer"/> instance for deserializing YAML front matter into page objects.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <remarks>
    /// This constructor stores references to the provided dependencies for use during content collection.
    /// The pipeline and deserializer are used to process markdown files with YAML front matter.
    /// </remarks>
    public MarkdownContentCollector(
        ILogger<MarkdownContentCollector<TPageType>> logger,
        IOptionsMonitor<SiteConfiguration> optionsMonitorSiteConfiguration,
        MarkdownPipeline pipeline,
        IDeserializer deserializer
    )
    {
        _logger = logger;
        _optionsMonitorSiteConfiguration = optionsMonitorSiteConfiguration;
        _pipeline = pipeline;
        _deserializer = deserializer;
    }

    /// <summary>
    /// Asynchronously collects markdown content from the segment directory and registers it with the content register.
    /// </summary>
    /// <param name="contentRegister">
    /// The <see cref="IContentRegister"/> instance to register collected content with.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="registration">
    /// The <see cref="IContentRegistration"/> instance containing configuration for the content segment.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to request cancellation of the collection operation.
    /// Defaults to <see cref="CancellationToken.None"/> if not specified.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> that represents the asynchronous content collection operation.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs the following steps:
    /// <list type="number">
    /// <item><description>Retrieves the content directory path from the site configuration</description></item>
    /// <item><description>Matches all markdown files (*.md) in the segment directory</description></item>
    /// <item><description>Processes each markdown file asynchronously to extract YAML front matter and content</description></item>
    /// <item><description>Registers processed pages with the content register</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Markdown files must contain YAML front matter at the beginning (delimited by ---) with properties
    /// that match the specified <typeparamref name="TPageType"/>. The remaining content after the front matter
    /// is stored as the page content.
    /// </para>
    /// <para>
    /// Files named _index.md are treated as index pages, and an _index.md file in the root segment directory
    /// is treated as the home page.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="contentRegister"/> or <paramref name="registration"/> is <see langword="null"/>.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the segment directory does not exist.</exception>
    public async ValueTask CollectAsync(
        IContentRegister contentRegister,
        IContentRegistration registration,
        CancellationToken cancellationToken
    )
    {
        var siteConfiguration = _optionsMonitorSiteConfiguration.CurrentValue;

        var contentDirectory = new DirectoryInfo(siteConfiguration.ContentPath ?? DefaultPaths.Content);
        var contentPath = Path.Combine(contentDirectory.FullName, registration.Segment);
        var segmentDirectory = new DirectoryInfo(contentPath);

        if (!segmentDirectory.Exists)
        {
            throw new DirectoryNotFoundException(
                $"The content directory '{segmentDirectory.FullName}' does not exist."
            );
        }

        var matcher = new Matcher().AddInclude("**/*.md");

        if (registration.ExcludePaths?.Any() == true)
        {
            foreach (var excludePath in registration.ExcludePaths)
            {
                _ = matcher.AddExclude($"{excludePath}/**");
            }
        }

        var result = matcher.Execute(new DirectoryInfoWrapper(segmentDirectory));

        if (!result.HasMatches)
        {
            LogNoItemsFound(registration.Segment);
            return;
        }

        LogCollectedItems(registration.Segment, result.Files.Count());

        await Parallel
            .ForEachAsync(
                result.Files.Select(file => new FileInfo(Path.Combine(segmentDirectory.FullName, file.Path))),
                cancellationToken,
                async (fileInfo, token) =>
                {
                    ArgumentNullException.ThrowIfNull(fileInfo);

                    try
                    {
                        // MARTIN: Get Encoding from Settings
                        var rawContent = await File.ReadAllTextAsync(fileInfo.FullName, Encoding.UTF8, token)
                            .ConfigureAwait(false);
                        var markdownDocument = Markdown.Parse(rawContent, _pipeline);

                        var frontMatterBlock = markdownDocument.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
                        if (frontMatterBlock is null)
                        {
                            LogFileMissingFrontMatter(fileInfo.FullName);
                            return;
                        }

                        var frontMatterLines = frontMatterBlock.Lines.ToString();
                        var pageContent = _deserializer.Deserialize<TPageType>(frontMatterLines);

                        var frontMatterEnd = rawContent.IndexOf(
                            "---",
                            frontMatterBlock.Span.End - 4,
                            StringComparison.Ordinal
                        );

                        pageContent.Content = rawContent[(frontMatterEnd + 3)..].TrimStart();

                        var isIndexPage = fileInfo.Name.Equals("_index.md", StringComparison.OrdinalIgnoreCase);
                        pageContent.IsIndexPage = isIndexPage;
                        pageContent.IsHomePage =
                            isIndexPage
                            && fileInfo.Directory!.FullName.Equals(
                                segmentDirectory.FullName,
                                StringComparison.OrdinalIgnoreCase
                            );

                        pageContent.RelativeLink = Path.GetRelativePath(
                            contentDirectory.FullName,
                            fileInfo.DirectoryName!
                        );

                        contentRegister.Register(pageContent);
                    }
                    catch (Exception ex)
                    {
                        LogErrorProcessingFile(fileInfo.FullName, ex);
                    }
                }
            )
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Logs that a specific number of items have been collected from a segment.
    /// </summary>
    /// <param name="segment">
    /// The name or identifier of the content segment being collected (e.g., <c>blog</c>, <c>docs</c>).
    /// </param>
    /// <param name="count">
    /// The number of items (markdown files) collected from the segment.
    /// </param>
    /// <remarks>
    /// <para>
    /// This is a logging method generated by the logging source generator via the
    /// <see cref="LoggerMessageAttribute"/> annotation.
    /// </para>
    /// <para>
    /// It logs at <see cref="LogLevel.Debug"/> level with <c>EventId = 0</c>.
    /// </para>
    /// </remarks>
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Collected {Count} items from segment '{Segment}'.")]
    private partial void LogCollectedItems(string segment, int count);

    /// <summary>
    /// Logs that no items were found in a segment.
    /// </summary>
    /// <param name="segment">
    /// The name or identifier of the content segment that was empty (e.g., <c>blog</c>, <c>docs</c>).
    /// </param>
    /// <remarks>
    /// <para>
    /// This is a logging method generated by the logging source generator via the
    /// <see cref="LoggerMessageAttribute"/> annotation.
    /// </para>
    /// <para>
    /// It logs at <see cref="LogLevel.Warning"/> level with <c>EventId = 1</c>.
    /// </para>
    /// </remarks>
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "No items found in segment '{Segment}'.")]
    private partial void LogNoItemsFound(string segment);

    /// <summary>
    /// Logs that a file does not contain valid YAML front matter and will be skipped.
    /// </summary>
    /// <param name="filePath">
    /// The full file system path to the file that is missing valid front matter.
    /// </param>
    /// <remarks>
    /// <para>
    /// This is a logging method generated by the logging source generator via the
    /// <see cref="LoggerMessageAttribute"/> annotation.
    /// </para>
    /// <para>
    /// It logs at <see cref="LogLevel.Warning"/> level with <c>EventId = 2</c>.
    /// </para>
    /// </remarks>
    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "File '{FilePath}' does not contain valid YAML front matter and will be skipped."
    )]
    private partial void LogFileMissingFrontMatter(string filePath);

    /// <summary>
    /// Logs that an error occurred while processing a specific file.
    /// </summary>
    /// <param name="filePath">
    /// The full file system path to the file that encountered an error.
    /// </param>
    /// <param name="ex">
    /// The <see cref="Exception"/> that occurred during file processing.
    /// </param>
    /// <remarks>
    /// <para>
    /// This is a logging method generated by the logging source generator via the
    /// <see cref="LoggerMessageAttribute"/> annotation.
    /// </para>
    /// <para>
    /// It logs at <see cref="LogLevel.Error"/> level with <c>EventId = 3</c> and includes exception details.
    /// </para>
    /// </remarks>
    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "An error occurred while processing the file '{FilePath}'."
    )]
    private partial void LogErrorProcessingFile(string filePath, Exception ex);
}
