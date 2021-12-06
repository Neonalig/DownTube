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
using System.Collections.ObjectModel;
using System.Diagnostics;

using AC = DownTube.Models.AudioChannels;
using AF = DownTube.Models.DASHAudioFlags;
using VC = DownTube.Models.VideoContainer;
using VF = DownTube.Models.DASHVideoFlags;
#endregion

namespace DownTube.Models {

    public readonly struct YouTubeFormat : IEquatable<YouTubeFormat> {
        public readonly Resolution Resolution;
        public readonly VC Container;
        public readonly VF VideoFlags;
        public readonly AF AudioFlags;
        public readonly Bitrate AudioBitrate;
        public readonly AC AudioChannels;

        public override string ToString() => $"{Resolution} {Container} {VideoFlags} {AudioFlags} {AudioBitrate} {AudioChannels}";

        public YouTubeFormat(Resolution Resolution = default, VC Container = default, VF VideoFlags = default, AF AudioFlags = default, Bitrate AudioBitrate = default, AC AudioChannels = default) {
            this.Resolution = Resolution;
            this.Container = Container;
            this.VideoFlags = VideoFlags;
            this.AudioFlags = AudioFlags;
            this.AudioBitrate = AudioBitrate;
            this.AudioChannels = AudioChannels;
        }

        internal static Dictionary<uint, YouTubeFormat> GetFormats(uint Start = 0, uint End = 572) {
            Dictionary<uint, YouTubeFormat> Dict = new Dictionary<uint, YouTubeFormat>();
            for (uint I = Start; I < End; I++) {
                YouTubeFormat ? F = GetFormat(I);
                if (F != null) { Dict.Add(I, F.Value); }
            }

            return Dict;
        }

        // ReSharper disable once InconsistentNaming
        public static ReadOnlyDictionary<uint, YouTubeFormat> ITagFormats = new ReadOnlyDictionary<uint, YouTubeFormat>(GetFormats(0, 572));

        // ReSharper disable once InconsistentNaming
        public static YouTubeFormat GetFormat(uint iTag) {
            //if (iTag != 0) { Debug.WriteLine("getting from tag: " + iTag); }

            switch (iTag) {
                case 571:
                case 402:
                    return new YouTubeFormat(Resolution.UHD8K, VC.MP4, VF.AV1 | VF.HFR);
                case 272:
                    return new YouTubeFormat(Resolution.UHD8K, VC.WebM, VF.VP9 | VF.HFR);
                case 138:
                    return new YouTubeFormat(Resolution.UHD8K, VC.MP4,  VF.H264);
                case 401:
                    return new YouTubeFormat(Resolution.UHD, VC.MP4, VF.AV1 | VF.HFR);
                case 337:
                    return new YouTubeFormat(Resolution.UHD, VC.WebM,  VF.VP92 | VF.HDR | VF.HFR);
                case 315:
                    return new YouTubeFormat(Resolution.UHD, VC.WebM,  VF.VP9 | VF.HFR);
                case 313:
                    return new YouTubeFormat(Resolution.UHD, VC.WebM, VF.VP9);
                case 305:
                    return new YouTubeFormat(Resolution.UHD, VC.MP4,  VF.H264 | VF.HFR);
                case 266:
                    return new YouTubeFormat(Resolution.UHD, VC.MP4,  VF.H264);
                case 400:
                    return new YouTubeFormat(Resolution.QHD, VC.MP4, VF.AV1 | VF.HFR);
                case 336:
                    return new YouTubeFormat(Resolution.QHD, VC.WebM,  VF.VP92 | VF.HDR | VF.HFR);
                case 308:
                    return new YouTubeFormat(Resolution.QHD, VC.WebM,  VF.VP9 | VF.HFR);
                case 271:
                    return new YouTubeFormat(Resolution.QHD, VC.WebM, VF.VP9);
                case 304:
                    return new YouTubeFormat(Resolution.QHD, VC.MP4,  VF.H264 | VF.HFR);
                case 264:
                    return new YouTubeFormat(Resolution.QHD, VC.MP4,  VF.H264);
                case 399:
                    return new YouTubeFormat(Resolution.FHD, VC.MP4, VF.AV1 | VF.HFR);
                case 335:
                    return new YouTubeFormat(Resolution.FHD, VC.WebM,  VF.VP92 | VF.HDR | VF.HFR);
                case 303:
                    return new YouTubeFormat(Resolution.FHD, VC.WebM,  VF.VP9 | VF.HFR);
                case 248:
                    return new YouTubeFormat(Resolution.FHD, VC.WebM, VF.VP9);
                case 299:
                    return new YouTubeFormat(Resolution.FHD, VC.MP4,  VF.H264 | VF.HFR);
                case 137:
                    return new YouTubeFormat(Resolution.FHD, VC.MP4,  VF.H264);
                case 398:
                    return new YouTubeFormat(Resolution.HD, VC.MP4, VF.AV1 | VF.HFR);
                case 334:
                    return new YouTubeFormat(Resolution.HD, VC.WebM,  VF.VP92 | VF.HDR | VF.HFR);
                case 302:
                    return new YouTubeFormat(Resolution.HD, VC.WebM,  VF.VP9 | VF.HFR);
                case 247:
                    return new YouTubeFormat(Resolution.HD, VC.WebM, VF.VP9);
                case 298:
                    return new YouTubeFormat(Resolution.HD, VC.MP4,  VF.H264 | VF.HFR);
                case 136:
                    return new YouTubeFormat(Resolution.HD, VC.MP4,  VF.H264);
                case 397:
                    return new YouTubeFormat(Resolution.SD, VC.MP4, VF.AV1);
                case 333:
                    return new YouTubeFormat(Resolution.SD, VC.WebM,  VF.VP92 | VF.HDR | VF.HFR);
                case 244:
                    return new YouTubeFormat(Resolution.SD, VC.WebM, VF.VP9);
                case 135:
                    return new YouTubeFormat(Resolution.SD, VC.MP4,  VF.H264);
                case 396:
                    return new YouTubeFormat(Resolution.LD, VC.MP4, VF.AV1);
                case 332:
                    return new YouTubeFormat(Resolution.LD, VC.WebM,  VF.VP92 | VF.HDR | VF.HFR);
                case 243:
                    return new YouTubeFormat(Resolution.LD, VC.WebM, VF.VP9);
                case 134:
                    return new YouTubeFormat(Resolution.LD, VC.MP4,  VF.H264);
                case 395:
                    return new YouTubeFormat(Resolution.QVGA, VC.MP4, VF.AV1);
                case 331:
                    return new YouTubeFormat(Resolution.QVGA, VC.WebM,  VF.VP92 | VF.HDR | VF.HFR);
                case 242:
                    return new YouTubeFormat(Resolution.QVGA, VC.WebM, VF.VP9);
                case 133:
                    return new YouTubeFormat(Resolution.QVGA, VC.MP4,  VF.H264);
                case 394:
                    return new YouTubeFormat(Resolution.Mobile, VC.MP4, VF.AV1);
                case 330:
                    return new YouTubeFormat(Resolution.Mobile, VC.WebM,  VF.VP92 | VF.HDR | VF.HFR);
                case 278:
                    return new YouTubeFormat(Resolution.Mobile, VC.WebM, VF.VP9);
                case 160:
                    return new YouTubeFormat(Resolution.Mobile, VC.MP4,  VF.H264);
                case 139:
                    return new YouTubeFormat(Resolution.None, VC.MP4, VF.None, AF.AAC | AF.HEv1, Bitrate.C48K, AC.Stereo);
                case 140:
                    return new YouTubeFormat(Resolution.None, VC.MP4, VF.None, AF.AAC | AF.LC, Bitrate.C128K, AC.Stereo);
                case 141:
                    return new YouTubeFormat(Resolution.None, VC.MP4, VF.None, AF.AAC | AF.LC, Bitrate.C256K, AC.Stereo);
                case 171:
                    return new YouTubeFormat(Resolution.None, VC.WebM, VF.None, AF.Vorbis, Bitrate.C128K, AC.Stereo);
                case 172:
                    return new YouTubeFormat(Resolution.None, VC.WebM, VF.None, AF.Vorbis, Bitrate.C256K, AC.Stereo);
                case 249:
                    return new YouTubeFormat(Resolution.None, VC.WebM, VF.None, AF.Opus, Bitrate.V50K, AC.Stereo);
                case 250:
                    return new YouTubeFormat(Resolution.None, VC.WebM, VF.None, AF.Opus, Bitrate.V70K, AC.Stereo);
                case 251:
                    return new YouTubeFormat(Resolution.None, VC.WebM, VF.None, AF.Opus, Bitrate.V160K, AC.Stereo);
                case 256:
                    return new YouTubeFormat(Resolution.None, VC.MP4, VF.None, AF.AAC | AF.HEv1, Bitrate.C192K, AC.Surround51);
                case 258:
                    return new YouTubeFormat(Resolution.None, VC.MP4, VF.None, AF.AAC | AF.LC, Bitrate.C384K, AC.Surround71);
                case 18:
                    return new YouTubeFormat(Resolution.P360, VC.MP4, VF.H264 | VF.Baseline | VF.L30, AF.AAC | AF.LC, Bitrate.C96K, AC.Stereo);
                case 59:
                    return new YouTubeFormat(Resolution.P480, VC.MP4, VF.H264 | VF.Main | VF.L31, AF.AAC | AF.LC, Bitrate.C128K, AC.Stereo);
                case 22:
                    return new YouTubeFormat(Resolution.P720, VC.MP4, VF.H264 | VF.High | VF.L31, AF.AAC | AF.LC, Bitrate.C192K, AC.Stereo);
                case 37:
                    return new YouTubeFormat(Resolution.P1080, VC.MP4, VF.H264 | VF.High | VF.L40, AF.AAC | AF.LC, Bitrate.C128K, AC.Stereo);
                case 43:
                    return new YouTubeFormat(Resolution.P360, VC.WebM, VF.VP8, AF.Vorbis, Bitrate.C128K, AC.Stereo);
                case 91:
                    return new YouTubeFormat(Resolution.P144, VC.MPEGTSHLS, VF.H264 | VF.Baseline | VF.L11, AF.AAC | AF.HEv1, Bitrate.C48K, AC.Stereo);
                case 92:
                    return new YouTubeFormat(Resolution.P240, VC.MPEGTSHLS, VF.H264 | VF.Main | VF.L21, AF.AAC | AF.LC, Bitrate.C48K, AC.Stereo);
                case 93:
                    return new YouTubeFormat(Resolution.P360, VC.MPEGTSHLS, VF.H264 | VF.Main | VF.L30, AF.AAC | AF.LC, Bitrate.C128K, AC.Stereo);
                case 94:
                    return new YouTubeFormat(Resolution.P480, VC.MPEGTSHLS, VF.H264 | VF.Main | VF.L31, AF.AAC | AF.LC, Bitrate.C128K, AC.Stereo);
                case 95:
                    return new YouTubeFormat(Resolution.P720, VC.MPEGTSHLS, VF.H264 | VF.Main | VF.L31, AF.AAC | AF.LC, Bitrate.C256K, AC.Stereo);
                case 96:
                    return new YouTubeFormat(Resolution.P1080, VC.MPEGTSHLS, VF.H264 | VF.High | VF.L40, AF.AAC | AF.LC, Bitrate.C256K, AC.Stereo);
                case 300:
                    return new YouTubeFormat(Resolution.P720, VC.MPEGTSHLS, VF.H264 | VF.Main | VF.L32 | VF.HFR, AF.AAC | AF.LC, Bitrate.C128K, AC.Stereo);
                case 301:
                    return new YouTubeFormat(Resolution.P1080, VC.MPEGTSHLS, VF.H264 | VF.High | VF.L42 | VF.HFR, AF.AAC | AF.LC, Bitrate.C128K, AC.Stereo);
                default:
                    return new YouTubeFormat(Resolution.None, VC.None, VF.None, AF.None, Bitrate.None, AC.None);
            }
        }

        public override bool Equals( object Obj ) => Obj is YouTubeFormat Format && Equals(Format);
        public bool Equals( YouTubeFormat Other ) => EqualityComparer<Resolution>.Default.Equals(Resolution, Other.Resolution) && Container == Other.Container && VideoFlags == Other.VideoFlags && AudioFlags == Other.AudioFlags && EqualityComparer<Bitrate>.Default.Equals(AudioBitrate, Other.AudioBitrate) && AudioChannels == Other.AudioChannels;

        public override int GetHashCode() {
            int HashCode = -1322838795;
            HashCode = HashCode * -1521134295 + Resolution.GetHashCode();
            HashCode = HashCode * -1521134295 + Container.GetHashCode();
            HashCode = HashCode * -1521134295 + VideoFlags.GetHashCode();
            HashCode = HashCode * -1521134295 + AudioFlags.GetHashCode();
            HashCode = HashCode * -1521134295 + AudioBitrate.GetHashCode();
            HashCode = HashCode * -1521134295 + AudioChannels.GetHashCode();
            return HashCode;
        }

        public static bool operator ==( YouTubeFormat Left, YouTubeFormat Right ) => Left.Equals(Right);
        public static bool operator !=( YouTubeFormat Left, YouTubeFormat Right ) => !(Left == Right);
    }
}