#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Windows;

#endregion

namespace DownTube.Views.Controls;
/// <summary>
/// Interaction logic for UtilityDownloaderWindow_FFmpegLegal.xaml
/// </summary>
public partial class UtilityDownloaderWindow_FFmpegLegal {
    public UtilityDownloaderWindow_FFmpegLegal() {
        InitializeComponent();
    }

    void FFmpegLicenseHyperlink_Click( object Sender, RoutedEventArgs E ) => "https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html".NavigateToWebsite();

    void FFmpegLegalHyperlink_Click( object Sender, RoutedEventArgs E ) => "https://www.ffmpeg.org/legal.html".NavigateToWebsite();
}
