namespace NetEvolve.ForgingBlazor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetEvolve.ForgingBlazor.Builders;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;
using NetEvolve.ForgingBlazor.Models;
using NetEvolve.ForgingBlazor.Services;

/// <summary>
/// Provides extension methods for <see cref="IApplicationBuilder"/> to configure default page handling.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a blog segment to the application with the default <see cref="BlogPost"/> type.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance to configure.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="segment">
    /// The URL segment identifier for the blog section.
    /// Cannot be <see langword="null"/> or whitespace.
    /// </param>
    /// <returns>
    /// An <see cref="IBlogSegmentBuilder{BlogPost}"/> instance for further blog configuration.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is a convenience method that calls <see cref="AddBlogSegment{TBlogType}(IApplicationBuilder, string)"/>
    /// with <see cref="BlogPost"/> as the type parameter.
    /// </para>
    /// <para>
    /// The segment parameter defines the URL path component where blog posts will be accessible.
    /// For example, a segment of <c>blog</c> will place blog posts under the <c>/blog/</c> path.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segment"/> is <see langword="null"/> or whitespace.</exception>
    /// <seealso cref="AddBlogSegment{TBlogType}(IApplicationBuilder, string)"/>
    public static IBlogSegmentBuilder<BlogPost> AddBlogSegment(this IApplicationBuilder builder, string segment) =>
        builder.AddBlogSegment<BlogPost>(segment);

    /// <summary>
    /// Adds a blog segment to the application with a custom blog post type.
    /// </summary>
    /// <typeparam name="TBlogType">
    /// The custom blog post type that inherits from <see cref="BlogPostBase"/>.
    /// </typeparam>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance to configure.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="segment">
    /// The URL segment identifier for the blog section.
    /// Cannot be <see langword="null"/> or whitespace.
    /// </param>
    /// <returns>
    /// An <see cref="IBlogSegmentBuilder{TBlogType}"/> instance for further blog configuration.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method configures a blog section in the application with custom blog post types.
    /// The segment parameter defines the URL path component where blog posts will be accessible.
    /// </para>
    /// <para>
    /// Use the returned <see cref="IBlogSegmentBuilder{TBlogType}"/> to add validation rules,
    /// configure pagination, or customize blog behavior.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segment"/> is <see langword="null"/> or whitespace.</exception>
    /// <seealso cref="BlogPostBase"/>
    /// <seealso cref="IBlogSegmentBuilder{TBlogType}"/>
    public static IBlogSegmentBuilder<TBlogType> AddBlogSegment<TBlogType>(
        this IApplicationBuilder builder,
        string segment
    )
        where TBlogType : BlogPostBase
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);

        return new BlogSegmentBuilder<TBlogType>(builder, segment);
    }

    /// <summary>
    /// Configures the application with default pages using the standard <see cref="Page"/> implementation.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance to configure.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="IApplicationBuilder"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is a convenience method that calls <see cref="AddDefaultContent{TPageType}(IApplicationBuilder)"/>
    /// with <see cref="Page"/> as the type parameter, providing a quick way to set up default pages
    /// without specifying a custom page type.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when default pages have already been registered.</exception>
    /// <seealso cref="AddDefaultContent{TPageType}(IApplicationBuilder)"/>
    public static IApplicationBuilder AddDefaultContent(this IApplicationBuilder builder) =>
        builder.AddDefaultContent<Page>();

    /// <summary>
    /// Configures the application with default pages using a custom page type that derives from <see cref="PageBase"/>.
    /// </summary>
    /// <typeparam name="TPageType">
    /// The custom page type that must inherit from <see cref="PageBase"/> and represent the default page implementation.
    /// </typeparam>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance to configure.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="IApplicationBuilder"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method configures the application with default page infrastructure using a custom page type.
    /// It performs the following steps:
    /// <list type="number">
    /// <item><description>Validates that the builder is not <see langword="null"/></description></item>
    /// <item><description>Checks that default pages have not already been registered to prevent duplicates</description></item>
    /// <item><description>Registers all ForgingBlazor core services via <see cref="ServiceCollectionExtensions.AddForgingBlazorServices"/></description></item>
    /// <item><description>Registers the <see cref="DefaultContentMarker"/> singleton to track that defaults are configured</description></item>
    /// <item><description>Registers the <see cref="DefaultContentRegistration{TPageType}"/> for content collection</description></item>
    /// <item><description>Registers the <see cref="MarkdownContentCollector{TPageType}"/> as a keyed singleton service</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Once default pages are registered, you can add additional custom segments using
    /// <see cref="AddSegment{TPageType}(IApplicationBuilder, string)"/> or <see cref="AddBlogSegment{TBlogType}(IApplicationBuilder, string)"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when default pages have already been registered in the service collection.</exception>
    /// <seealso cref="AddDefaultContent(IApplicationBuilder)"/>
    /// <seealso cref="AddSegment{TPageType}(IApplicationBuilder, string)"/>
    /// <seealso cref="ServiceCollectionExtensions.AddForgingBlazorServices"/>
    public static IApplicationBuilder AddDefaultContent<TPageType>(this IApplicationBuilder builder)
        where TPageType : PageBase
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder.Services.IsServiceTypeRegistered<DefaultContentMarker>())
        {
            throw new InvalidOperationException(
                "Default pages have already been registered. Multiple registrations are not allowed."
            );
        }

        _ = builder
            .Services.AddForgingBlazorServices()
            .AddMarkdownServices()
            .AddSingleton<DefaultContentMarker>()
            .AddSingleton<IContentRegistration, DefaultContentRegistration<TPageType>>();

        builder.Services.TryAddKeyedSingleton<IContentCollector, MarkdownContentCollector<TPageType>>(string.Empty);

        return builder;
    }

    /// <summary>
    /// Adds a content segment to the application with the default <see cref="Page"/> type.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance to configure.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="segment">
    /// The URL segment identifier for the content section.
    /// Cannot be <see langword="null"/> or whitespace.
    /// </param>
    /// <returns>
    /// An <see cref="ISegmentBuilder{Page}"/> instance for further segment configuration.
    /// </returns>
    /// <remarks>
    /// This is a convenience method that calls <see cref="AddSegment{TPageType}(IApplicationBuilder, string)"/>
    /// with <see cref="Page"/> as the type parameter.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segment"/> is <see langword="null"/> or whitespace.</exception>
    /// <seealso cref="AddSegment{TPageType}(IApplicationBuilder, string)"/>
    public static ISegmentBuilder<Page> AddSegment(this IApplicationBuilder builder, string segment) =>
        builder.AddSegment<Page>(segment);

    /// <summary>
    /// Adds a content segment to the application with a custom page type.
    /// </summary>
    /// <typeparam name="TPageType">
    /// The custom page type that inherits from <see cref="PageBase"/>.
    /// </typeparam>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance to configure.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="segment">
    /// The URL segment identifier for the content section.
    /// Cannot be <see langword="null"/> or whitespace.
    /// </param>
    /// <returns>
    /// An <see cref="ISegmentBuilder{TPageType}"/> instance for further segment configuration.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method configures a content segment in the application with custom page types.
    /// The segment parameter defines the URL path component where pages will be accessible.
    /// </para>
    /// <para>
    /// Use the returned <see cref="ISegmentBuilder{TPageType}"/> to add validation rules
    /// or customize segment behavior.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segment"/> is <see langword="null"/> or whitespace.</exception>
    /// <seealso cref="PageBase"/>
    /// <seealso cref="ISegmentBuilder{TPageType}"/>
    public static ISegmentBuilder<TPageType> AddSegment<TPageType>(this IApplicationBuilder builder, string segment)
        where TPageType : PageBase
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);

        _ = builder.Services.AddForgingBlazorServices();

        var segmentBuilder = new SegmentBuilder<TPageType>(builder, segment);
        return segmentBuilder;
    }

    /// <summary>
    /// Internal marker class used to track that default content has been registered.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This sealed class implements <see cref="IStartUpMarker"/> to provide a type-safe marker
    /// for tracking that default pages have been configured in the service collection.
    /// </para>
    /// <para>
    /// The marker is registered as a singleton in the service collection by <see cref="AddDefaultContent{TPageType}(IApplicationBuilder)"/>
    /// to prevent duplicate default content registrations. Before registering defaults, the builder
    /// checks if this marker already exists using <see cref="ServiceCollectionExtensions.IsServiceTypeRegistered{T}"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="AddDefaultContent{TPageType}(IApplicationBuilder)"/>
    /// <seealso cref="IStartUpMarker"/>
    private sealed class DefaultContentMarker : IStartUpMarker;
}
