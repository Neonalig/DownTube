#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

#endregion

namespace DownTube.Engine;

/// <summary>
/// Maintains user specified command-line arguments.
/// </summary>
public static class Args {

	static Args() => Init();

	static bool _HasInitialised = false;

	/// <summary>
	/// Initialises the argument parser.
	/// </summary>
	public static void Init() {
		if ( _HasInitialised ) { return; }
		Parse(string.Join(' ', Environment.GetCommandLineArgs().Subset(1))).FireAndForget(); //Parse only a subset of the arguments from arg[1] onwards, skipping arg[0] as that is the path to the application.
		_HasInitialised = true;
	}

	/// <summary>
	/// Gets the known argument definitions.
	/// </summary>
	/// <value>
	/// The known argument definitions.
	/// </value>
	public static readonly ReadOnlyDictionary<string, IArgDefinition> Definitions = new Dictionary<string, IArgDefinition> {
		{ "github", new ToggleArg("github", "g") },
		{ "offline", new ToggleArg("offline", "o") }
	}.GetReadOnly();

	/// <summary>
	/// Gets a value indicating the '--github [(<see langword="true"/>,<see langword="false"/>)]' argument 
	/// </summary>
	/// <remarks>Will always return <see langword="false"/> if <see cref="Offline"/> is set to <see langword="true"/></remarks>
	/// <value>
	/// <see langword="true" /> if GitHub libraries are allowed to be used; otherwise, <see langword="false" />.
	/// </value>
	/// <seealso cref="Offline"/>
	public static bool GitHub => !Offline && GetValue(Definitions["github"], true);

	/// <summary>
	/// Gets a value indicating the '--offline [(<see langword="true"/>,<see langword="false"/>)]' argument 
	/// </summary>
	/// <value>
	/// <see langword="true" /> if the internet is allowed to be used; otherwise, <see langword="false" />.
	/// </value>
	public static bool Offline => GetValue(Definitions["offline"], false);

	/// <summary>
	/// Gets the value, returning <paramref name="WhenNotDefined"/> if the argument has not yet been defined.
	/// </summary>
	/// <typeparam name="T">The value type</typeparam>
	/// <param name="Arg">The argument.</param>
	/// <param name="WhenNotDefined">The when not defined.</param>
	/// <returns>The argument value, or <paramref name="WhenNotDefined"/> if not yet defined.</returns>
	internal static T? GetValue<T>( IArgDefinition<T> Arg, T? WhenNotDefined ) => Arg.IsDefined ? Arg.Value : WhenNotDefined;

	/// <inheritdoc cref="GetValue{T}(IArgDefinition{T}, T?)"/>
	internal static T? GetValue<T>( IArgDefinition Arg, T? WhenNotDefined ) => Arg.IsDefined && Arg is IArgDefinition<T> ArgDef ? ArgDef.Value : WhenNotDefined;

	// ReSharper disable LoopCanBePartlyConvertedToQuery	
	/// <summary>
	/// Parses the specified arguments.
	/// </summary>
	/// <param name="Arguments">The arguments.</param>
	/// <returns></returns>
	public static IEnumerable<IArgDefinition> Parse( string Arguments ) {
		//Debug.WriteLine($"Finding arguments with the given string '{Arguments}'...", "WARNING");
		foreach ( IArgDefinitionMatch Match in GetDefinitions(Arguments) ) {
			//Debug.WriteLine($"\tMatch: {Match.GetType().GetTypeName()}");
			foreach ( IArgDefinition Def in Definitions.Values ) {
				//Debug.WriteLine($"\t\tPossible Def: {Def.GetType().GetTypeName()}");
				if ( Match.IsMatch(Def) ) {
					//Debug.WriteLine("\t\t\tMatch found!");
					Def.Init(Match.Value);
					Debug.WriteLine($"Argument was found: --{Def.Name}{(Def.ShortHand is not null ? $" (-{Def.ShortHand})" : string.Empty)} '{Match.Value}'");
					yield return Def;
					break;
				}
			}
		}
	}
	// ReSharper restore LoopCanBePartlyConvertedToQuery

	static readonly Regex _ArgDefRegex = new Regex("-(?:-(?<Name>[^ -]+)|(?<ShortHand>[^ -]+))(?: (?<Value>[^-]+))?");

