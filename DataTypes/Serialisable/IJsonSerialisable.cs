#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.IO;

using Newtonsoft.Json;

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents a dataform which supports json (de/)serialisation.
/// </summary>
public interface IJsonSerialisable {
    /// <summary>
    /// Deserialises the json data supplied by the <paramref name="Reader"/>, utilising the specified serialiser.
    /// </summary>
    /// <param name="Serialiser">The json data serialiser.</param>
    /// <param name="Reader">The json data provider.</param>
    /// <returns>A deserialised object, or <see langword="null"/>.</returns>
    static object? Deserialise( JsonSerializer Serialiser, JsonReader Reader ) => Serialiser.Deserialize(Reader);

    /// <summary>
    /// Deserialises the json data supplied by the <paramref name="File"/>'s contained json data, utilising the specified serialiser.
    /// </summary>
    /// <param name="Serialiser">The json data serialiser.</param>
    /// <param name="File">The file path containing the json data to deserialise.</param>
    /// <returns>A deserialised object, or <see langword="null"/>.</returns>
    static object? Deserialise( JsonSerializer Serialiser, FileInfo File ) {
        using ( FileStream FS = File.OpenRead() ) {
            using (StreamReader SR = new StreamReader(FS) ) {
                using (JsonTextReader JTR = new JsonTextReader(SR) ) {
                    return Serialiser.Deserialize(JTR);
                }
            }
        }
    }

    /// <summary>
    /// Asynchronously deserialises the json data supplied by the <paramref name="File"/>'s contained json data, utilising the specified serialiser.
    /// </summary>
    /// <param name="Serialiser">The json data serialiser.</param>
    /// <param name="File">The file path containing the json data to deserialise.</param>
    /// <param name="Token">The cancellation token used to cancel deserialisation.</param>
    /// <returns>A deserialised object, or <see langword="null"/>.</returns>
    static async Task<object?> DeserialiseAsync( JsonSerializer Serialiser, FileInfo File, CancellationToken Token = default ) {
        string Text = await System.IO.File.ReadAllTextAsync(File.FullName, Token);
        using ( StreamReader SR = new StreamReader(Text) ) {
            using (JsonTextReader JTR = new JsonTextReader(SR) ) {
                return Serialiser.Deserialize(JTR);
            }
        }
    }

    /// <summary>
    /// Serialises the json data to the supplied <paramref name="Writer"/>, utilising the specified serialiser.
    /// </summary>
    /// <param name="Serialiser">The json data serialiser.</param>
    /// <param name="Writer">The json data writing destination.</param>
    /// <param name="Data">The data to serialise.</param>
    static void Serialise( JsonSerializer Serialiser, JsonWriter Writer, object Data ) => Serialiser.Serialize(Writer, Data);

    /// <summary>
    /// Serialises the json data to the supplied <paramref name="Destination"/> file path, utilising the specified serialiser.
    /// </summary>
    /// <param name="Serialiser">The json data serialiser.</param>
    /// <param name="Destination">The file to truncate (ensure size is zero bytes) and append the serialised json data to.</param>
    /// <param name="Data">The data to serialise.</param>
    static void Serialise( JsonSerializer Serialiser, FileInfo Destination, object Data ) {
        using ( FileStream FS = Destination.Open(FileMode.Truncate, FileAccess.Write) ) {
            using ( StreamWriter SW = new StreamWriter(FS) ) {
                Serialiser.Serialize(SW, Data);
            }
        }
    }
    /// <summary>
    /// Asynchronously serialises the json data to the supplied <paramref name="Destination"/> file path, utilising the specified serialiser.
    /// </summary>
    /// <param name="Serialiser">The json data serialiser.</param>
    /// <param name="Destination">The file to truncate (ensure size is zero bytes) and append the serialised json data to.</param>
    /// <param name="Data">The data to serialise.</param>
    /// <param name="Token">The cancellation token used to cancel serialisation.</param>
    static async Task SerialiseAsync( JsonSerializer Serialiser, FileInfo Destination, object Data, CancellationToken Token = default ) {
        if ( Token.IsCancellationRequested ) { return; }
        //await using ( FileStream FS = Destination.Open(FileMode.Truncate, FileAccess.Write) ) {
        await using ( FileStream FS = File.Open(Destination.FullName, FileMode.Open) ) {
            if ( Token.IsCancellationRequested ) { return; }
            FS.SetLength(0);
            if ( Token.IsCancellationRequested ) { return; }
            await using (StreamWriter SW = new StreamWriter(FS) ) {
                if ( Token.IsCancellationRequested ) { return; }
                Serialiser.Serialize(SW, Data);
            }
        }
    }
}