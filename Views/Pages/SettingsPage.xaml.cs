#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace DownTube.Views.Pages; 

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage {
    public SettingsPage() {
        InitializeComponent();
    }
}

/// <summary>
/// Represents a named field for the <see cref="SettingsPage"/>.
/// </summary>
public class SettingsField {
    /// <summary>
    /// Initialises a new instance of the <see cref="SettingsField"/> class.
    /// </summary>
    public SettingsField() : this(string.Empty, string.Empty, new TextBlock { Text = "Error." }) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="SettingsField"/> class.
    /// </summary>
    /// <param name="Name">The setting name.</param>
    /// <param name="ToolTip">The tooltip.</param>
    /// <param name="Content">The displayed content.</param>
    public SettingsField( string Name, string ToolTip, UIElement Content ) {
        this.Name = Name;
        this.ToolTip = ToolTip;
        this.Content = Content;
    }

    /// <summary>
    /// Gets or sets the name of the setting.
    /// </summary>
    /// <value>
    /// The setting name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the tooltip.
    /// </summary>
    /// <value>
    /// The tooltip.
    /// </value>
    public string ToolTip { get; set; }

    /// <summary>
    /// Gets or sets the displayed content.
    /// </summary>
    /// <value>
    /// The displayed content.
    /// </value>
    public UIElement Content { get; set; }
}