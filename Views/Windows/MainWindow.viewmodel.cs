#region Copyright (C) 2017-2021  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

using System.Collections.ObjectModel;
using System.Windows.Controls;

using DownTube.Views.Pages;

using WPFUI.Common;

namespace DownTube.Views.Windows;

/// <summary>
/// The viewmodel for <see cref="MainWindow"/>.
/// </summary>
public class MainWindow_ViewModel : Window_ViewModel<MainWindow> {

    /// <summary>
    /// Gets the collection of navigation items to display on the sidebar.
    /// </summary>
    /// <value>
    /// The collection of available sidebar navigation items.
    /// </value>
    public ObservableCollection<NavItem> NavItems { get; set; } = new ObservableCollection<NavItem> {
        new NavItem {
            Name = "Search",
            Tag = nameof(SearchPage),
            Type = typeof(SearchPage),
            Icon = Icon.Search48
        }
    };

    /// <summary>
    /// Gets the collection of navigation items to display on the footer.
    /// </summary>
    /// <value>
    /// The collection of available footer navigation items.
    /// </value>
    public ObservableCollection<NavItem> NavFooterItems { get; set; } = new ObservableCollection<NavItem> {
        new NavItem {
            Name = "Settings",
            Tag = nameof(SettingsPage),
            Type = typeof(SettingsPage),
            Icon = Icon.Settings48
        }
    };

    /// <inheritdoc />
    public override Border WindowBGBorder => View.MainBorder;
}