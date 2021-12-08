using System.Globalization;
using System.Windows.Data;

using ReactiveUI;

namespace DownTube.Converters;

public abstract class AsyncValueConverter<TFrom, TTo> : ReactiveObject, IValueConverter {
    /// <summary>
    /// Converts the data type from <typeparamref name="TFrom"/> to <typeparamref name="TTo"/> asynchronously.
    /// </summary>
    /// <param name="From">The original data.</param>
    /// <param name="Token">The cancellation token.</param>
    /// <returns>An asynchronous task containing the conversion result once awaited.</returns>
    public abstract Task<TTo?> ConvertForwardAsync( TFrom? From, CancellationToken Token = default );

    /// <summary>
    /// Converts the data type from <typeparamref name="TTo"/> to <typeparamref name="TFrom"/> asynchronously.
    /// </summary>
    /// <param name="To">The original data.</param>
    /// <param name="Token">The cancellation token.</param>
    /// <returns>An asynchronous task containing the conversion result once awaited.</returns>
    public virtual Task<TFrom?> ConvertBackwardAsync( TTo? To, CancellationToken Token = default ) => throw new NotImplementedException();

    /// <inheritdoc />
    public object Convert( object Value, Type TargetType, object Parameter, CultureInfo Culture ) {
        if ( !CanConvertForward ) {
            throw new NotSupportedException($"Forward conversions are not supported by {GetType().GetTypeName()}.");
        }
        TFrom? ConvertFrom = Value is TFrom TF ? TF : default;
        Debug.WriteLine($"Converting forward asynchronously with {ConvertFrom?.ToString() ?? "<NULL>"} to {typeof(TTo)}...");
        Task<TTo?> Tk = Task.Run(async() => await ConvertForwardAsync(ConvertFrom));
        return new TaskCompletionNotifier<TTo?>(Tk);
    }

    /// <inheritdoc />
    public object ConvertBack( object Value, Type TargetType, object Parameter, CultureInfo Culture ) {
        if ( !CanConvertBackward ) {
            throw new NotSupportedException($"Backward conversions are not supported by {GetType().GetTypeName()}.");
        }
        TTo? ConvertTo = Value is TTo TT ? TT : default;
        Task<TFrom?> Tk = Task.Run(async() => await ConvertBackwardAsync(ConvertTo));
        return new TaskCompletionNotifier<TFrom?>(Tk);
    }

    /// <summary>
    /// Does this converter support forwards conversion? (from <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>)
    /// </summary>
    public virtual bool CanConvertForward => true;

    /// <summary>
    /// Does this converter support backwards conversion? (from <typeparamref name="TTo"/> to <typeparamref name="TFrom"/>)
    /// </summary>
    public virtual bool CanConvertBackward => false;
}