	/// <summary>
	/// Gets the definition matches present from the given arguments string.
	/// </summary>
	/// <param name="Arguments">The arguments.</param>
	/// <returns>The present definition matches</returns>
	public static IEnumerable<IArgDefinitionMatch> GetDefinitions( string Arguments ) {
		foreach ( Match M in _ArgDefRegex.Matches(Arguments) ) {
			Group ValueGroup = M.Groups["Value"];
			string? Value = ValueGroup.Success ? ValueGroup.Value : null;

			Group NameGroup = M.Groups["Name"];
			if ( NameGroup.Success ) {
				yield return new ArgDefinitionMatch_FullName(NameGroup.Value, Value);
				continue;
			}

			Group ShortHandGroup = M.Groups["ShortHand"];
			if ( ShortHandGroup.Success ) {
				yield return new ArgDefinitionMatch_ShortHand(ShortHandGroup.Value, Value);
				continue;
			}

			Debug.WriteLine($"Match '{M.Value}' failed when to determine name or shorthand. Malformed?", "WARNING");
		}
	}
}

public interface IArgDefinitionMatch {
	/// <summary>
	/// Gets the matched value.
	/// </summary>
	/// <value>
	/// The matched value.
	/// </value>
	string? Value { get; }

	/// <summary>
	/// Determines whether the specified definition is a match.
	/// </summary>
	/// <param name="Def">The definition.</param>
	/// <returns>
	/// <see langword="true" /> if the specified definition is a match; otherwise, <see langword="false" />.
	/// </returns>
	bool IsMatch( IArgDefinition Def );
}

public class ArgDefinitionMatch_FullName : IArgDefinitionMatch {
	/// <summary>
	/// Gets the name to match.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public string Name { get; }

	/// <inheritdoc />
	public string? Value { get; }

	/// <summary>
	/// Initialises a new instance of the <see cref="ArgDefinitionMatch_FullName"/> class.
	/// </summary>
	/// <param name="Name">The match name.</param>
	/// <param name="Value">The match value.</param>
	public ArgDefinitionMatch_FullName( string Name, string? Value ) {
		this.Name = Name;
		this.Value = Value;
	}

	/// <inheritdoc />
	public bool IsMatch( IArgDefinition Def ) => Name.Equals(Def.Name, StringComparison.InvariantCultureIgnoreCase);
}

public class ArgDefinitionMatch_ShortHand : IArgDefinitionMatch {
	/// <summary>
	/// Gets the shorthand name to match.
	/// </summary>
	/// <value>
	/// The shorthand name.
	/// </value>
	public string ShortHand { get; }

	/// <inheritdoc />
	public string? Value { get; }

	/// <summary>
	/// Initialises a new instance of the <see cref="ArgDefinitionMatch_FullName"/> class.
	/// </summary>
	/// <param name="ShortHand">The match name.</param>
	/// <param name="Value">The match value.</param>
	public ArgDefinitionMatch_ShortHand( string ShortHand, string? Value ) {
		this.ShortHand = ShortHand;
		this.Value = Value;
	}

	/// <inheritdoc />
	public bool IsMatch( IArgDefinition Def ) => Def.ShortHand is not null && ShortHand.Equals(Def.ShortHand, StringComparison.InvariantCultureIgnoreCase);
}

/// <summary>
/// Determines the definition of a known argument.
/// </summary>
public interface IArgDefinition {
	/// <summary>
	/// Gets the name of the argument.
	/// <para/><b>Example:</b>
	/// <example> help </example>
	/// </summary>
	/// <value>
	/// The name of the argument.
	/// </value>
	/// <seealso cref="ShortHand"/>
	string Name { get; }

	/// <summary>
	/// Gets the shorthand name of the argument.
	/// <para/><b>Example:</b>
	/// <example> ? </example>
	/// </summary>
	/// <value>
	/// The shorthand name of the argument.
	/// </value>
	/// <seealso cref="Name"/>
	string? ShortHand { get; }

	/// <summary>
	/// Initialises the argument with the given user input.
	/// </summary>
	/// <param name="Value">The user input.</param>
	void Init( string? Value );

