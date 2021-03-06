#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using MVVMUtils;

using PropertyChanged;

#endregion

namespace DownTube.Views.Controls;

/// <summary>
/// <see cref="UserControl"/> which allows the user to add/remove tags.
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="IComponentConnector" />
public partial class TagViewer : IView<TagViewer_ViewModel> {

	/// <summary>
	/// Initialises a new instance of the <see cref="TagViewer"/> class.
	/// </summary>
	public TagViewer() {
		InitializeComponent();
		VM = new TagViewer_ViewModel();
		DataContext = VM;

		//Below is for testing purposes.
		//KeyDown += ( _, E ) => {
		//	if ( !E.IsDown ) {
		//		return;
		//	}
		//	switch ( E.Key ) {
		//		case System.Windows.Input.Key.E:
		//			CanDelete = !CanDelete;
		//			break;
		//		case System.Windows.Input.Key.R:
		//			CanAdd = !CanAdd;
		//			break;
		//		case System.Windows.Input.Key.T:
		//			CanEdit = !CanEdit;
		//			break;
		//	}
		//};
	}

	/// <inheritdoc cref="TagViewer_ViewModel.RemoveTag(Tag)"/>
	public bool RemoveTag( Tag Tag ) => VM.RemoveTag(Tag);

	/// <inheritdoc cref="TagViewer_ViewModel.RemoveTag(Guid)"/>
	public bool RemoveTag( Guid ID ) => VM.RemoveTag(ID);

	/// <inheritdoc cref="TagViewer_ViewModel.RemoveTag(string)"/>
	public bool RemoveTag( string Name ) => VM.RemoveTag(Name);

	/// <inheritdoc cref="TagViewer_ViewModel.AddTag()"/>
	public void AddTag() => VM.AddTag();

	/// <inheritdoc cref="TagViewer_ViewModel.AddTag(Tag)"/>
	public void AddTag( Tag Tag ) => VM.AddTag(Tag);

	/// <inheritdoc cref="TagViewer_ViewModel.AddTag(Tag)"/>
	/// <param name="ID">The identifier.</param>
	/// <param name="Name">The name of the tag.</param>
	public void AddTag( Guid ID, string Name ) => AddTag(new Tag(ID, Name));

	/// <inheritdoc cref="TagViewer_ViewModel.AddTag(Tag)"/>
	/// <param name="Name">The name of the tag.</param>
	public void AddTag( string Name ) => AddTag(Guid.NewGuid(), Name);

	/// <inheritdoc cref="TagViewer_ViewModel.ClearTags()"/>
	public void ClearTags() => VM.ClearTags();

	/// <summary>
	/// Occurs when the Click <see langword="event"/> is raised.
	/// </summary>
	/// <param name="Sender">The source of the <see langword="event"/>.</param>
	/// <param name="E">The raised <see langword="event"/> arguments.</param>
	void DeleteButton_Click( object Sender, RoutedEventArgs E ) {
		if ( !VM.CanDelete ) { return; }
		if (Sender is FrameworkElement { TemplatedParent: ContentPresenter { Content: Tag Tg } } ) {
			_ = RemoveTag(Tg);
		}
	}

	/// <summary>
	/// Occurs when the Click <see langword="event"/> is raised.
	/// </summary>
	/// <param name="Sender">The source of the <see langword="event"/>.</param>
	/// <param name="E">The raised <see langword="event"/> arguments.</param>
	void AddButton_Click( object Sender, RoutedEventArgs E ) {
		if ( !VM.CanAdd ) { return; }
		AddTag();
	}

	/// <inheritdoc />
	public TagViewer_ViewModel VM { get; }

	/// <inheritdoc cref="TagViewer_ViewModel.CanAdd"/>
	public bool CanAdd {
		get => VM.CanAdd;
		set => VM.CanAdd = value;
	}

	/// <inheritdoc cref="TagViewer_ViewModel.CanDelete"/>
	public bool CanDelete {
		get => VM.CanDelete;
		set => VM.CanDelete = value;
	}

