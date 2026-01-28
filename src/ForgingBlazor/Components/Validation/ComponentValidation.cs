namespace NetEvolve.ForgingBlazor.Components.Validation;

using System.Reflection;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Validates that content components do not define @page or @layout directives,
/// as required by REQ-CMP-009 and REQ-CMP-010.
/// </summary>
internal static class ComponentValidation
{
    /// <summary>
    /// Validates that a component type does not have @page or @layout directives.
    /// </summary>
    /// <param name="componentType">The component type to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="componentType"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the component has @page or @layout directive.</exception>
    public static void ValidateNoPageOrLayoutDirective(Type componentType)
    {
        ArgumentNullException.ThrowIfNull(componentType);

        if (!typeof(IComponent).IsAssignableFrom(componentType))
        {
            throw new ArgumentException(
                $"Type '{componentType.FullName}' does not implement IComponent.",
                nameof(componentType)
            );
        }

        // Check for RouteAttribute (which represents @page directive)
        if (componentType.GetCustomAttribute<RouteAttribute>() is not null)
        {
            throw new InvalidOperationException(
                $"Content component '{componentType.FullName}' must not define @page directive. "
                    + "Content routes are determined by the routing configuration, not by component attributes. "
                    + "(Requirement: REQ-CMP-009)"
            );
        }

        // Check for LayoutAttribute (which represents @layout directive)
        if (componentType.GetCustomAttribute<LayoutAttribute>() is not null)
        {
            throw new InvalidOperationException(
                $"Content component '{componentType.FullName}' must not define @layout directive. "
                    + "Layouts are specified in the routing configuration, not by component attributes. "
                    + "(Requirement: REQ-CMP-010)"
            );
        }
    }

    /// <summary>
    /// Validates that a component type has a parameter accepting <see cref="ResolvedContent{TDescriptor}"/>.
    /// </summary>
    /// <param name="componentType">The component type to validate.</param>
    /// <returns><see langword="true"/> if the component has a ResolvedContent parameter; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="componentType"/> is null.</exception>
    public static bool HasResolvedContentParameter(Type componentType)
    {
        ArgumentNullException.ThrowIfNull(componentType);

        var properties = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<ParameterAttribute>() is null)
            {
                continue;
            }

            var propertyType = property.PropertyType;

            // Check if the property type is ResolvedContent<T>
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Name == "ResolvedContent`1")
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Validates that a component type implements IComponent.
    /// </summary>
    /// <param name="componentType">The component type to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="componentType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the type does not implement IComponent.</exception>
    public static void ValidateIsComponent(Type componentType)
    {
        ArgumentNullException.ThrowIfNull(componentType);

        if (!typeof(IComponent).IsAssignableFrom(componentType))
        {
            throw new ArgumentException(
                $"Type '{componentType.FullName}' does not implement IComponent (Requirement: REQ-CMP-007).",
                nameof(componentType)
            );
        }
    }

    /// <summary>
    /// Validates that a layout type derives from LayoutComponentBase.
    /// </summary>
    /// <param name="layoutType">The layout type to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="layoutType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the type does not derive from LayoutComponentBase.</exception>
    public static void ValidateIsLayout(Type layoutType)
    {
        ArgumentNullException.ThrowIfNull(layoutType);

        if (!typeof(LayoutComponentBase).IsAssignableFrom(layoutType))
        {
            throw new ArgumentException(
                $"Type '{layoutType.FullName}' does not derive from LayoutComponentBase (Requirement: REQ-CMP-007).",
                nameof(layoutType)
            );
        }
    }
}
