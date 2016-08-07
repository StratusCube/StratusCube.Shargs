using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratusCube.Shargs.Extensions {
	public static class ArgumentParserExtensions {

        /// <summary>
        /// Checks if the given string is a switch in the passed in ArgumentParser
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <param name="argParser">The ArgumentParser</param>
        /// <returns>Returns true if the switch is contained in the ArgumentParser</returns>
		public static bool IsSwitch(this string s , ArgumentParser argParser) =>
			argParser.Switches.Keys.Any(k => k == s) ||
			argParser.Switches.Keys.Contains(argParser?.FindSwitchOrFlagFromAlias(s) ?? string.Empty);

        /// <summary>
        /// Checks if the given string is a flag in the passed in ArgumentParser
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <param name="argParser">The ArgumentParser</param>
        /// <returns>Retruns true if the flag provided is contained in the ArgumentParser</returns>
		public static bool IsFlag(this string s , ArgumentParser argParser) =>
			argParser.Flags.Keys.Any(k => k == s) ||
			argParser.Flags.Keys.Contains(argParser?.FindSwitchOrFlagFromAlias(s) ?? string.Empty);

        /// <summary>
        /// Check to see if the ailia is contained in the passed in ArgumentParser
        /// </summary>
        /// <param name="s">Ther string to check</param>
        /// <param name="argParser">The ArgumentParser</param>
        /// <returns>Returns true if the alias is in the provided ArgumentParser</returns>
		public static bool IsAlias(this string s , ArgumentParser argParser) =>
			 argParser.Aliases.Keys.Contains(s);

        /// <summary>
        /// Parses switches and flags from a string array of arguments
        /// </summary>
        /// <param name="args">The arguments array to parse the switches and flags from</param>
        /// <returns>Returns a new array of all the switches and flags</returns>
		public static string[] ParseSwitchesAndFlags(this ICollection<string> args) =>
			args.Where(a => a.Contains("-") || a.Contains("/")).ToArray();
	}
}
