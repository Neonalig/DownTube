using System.Globalization;
using System.Windows;

namespace DownTube.Converters;

public class NullabilityToVisibilityConverter : ValueConverter<object?, Visibility> {

    /// <summary>
    /// Gets or sets the default value returned.
    /// </summary>
    /// <value>
    /// The default return value.
    /// </value>
    public Visibility DefaultReturn { get; set; } = Visibility.Visible;

    /// <summary>
    /// Gets or sets the value returned when <see langword="null"/>.
    /// </summary>
    /// <value>
    /// The return value when <see langword="null"/>.
    /// </value>
    public Visibility ReturnWhenNull { get; set; } = Visibility.Collapsed;

    /// <inheritdoc />
    public override bool CanForward => true;

    /// <inheritdoc />
    public override bool CanForwardWhenNull => true;

    /// <inheritdoc />
    public override bool CanReverse => false;

    public override bool CanReverseWhenNull => false;

    /// <inheritdoc />
    public override Visibility Forward( object? From, object? Parameter = null, CultureInfo? Culture = null ) => From is null ? ReturnWhenNull : DefaultReturn;

    public override Visibility ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => ReturnWhenNull;

    /// <inheritdoc />
    public override object? Reverse( Visibility To, object? Parameter = null, CultureInfo? Culture = null ) => null;
}
