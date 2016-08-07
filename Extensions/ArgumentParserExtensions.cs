using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratusCube.Shargs.Extensions {
	public static class ArgumentParserExtensions {
		public static bool IsSwitch(this string s , ArgumentParser argParser) =>
			argParser.Switches.Keys.Any(k => k == s) ||
			argParser.Switches.Keys.Contains(argParser?.FindSwitchOrFlagFromAlias(s) ?? string.Empty);

		public static bool IsFlag(this string s , ArgumentParser argParser) =>
			argParser.Flags.Keys.Any(k => k == s) ||
			argParser.Flags.Keys.Contains(argParser?.FindSwitchOrFlagFromAlias(s) ?? string.Empty);

		public static bool IsAlias(this string s , ArgumentParser argParser) =>
			 argParser.Aliases.Keys.Contains(s);

		public static string[] ParseSwitchesAndFlags(this ICollection<string> args) =>
			args.Where(a => a.Contains("-") || a.Contains("/")).ToArray();
	}
}