	/// <inheritdoc cref="TagViewer_ViewModel.CanEdit"/>
	public bool CanEdit {
		get => VM.CanEdit;
		set => VM.CanEdit = value;
	}

	/// <inheritdoc cref="TagViewer_ViewModel.DefaultTagText"/>
	public string DefaultTagText {
		get => VM.DefaultTagText;
		set => VM.DefaultTagText = value;
	}

	/// <summary>
	/// Gets the current tags.
	/// </summary>
	/// <value>
	/// The tags collection at the current point in time.
	/// </value>
	public Tag[] Tags => VM.Tags.ToArray();
}

/// <summary>
/// Represents a simple <see cref="string"/> tag and identifier.
/// </summary>
public class Tag : Reactive, IEquatable<Tag> {

	/// <summary>
	/// Gets the identifier.
	/// </summary>
	/// <value>
	/// The identifier.
	/// </value>
	public Guid ID { get; }

	string _Name;
	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public string Name {
		get => _Name;
		set => this.SetAndRaise( ref _Name, value );
	}

	/// <summary>
	/// Initialises a new instance of the <see cref="Tag"/> struct.
	/// </summary>
	/// <param name="ID">The identifier.</param>
	/// <param name="Name">The name.</param>
	public Tag( Guid ID, string Name ) {
		this.ID = ID;
		_Name = Name;
	}

	/// <summary>
	/// Initialises a new instance of the <see cref="Tag"/> struct.
	/// </summary>
	/// <param name="Name">The name.</param>
	public Tag( string Name ) : this(Guid.NewGuid(), Name) { }

	/// <summary>
	/// Initialises a default instance of the <see cref="Tag"/> struct.
	/// </summary>
	public Tag() : this(string.Empty) { }

	/// <summary>
	/// The default (empty) tag.
	/// </summary>
	public static readonly Tag Empty = new Tag();

	#region Converters

	/// <summary>
	/// Performs an <see langword="implicit"/> conversion from <see cref="String"/> to <see cref="Tag"/>.
	/// </summary>
	/// <param name="Name">The name.</param>
	/// <returns>
	/// The result of the conversion.
	/// </returns>
	public static implicit operator Tag( string Name ) => new Tag(Name);

	/// <summary>
	/// Performs an explicit conversion from <see cref="Tag"/> to <see cref="System.String"/>.
	/// </summary>
	/// <param name="Tag">The tag.</param>
	/// <returns>
	/// The result of the conversion.
	/// </returns>
	public static explicit operator string(Tag Tag) => Tag.Name;

	/// <summary>
	/// Performs an explicit conversion from <see cref="Tag"/> to <see cref="Guid"/>.
	/// </summary>
	/// <param name="Tag">The tag.</param>
	/// <returns>
	/// The result of the conversion.
	/// </returns>
	public static explicit operator Guid( Tag Tag ) => Tag.ID;

	#endregion

	#region IEquatable Implementation

	/// <inheritdoc />
	public bool Equals( Tag? Other ) => Other is not null && ID.Equals(Other.ID);

	/// <inheritdoc />
	public override bool Equals( object? Obj ) => Obj is Tag Other && Equals(Other);

	/// <inheritdoc />
	public override int GetHashCode() => ID.GetHashCode();

	/// <summary>
	/// Implements the '==' <see langword="operator"/>.
	/// </summary>
	/// <param name="Left">The left operand.</param>
	/// <param name="Right">The right operand.</param>
	/// <returns>
	/// The result of the <see langword="operator"/>.
	/// </returns>
	public static bool operator ==( Tag Left, Tag Right ) => Left.Equals(Right);

	/// <summary>
	/// Implements the '!=' <see langword="operator"/>.
	/// </summary>
	/// <param name="Left">The left operand.</param>
	/// <param name="Right">The right operand.</param>
	/// <returns>
	/// The result of the <see langword="operator"/>.
	/// </returns>
	public static bool operator !=( Tag Left, Tag Right ) => !Left.Equals(Right);

