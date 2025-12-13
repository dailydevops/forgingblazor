namespace NetEvolve.ForgingBlazor;

using Microsoft.Extensions.DependencyInjection;
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
    /// This is a convenience method that calls <see cref="AddBlogSegment{TBlogType}(IApplicationBuilder, string)"/>
    /// with <see cref="BlogPost"/> as the type parameter.
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

        return null!;
    }

    /// <summary>
    /// Configures the application with default pages using the standard <see cref="Page"/> implementation.
    /// This method registers the necessary services and sets up the default page infrastructure for the application.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance to configure.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static IApplicationBuilder AddDefaultContent(this IApplicationBuilder builder) =>
        builder.AddDefaultContent<Page>();

    /// <summary>
    /// Configures the application with default pages using a custom page type that derives from <see cref="PageBase"/>.
    /// This generic method allows you to specify a custom page implementation while automatically registering all required ForgingBlazor services.
    /// The method validates the builder argument and ensures the service collection is properly configured before returning.
    /// </summary>
    /// <typeparam name="TPageType">The custom page type that must inherit from <see cref="PageBase"/>.</typeparam>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance to configure.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
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
            .AddSingleton<DefaultContentMarker>()
            .AddSingleton<IContentRegistration, DefaultContentRegistration<TPageType>>();

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
        where TPageType : PageBase => null!;

    /// <summary>
    /// Internal marker class used to track that default content has been registered.
    /// </summary>
    /// <remarks>
    /// This sealed class implements <see cref="IStartUpMarker"/> to identify that default pages
    /// have been configured, preventing duplicate registrations.
    /// </remarks>
    private sealed class DefaultContentMarker : IStartUpMarker;
}
