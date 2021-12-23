using System.Globalization;
using System.Windows.Data;

using MVVMUtils;

namespace DownTube.Converters;

/// <summary>
/// Performs a chain conversion on the elements of the given collection.
/// </summary>
/// <typeparam name="TIn">The type of the in.</typeparam>
/// <typeparam name="TOut">The type of the out.</typeparam>
/// <seealso cref="ValueConverter{TFrom, TTo}"/>
[ValueConversion(typeof(IEnumerable<>), typeof(IEnumerable<>))]
public class CollectionInternalConverter<TIn, TOut> : ValueConverter<IEnumerable<TIn?>, IEnumerable<TOut?>> {

	/// <summary>
	/// Gets or sets the converter.
	/// </summary>
	/// <value>
	/// The converter.
	/// </value>
	public ValueConverter<TIn, TOut>? Converter { get; set; }

	/// <inheritdoc />
	public override bool CanReverse => Converter?.CanReverse ?? false;

	/// <inheritdoc />
	public override bool CanReverseWhenNull => Converter?.CanReverseWhenNull ?? false;

	/// <inheritdoc />
	public override bool CanForward => Converter?.CanForward ?? false;

	/// <inheritdoc />
	public override bool CanForwardWhenNull => Converter?.CanForwardWhenNull ?? false;

	/// <inheritdoc />
	public override IEnumerable<TIn?> ReverseWhenNull( object? Parameter = null, CultureInfo? Culture = null ) {
		if ( Converter is null ) { yield break; }
		yield return Converter.ReverseWhenNull(Parameter, Culture);
	}

	/// <inheritdoc />
	public override IEnumerable<TOut?> ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) {
		if ( Converter is null ) { yield break; }
		yield return Converter.ForwardWhenNull(Parameter, Culture);
	}

	/// <inheritdoc />
	public override IEnumerable<TOut?> Forward( IEnumerable<TIn?> From, object? Parameter = null, CultureInfo? Culture = null ) {
		if ( Converter is null ) { yield break; }
		foreach ( TIn? Input in From ) {
			yield return Input switch {
				null => Converter.ForwardWhenNull(Parameter, Culture),
				_    => Converter.Forward(Input, Parameter, Culture)
			};
		}
	}

	/// <inheritdoc />
	public override IEnumerable<TIn?> Reverse( IEnumerable<TOut?> To, object? Parameter = null, CultureInfo? Culture = null ) {
		if ( Converter is null ) { yield break; }
		foreach ( TOut? Output in To ) {
			yield return Output switch {
				null => Converter.ReverseWhenNull(Parameter, Culture),
				_    => Converter.Reverse(Output, Parameter, Culture)
			};
		}
	}
}