	#endregion

	public override string ToString() => $"{ID}#{Name}";
}

/// <summary>
/// Viewmodel containing the required data bindings for the relevant view.
/// </summary>
/// <seealso cref="ViewModel{TView}"/>
/// <seealso cref="TagViewer"/>
public class TagViewer_ViewModel : ViewModel<TagViewer> {

	/// <summary>
	/// Gets the tags.
	/// </summary>
	/// <value>
	/// The tags.
	/// </value>
	public ObservableCollection<Tag> Tags { get; } = new ObservableCollection<Tag>();

	/// <summary>
	/// Gets or sets a value indicating whether new tags can be added to the control.
	/// </summary>
	/// <value>
	/// <see langword="true" /> if new tags are allowed to be added by the user; otherwise, <see langword="false" />.
	/// </value>
	public bool CanAdd { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether tags can be deleted from the control.
	/// </summary>
	/// <value>
	/// <see langword="true" /> if tags are allowed to be deleted by the user; otherwise, <see langword="false" />.
	/// </value>
	public bool CanDelete { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether tag names can be edited by the user.
	/// </summary>
	/// <value>
	/// <see langword="true" /> if tags can be edited; otherwise, <see langword="false" />.
	/// </value>
	public bool CanEdit { get; set; } = true;

	/// <summary>
	/// Logical inverse of <see cref="CanEdit"/>.
	/// </summary>
	/// <value>
	/// <see langword="true" /> if tags cannot be edited; otherwise, <see langword="false" />.
	/// </value>
	[System.Windows.Markup.DependsOn(nameof(CanEdit))]
	public bool CanNotEdit => !CanEdit;

	/// <summary>
	/// Gets or sets the default text used when a new tag is added.
	/// </summary>
	/// <remarks>For a new tag to be allowed to be created, <see cref="CanAdd"/> must be set to <see langword="true"/>.</remarks>
	/// <value>
	/// The default text for new tags.
	/// </value>
	public string DefaultTagText { get; set; } = "New Tag";

	#region Events

	/// <summary>
	/// Raised when a <see cref="Tag"/> is changed.
	/// </summary>
	/// <param name="Sender">The sender.</param>
	/// <param name="Changed">The changed tag.</param>
	public delegate void TagChangedEventArgs( TagViewer Sender, Tag Changed );

	/// <summary>
	/// Raised when a <see cref="Tag"/> is added.
	/// </summary>
	/// <param name="Sender">The sender.</param>
	/// <param name="New">The new tag.</param>
	public delegate void TagAddedEventArgs( TagViewer Sender, Tag New );

	/// <summary>
	/// Raised when a <see cref="Tag"/> is removed.
	/// </summary>
	/// <param name="Sender">The sender.</param>
	/// <param name="Removed">The removed tag.</param>
	public delegate void TagRemovedEventArgs( TagViewer Sender, Tag Removed );

	/// <summary>
	/// Raised when all <see cref="Tag"/> instances are removed.
	/// </summary>
	/// <param name="Sender">The sender.</param>
	public delegate void TagsClearedEventArgs( TagViewer Sender );

	/// <summary>
	/// Raised when a <see cref="Tag"/> is changed.
	/// </summary>
	public event TagChangedEventArgs? TagChanged;

	/// <summary>
	/// Invokes the <see cref="TagChanged"/> <see langword="event"/>.
	/// </summary>
	/// <param name="Changed">The changed tag.</param>
	[SuppressPropertyChangedWarnings]
	public void OnTagChanged( Tag Changed ) => TagChanged?.Invoke(View, Changed);

	/// <summary>
	/// Raised when a <see cref="Tag"/> is added.
	/// </summary>
	public event TagAddedEventArgs? TagAdded;

	/// <summary>
	/// Invokes the <see cref="TagAdded"/> <see langword="event"/>.
	/// </summary>
	/// <param name="New">The new tag.</param>
	public void OnTagAdded( Tag New ) => TagAdded?.Invoke(View, New);

	/// <summary>
	/// Raised when a <see cref="Tag"/> is removed.
	/// </summary>
	public event TagRemovedEventArgs? TagRemoved;

	/// <summary>
	/// Invokes the <see cref="TagRemoved"/> <see langword="event"/>.
	/// </summary>
	/// <param name="Removed">The removed tag.</param>
	public void OnTagRemoved( Tag Removed ) => TagRemoved?.Invoke(View, Removed);

	/// <summary>
	/// Raised when all <see cref="Tag"/> instances are removed.
	/// </summary>
	public event TagsClearedEventArgs? TagsCleared;

	/// <summary>
	/// Invokes the <see cref="TagsCleared"/> <see langword="event"/>.
	/// </summary>
	public void OnTagsCleared() => TagsCleared?.Invoke(View);

	#endregion

	/// <summary>
	/// Adds a new tag with the <see cref="DefaultTagText"/>.
	/// </summary>
	/// <exception cref="NotSupportedException">Multiple tags with the same identifier may not exist in the same collection.</exception>
	public void AddTag() => AddTag(new Tag(DefaultTagText));

	/// <summary>
	/// Adds the new tag.
	/// </summary>
	/// <param name="Tag">The new tag.</param>
	/// <exception cref="NotSupportedException">Multiple tags with the same identifier may not exist in the same collection.</exception>
	public void AddTag( Tag Tag ) {
		foreach ( Tag Tg in Tags ) {
			if (Tg.ID == Tag.ID ) {
				throw new NotSupportedException("Multiple tags with the same identifier may not exist in the same collection.");
			}
		}
		Tag.PropertyChanged += OnTagChanged;
		Tags.Add(Tag);

		OnTagAdded(Tag);
	}

	[SuppressPropertyChangedWarnings]
	void OnTagChanged( object? Sender, PropertyChangedEventArgs E ) => OnTagChanged((Sender as Tag).CatchNull());

	/// <summary>
	/// Removes the given tag.
	/// </summary>
	/// <param name="Tag">The tag to remove.</param>
	/// <returns><see langword="true"/> if the tag was successfully found and removed; otherwise <see langword="false"/>.</returns>
	public bool RemoveTag( Tag Tag ) => RemoveTag(Tag.ID);

	/// <summary>
	/// Removes the tag with the given identifier.
	/// </summary>
	/// <param name="ID">The identifier.</param>
	/// <returns><see langword="true"/> if the tag was successfully found and removed; otherwise <see langword="false"/>.</returns>
	public bool RemoveTag( Guid ID ) {
		int I, L = Tags.Count;
		bool Found = false;
		for ( I = 0; I < L; I++ ) {
			if ( Tags[I].ID == ID ) {
				Found = true;
				break;
			}
		}
		if ( Found ) {
			Tag ToRemove = Tags[I];
			ToRemove.PropertyChanged -= OnTagChanged;
			Tags.RemoveAt(I);
			OnTagRemoved(ToRemove);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Removes the tag with the given identifier.
	/// </summary>
	/// <param name="Name">The name of the tag.</param>
	/// <returns><see langword="true"/> if the tag was successfully found and removed; otherwise <see langword="false"/>.</returns>
	public bool RemoveTag( string Name ) {
		int I, L = Tags.Count;
		bool Found = false;
		for ( I = 0; I < L; I++ ) {
			if ( Tags[I].Name == Name ) {
				Found = true;
				break;
			}
		}
		if ( Found ) {
			Tag ToRemove = Tags[I];
			ToRemove.PropertyChanged -= OnTagChanged;
			Tags.RemoveAt(I);
			OnTagRemoved(ToRemove);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Clears the tags collection.
	/// </summary>
	public void ClearTags() {
		foreach( Tag Tag in Tags ) {
			Tag.PropertyChanged -= OnTagChanged;
			OnTagRemoved(Tag);
		}
		Tags.Clear();
		OnTagsCleared();
	}
}