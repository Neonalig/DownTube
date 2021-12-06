#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#nullable enable

#region Using Directives

using System;
using System.Collections.Generic;
using DownTube.Extensions;

#endregion

namespace DownTube.Models {
    public readonly struct YV_JP : IJsonCereal {
        public string? Webpage_URL { get; }
        public string? Uploader { get; }
        public string? Title { get; }
        public uint? Duration { get; }
        public YV_F_JP[]? Formats { get; }

        public override string ToString() => $"{Webpage_URL ?? "<NO URL>"} {Uploader ?? "<NO UPLOADER>"} {Title ?? "<NO TITLE>"} {Duration?.ToString() ?? "<NO DURATION>"} {(Formats != null ? "'" + string.Join("', '", Formats) + "'" : "<NO FORMATS>")}";

        public YV_JP( string? Webpage_URL = null, string? Uploader = null, string? Title = null, uint? Duration = null, YV_F_JP[]? Formats = null ) {
            this.Webpage_URL = Webpage_URL;
            this.Uploader = Uploader;
            this.Title = Title;
            this.Duration = Duration;
            this.Formats = Formats;
        }

        public readonly struct YV_F_JP : IJsonCereal {
            public uint? Format_ID { get; }
            public YV_F_JP( uint? Format_ID = null ) => this.Format_ID = Format_ID;
            public override string ToString() => Format_ID?.ToString() ?? "No Format ID";
        }

        public YouTubeVideo GetVideo() {
            YouTubeFormat[] Fs = Array.Empty<YouTubeFormat>();
            if (Formats != null) {
                List<YouTubeFormat> F = new List<YouTubeFormat>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (YV_F_JP Format in Formats) {
                    if (Format.Format_ID.HasValue && Format.Format_ID.Value > 0) {
                        F.Add(YouTubeFormat.GetFormat(Format.Format_ID.Value));
                    }
                }

                Fs = F.ToArray();
            }

            return new YouTubeVideo(Webpage_URL != null ? new Uri(Webpage_URL) : null, Uploader, Title, Duration != null ? TimeSpan.FromSeconds(Duration.Value) : (TimeSpan?)null, Fs);
        }
    }
}