	/// <summary>
	/// Gets a value indicating whether the argument is defined.
	/// </summary>
	/// <value>
	/// <see langword="true" /> if the argument is defined; otherwise, <see langword="false" />.
	/// </value>
	bool IsDefined { get; set; }
}

/// <summary>
/// Determines the definition of a known argument, and it's parsed value.
/// </summary>
/// <typeparam name="T">The parsed value type.</typeparam>
public interface IArgDefinition<T> : IArgDefinition {
	/// <summary>
	/// Gets or sets the parsed value.
	/// </summary>
	/// <value>
	/// The parsed value.
	/// </value>
	T? Value { get; set; }
}

/// <summary>
/// Simple runtime argument definition.
/// </summary>
/// <seealso cref="IArgDefinition" />
public class ArgDefinition : IArgDefinition {
	/// <inheritdoc />
	public string Name { get; }

	/// <inheritdoc />
	public string? ShortHand { get; }

	/// <inheritdoc />
	public void Init( string? Value ) {
		InitAction?.Invoke(this, Value);
		IsDefined = true;
	}

	/// <inheritdoc />
	public bool IsDefined { get; set; }

	/// <summary>
	/// The initialisation action. Ignored when <see langword="null"/>.
	/// </summary>
	Action<ArgDefinition, string?>? InitAction { get; }

	/// <summary>
	/// Initialises a new instance of the <see cref="ArgDefinition"/> class.
	/// </summary>
	/// <param name="Name">The name.</param>
	/// <param name="ShortHand">The shorthand.</param>
	/// <param name="InitAction">The initialisation action, or <see langword="null"/> to ignore it.</param>
	public ArgDefinition( string Name, string? ShortHand, Action<ArgDefinition, string?>? InitAction = null ) {
		this.Name = Name;
		this.ShortHand = ShortHand;
		this.InitAction = InitAction;
	}
}

/// <summary>
/// Simple runtime argument definition.
/// </summary>
/// <seealso cref="IArgDefinition" />
public class ArgDefinition<T> : IArgDefinition<T> {
	/// <inheritdoc />
	public string Name { get; }

	/// <inheritdoc />
	public string? ShortHand { get; }

	/// <inheritdoc />
	public void Init( string? Value ) {
		this.Value = InitAction.Invoke(Value);
		IsDefined = true;
	}

	/// <inheritdoc />
	public bool IsDefined { get; set; }

	/// <summary>
	/// The initialisation action.
	/// </summary>
	Func<string?, T?> InitAction { get; }

	/// <summary>
	/// Initialises a new instance of the <see cref="ArgDefinition"/> class.
	/// </summary>
	/// <param name="Name">The name.</param>
	/// <param name="ShortHand">The shorthand.</param>
	/// <param name="InitAction">The value initialisation action.</param>
	public ArgDefinition( string Name, string? ShortHand, Func<string?, T?> InitAction ) {
		this.Name = Name;
		this.ShortHand = ShortHand;
		this.InitAction = InitAction;
		Value = default;
	}

	/// <inheritdoc />
	public T? Value { get; set; }
}

/// <summary>
/// Simple runtime toggle argument.
/// </summary>
/// <seealso cref="IArgDefinition{T}"/>
public class ToggleArg : IArgDefinition<bool> {
	/// <inheritdoc />
	public string Name { get; }

	/// <inheritdoc />
	public string? ShortHand { get; }

	/// <inheritdoc />
	public bool Value { get; set; }

	/// <inheritdoc />
	public bool IsDefined { get; set; }

	/// <summary>
	/// The default value when no additional value is provided.
	/// </summary>
	public bool DefaultValue { get; }

	/// <inheritdoc />
	public void Init( string? Value ) {
		this.Value = Value is null ? DefaultValue : bool.TryParse(Value, out bool Res) ? Res : DefaultValue;
		IsDefined = true;
	}

	/// <summary>
	/// Initialises a new instance of the <see cref="ToggleArg"/> class.
	/// </summary>
	/// <param name="Name">The name.</param>
	/// <param name="ShortHand">The short hand.</param>
	/// <param name="DefaultValue">If <see langword="true" />, [default value]; otherwise [Else].</param>
	public ToggleArg( string Name, string? ShortHand, bool DefaultValue = true ) {
		this.Name = Name;
		this.ShortHand = ShortHand;
		this.DefaultValue = DefaultValue;
	}
}