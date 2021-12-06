#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#nullable enable

#region Using Directives

using System.Windows;
using DownTube.Common;
using MahApps.Metro.IconPacks;

#endregion

namespace DownTube {
    public partial class MessageBoxWindow {
        readonly QEvent _OnYes, _OnNo;

        public MessageBoxWindow( string Title, string Heading, string Content, PackIconFeatherIconsKind Kind, QAction OnYes, QAction OnNo, QAction OnClose ) {
            InitializeComponent();

            this.Title = Title;
            LabelHeading.Content = Heading;
            LabelContent.Content = Content;
            MessageIcon.Kind = Kind;

            _OnYes = new QEvent(OnYes, OnClose, Close);
            _OnNo = new QEvent(OnNo, OnClose, Close);
        }

        void ButtonYes_Click(object Sender, RoutedEventArgs E) => _OnYes.Invoke();
        void ButtonNo_Click(object Sender, RoutedEventArgs E) => _OnNo.Invoke();
    }
}
