#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

using System.IO;
using Newtonsoft.Json;

#nullable enable

namespace DownTube.Extensions {

    internal interface IJsonCereal { }

    internal static class Json_Extensions {
        public static readonly JsonSerializer Cerealiser = new JsonSerializer {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            DateFormatString = "O",
            NullValueHandling = NullValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        public static bool WriteToFile( this IJsonCereal ? Obj, FileInfo Destination ) {
            using (StreamWriter SW = Destination.CreateText()) { Cerealiser.Serialize(SW, Obj); }

            return false;
        }

        public static T ReadFromFile<T>( this FileInfo Location ) where T : IJsonCereal {
            if (TryReadFromFile(Location, out T D)) { return D; }

            throw new JsonReaderException("Read Failed.");
        }

        public static bool TryReadFromFile<T>( this FileInfo Location, out T Deserialized ) where T : IJsonCereal {
            using (FileStream FS = Location.OpenRead()) {
                using (StreamReader SR = new StreamReader(FS)) {
                    using (JsonTextReader JTR = new JsonTextReader(SR)) {
                        T D = Cerealiser.Deserialize<T>(JTR);
                        if (D != null) {
                            Deserialized = D;
                            return true;
                        }
                    }
                }
            }

            Deserialized = default!;
            return false;
        }

        public static bool TryRead<T>( this string JSON, out T Deserialized ) where T : IJsonCereal {
            using (StringReader SR = new StringReader(JSON)) {
                using (JsonTextReader JTR = new JsonTextReader(SR)) {
                    T D = Cerealiser.Deserialize<T>(JTR);
                    if (D != null) {
                        Deserialized = D;
                        return true;
                    }
                }
            }

            Deserialized = default!;
            return false;
        }
    }
}