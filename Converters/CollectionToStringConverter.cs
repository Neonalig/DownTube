using System.Globalization;
using System.Windows.Data;

using MVVMUtils;

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="IEnumerable"/> to <see cref="string"/>.
/// </summary>
/// <seealso cref="ValueConverter{TFrom, TTo}"/>
[ValueConversion(typeof(IEnumerable), typeof(string))]
public class CollectionToStringConverter : ValueConverter<IEnumerable, string> {
	/// <summary>
	/// Gets or sets the prefix.
	/// </summary>
	/// <value>
	/// The prefix.
	/// </value>
	public string Prefix { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the suffix.
	/// </summary>
	/// <value>
	/// The suffix.
	/// </value>
	public string Suffix { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the delimiter.
	/// </summary>
	/// <value>
	/// The delimiter.
	/// </value>
	public string Delimiter { get; set; } = ", ";

	/// <inheritdoc />
	public override bool CanReverse => false;

	/// <inheritdoc />
	public override bool CanForwardWhenNull => true;

	/// <inheritdoc />
	public override string ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => string.Empty;

	/// <inheritdoc />
	public override bool CanReverseWhenNull => true;

	/// <inheritdoc />
	public override IEnumerable? ReverseWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => null;

	/// <inheritdoc />
	public override string Forward( IEnumerable From, object? Parameter = null, CultureInfo? Culture = null ) {
		List<string> Str = new List<string>();
		foreach ( object Item in From ) {
			Str.Add(Item.GetString());
		}
		return string.Join(", ", Str);
	}

	/// <inheritdoc />
	public override IEnumerable Reverse( string To, object? Parameter = null, CultureInfo? Culture = null ) => throw new NotSupportedException();
}
