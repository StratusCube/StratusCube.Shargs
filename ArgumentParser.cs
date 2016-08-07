using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratusCube.Shargs {

	/// <summary>
	/// Argument parser responsible for storing, mapping, and parsing arguments.
	/// </summary>
	public class ArgumentParser {

		/// <summary>
		/// The switch to switch on and the help documentation to follow. Switches have matching arguments.
		/// </summary>
		private Dictionary<string , string> _Switches { get; set; }

		/// <summary>
		/// A copy of Switches that, if modified, will not effect the ArgumentParser
		/// </summary>
		public Dictionary<string , string> Switches => _Switches;

        private Dictionary<string , Type> _SwitchTypes { get; set; }

		/// <summary>
		/// The alias and the corresponding switch value.
		/// </summary>
		private Dictionary<string , string> _Aliases { get; set; }

		/// <summary>
		/// A copy of Aliases that, if modified, will not effect the ArgumentParser
		/// </summary>
		public Dictionary<string , string> Aliases => _Aliases;

		/// <summary>
		/// The flags for the program, flags do not have attached arguments.
		/// </summary>
		private Dictionary<string , string> _Flags { get; set; }

		/// <summary>
		/// A copy of Flags that, if modified, will not effect the ArgumentParser
		/// </summary>
		public Dictionary<string , string> Flags => _Flags;

		/// <summary>
		/// The Arguments to parse through
		/// </summary>
		private string[] Args { get; set; }

		/// <summary>
		/// The title for the program that will be displayed in help
		/// </summary>
		private string ProgramTitle { get; set; }

		/// <summary>
		/// The description will be perpended. 
		/// </summary>
		private string HelpDescription { get; set; }

		/// <summary>
		/// The default constructor for setting up the ArgumetParser
		/// </summary>
		/// <param name="args">The arguments to parse through.</param>
		/// <param name="switches">The switches used for parsing.</param>
		/// <param name="aliases">The aliases to use with the switches that match up.</param>
		public ArgumentParser(string[] args ,
			Dictionary<string , string> switches ,
			Dictionary<string , string> flags ,
			Dictionary<string , string> aliases) {

			this.Args = args;
			this._Switches = switches;
			this._Aliases = aliases;
			this._Flags = flags;
			this.InitializeHelp();
		}

		/// <summary>
		/// Constructor for setting up only the arguments.
		/// </summary>
		/// <param name="args">The arguments used for parsing</param>
		public ArgumentParser(string[] args) {
			this.Initialize(args);
		}

		/// <summary>
		/// Constructor for initializing the argument parser with a program title and description
		/// </summary>
		/// <param name="args">The arguments to parse</param>
		/// <param name="programTitle">The title for the program</param>
		/// <param name="helpDescription">The description for the program.</param>
		public ArgumentParser(string[] args , string programTitle , string helpDescription) {
			this.Initialize(args);
			this.ProgramTitle = programTitle;
			this.HelpDescription = helpDescription;
		}

		/// <summary>
		/// Initializes the program with default values.
		/// </summary>
		/// <param name="args">The argument to parse</param>
		private void Initialize(string[] args) {
			this.Args = args;
			this._Switches = new Dictionary<string , string>();
			this._Flags = new Dictionary<string , string>();
			this._Aliases = new Dictionary<string , string>();
			this.InitializeHelp();
		}

		/// <summary>
		/// Initializes the default help switches.
		/// </summary>
		private void InitializeHelp() {
			this.CreateSwitch("-h" , "Presents help documentation.");
			this.MapAlias("--help" , "-h");
		}

		/// <summary>
		/// Adds an alias to watch for in the argument parser.
		/// </summary>
		/// <param name="alias">Alias to watch</param>
		/// <param name="matchingSwitch">The switch behavior that is mapped to the alias.</param>
		public ArgumentParser MapAlias(string alias , string matchingSwitchOrFlag) {
			if (this._Switches.Keys.Any(s => s == matchingSwitchOrFlag) ||
				this._Flags.Keys.Any(s => s == matchingSwitchOrFlag))

				this._Aliases.Add(alias , matchingSwitchOrFlag);

			else
				throw new InvalidOperationException(
					$"Cannot assign alias to a non-existent switch or flag {matchingSwitchOrFlag}"
                );

            return this;
		}

		/// <summary>
		/// Maps a collection of aliases to a switch or flag
		/// </summary>
		/// <param name="aliases">The collection of aliases to map</param>
		/// <param name="matchingSwitchOrFlag">The switch or flag to map the aliases to</param>
		public ArgumentParser MapAliases<T>(T aliases , string matchingSwitchOrFlag) where T : ICollection<string> {
            aliases.ToList().ForEach(a => this.MapAlias(a , matchingSwitchOrFlag));
            return this;
        }

        /// <summary>
        /// Creates a new switch to watch for when parsing arguments
        /// </summary>
        /// <param name="sw">The switch to create Example: '-e'</param>
        /// <param name="help">the help documentation Example: 'this is an example switch'</param>
        public ArgumentParser CreateSwitch(string sw , string help) { 
            this._Switches.Add(sw , help);
            return this;
        }

        /// <summary>
        /// Creates a new flag to watch. Flags do not pair with arguments.
        /// </summary>
        /// <param name="flag">The flag to create. Example: '-f'</param>
        /// <param name="help">The help documentation. Example: 'this is a flag that does not have matching arguments.'</param>
        public ArgumentParser CreateFlag(string flag , string help) {
            this._Flags.Add(flag , help);
            return this;
        }

        /// <summary>
        /// Creates a set from flags with help documentation.
        /// </summary>
        /// <param name="flags">A dictionary where the key is the flag (not alias) and the value is help documentation.</param>
        /// <returns></returns>
        public ArgumentParser CreateFlags(Dictionary<string , string> flags) {
            this._Flags = this._Flags.Concat(flags).ToDictionary(i => i.Key, i=> i.Value);

            return this;
        }

        /// <summary>
        /// Creates a set from flags with help documentation.
        /// </summary>
        /// <param name="switches">A dictionary where the key is the switch (not alias) and the value is help documentation.</param>
        /// <returns></returns>
        public ArgumentParser CreateSwitches(Dictionary<string , string> switches) {
            this._Switches = this._Switches.Concat(switches).ToDictionary(i => i.Key , i => i.Value);

            return this;
        }

        /// <summary>
        /// Parses an argument for a given switch
        /// </summary>
        /// <param name="sw">The argument to parse for the provided switch</param>
        /// <returns>Returns the argument for the switch.</returns>
        public string GetArgument(string sw) {
			if (!this.IsSwitch(sw)) return null;

			foreach (var a in this._Aliases) {
				if (a.Value == sw && this.ArgsContain(a.Key)) {
					return this.Args[Array.FindIndex(this.Args , o => o == a.Key) + 1];
				}
			}

			return this.Args[Array.FindIndex(this.Args , o => o == sw) + 1];
		}

		/// <summary>
		/// Checks to see if the switch or flag is in the currently used argument array.
		/// Returns true if the arguments contain a certain switch or flag
		/// </summary>
		/// <param name="switchOrFlag">The switch or flag to check</param>
		/// <returns>Returns true if the arguments contain a certain switch or flag.</returns>
		public bool ArgsContain(string switchOrFlag) => this.Args.Any(a => a == switchOrFlag);

		/// <summary>
		/// Checks to see if the switch or flag is in the currently used argument array.
		/// Returns true if the arguments contain a certain switch or flag
		/// </summary>
		/// <param name="switchOrFlag">The switch or flag to check</param>
		/// <param name="args">The arguments to check against</param>
		/// <returns>Returns true if the switch is found in the arguments array.</returns>
		public static bool ArgsContain(string switchOrFlag , string[] args) => 
			args.Any(a => a == switchOrFlag);

		/// <summary>
		/// Finds a switch or flag from the passed in alias associated with the 
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public string FindSwitchOrFlagFromAlias(string alias) => 
			this._Aliases.Keys.Contains(alias) ?
			this._Aliases[alias] :
			null;

		/// <summary>
		/// Checks to see if the parameter is a an alias
		/// </summary>
		/// <param name="s">The string representation of the alias</param>
		/// <returns>Returns true if the value is an alias</returns>
		public bool IsAlias(string s) => this._Aliases.Keys.Contains(s);

		/// <summary>
		/// Checks to see if the parameter is a a switch
		/// </summary>
		/// <param name="s">The string representation of the switch</param>
		/// <returns>Returns true if the value is a switch</returns>
		public bool IsSwitch(string s) =>
			this._Switches.Keys.Any(k => k == s) ||
			this._Switches.Keys.Contains(this?.FindSwitchOrFlagFromAlias(s) ?? string.Empty);

		/// <summary>
		/// Checks to see if the parameter is a flag
		/// </summary>
		/// <param name="s">The string representation of the flag</param>
		/// <returns>Returns true if the value is a flag</returns>
		public bool IsFlag(string s) =>
			this._Flags.Keys.Any(k => k == s) ||
			this._Flags.Keys.Contains(this?.FindSwitchOrFlagFromAlias(s) ?? string.Empty);

		/// <summary>
		/// Returns the help documentation for the switches, flags, and aliases passed in
		/// </summary>
		/// <returns>Retruns a string that represents the help documentation.</returns>
		public string GetHelpDocumentation() {
			var help = new StringBuilder();

			help.AppendLine("\n");

			help.AppendLine($"// {this.ProgramTitle} //");
			help.AppendLine();

			help.AppendLine(this.HelpDescription);

			help.AppendLine("-----------------------------------------");

			help.AppendLine();
			help.AppendLine($"{"Switches:",-15} Aliases: {string.Empty,-16}Description:");
			foreach (var s in this._Switches)
				help.AppendLine($"{s.Key,-15} {GetAliaseString(s.Key),-25}: {s.Value}");

			help.AppendLine();

			help.AppendLine("Flags:");
			foreach (var f in this._Flags)
				help.AppendLine($"{f.Key,-15} {GetAliaseString(f.Key),-25}: {f.Value}");

			return help.ToString();
		}

        /// <summary>
        /// Gets the aliases as a string that is comma separated
        /// </summary>
        /// <param name="key">The switch or flag to find and map the aliases to.</param>
        /// <returns>Returns a comma separated string of aliases.</returns>
        private string GetAliaseString(string key)  {
            var aliases = this._Aliases.Where(a => a.Value == key).Select(a => a.Key);
            return aliases != null && aliases.Count() > 0 ? aliases.Aggregate((v1 , v2) => $"{v1}, {v2}") : "";
         }

		/// <summary>
		/// Sets the title of the program to display in help.
		/// </summary>
		/// <param name="title">The title of the program</param>
		public void SetHelpTitle(string title) => this.ProgramTitle = title;

		/// <summary>
		/// Sets the help description for the program.
		/// </summary>
		/// <param name="description">The help description for the program</param>
		public void SetHelpDescription(string description) =>
			this.HelpDescription = description;

		/// <summary>
		/// Parses a string array for arguments that have been passed in.
		/// </summary>
		/// <param name="args">The arguments to parse through.</param>
		/// <returns>Returns a string array of only the passed in switches and flags.</returns>
		public static string[] ParseOnlySwitchesOrFlags(string[] args) =>
			args.Where(a => a.Contains("-") || a.Contains("/")).ToArray();

		/// <summary>
		/// Parses a string array for arguments that have been passed in.
		/// </summary>
		/// <param name="args">The arguments to parse through.</param>
		/// <returns>Returns a string array of only the passed in switches and flags.</returns>
		public string[] ParseSwitchesAndFlags() =>
			this.Args.Where(a => a.Contains("-") || a.Contains("/")).ToArray();
	}
}
