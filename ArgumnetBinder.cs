using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratusCube.Shargs {

	/// <summary>
	/// An argument binder responsible for binding function to switches or flags for a given ArgumnetParser.
	/// The ArgumentParser that is passed in is copied and is independent from the one used in this class.
	/// </summary>
	public class ArgumnetBinder {

		/// <summary>
		/// The ArgumentParser for tracking bound methods.
		/// </summary>
		private ArgumentParser ArgumnetParser { get; set; }

		/// <summary>
		/// The Default constructor which requires an ArgumentParser.
		/// </summary>
		/// <param name="argParser">The argument parser to track. This is copied into the object</param>
		public ArgumnetBinder(ArgumentParser argParser) {
			this.ArgumnetParser = argParser;
		}

		/// <summary>
		/// The function binding where the key is the switch or flag and the value a delegate.
		/// </summary>
		private Dictionary<string , Delegate> FunctionBindings = new Dictionary<string , Delegate>();

		/// <summary>
		/// Binds a switch to a function. Switch functions must have inputs.
		/// Returns true if successfully bound to a switch. 
		/// Used when an output is specified.
		/// </summary>
		/// <typeparam name="T">The output type of the function</typeparam>
		/// <param name="sw">The switch to bind the function to</param>
		/// <param name="function">The function to bind to the given switch.</param>
		/// <returns>Returns true if successfully bound</returns>
		public bool BindSwitch<T>(string sw , Func<string , T> function) {
			if (!this.ArgumnetParser.IsSwitch(sw) || this.ArgumnetParser.IsAlias(sw)) return false;
			FunctionBindings.Add(sw , function);
			return true;
		}

		/// <summary>
		/// Binds a switch to a function. Switch functions must have inputs.
		/// Returns true if successfully bound to a switch.
		/// Used if no output is specified.
		/// </summary>
		/// <param name="sw">The switch to bind the function to</param>
		/// <param name="function">The function to bind to a switch</param>
		/// <returns>Returns true if successfully bound</returns>
		public bool BindSwitch(string sw , Action<string> function) {
			if (!this.ArgumnetParser.IsSwitch(sw) || this.ArgumnetParser.IsAlias(sw)) return false;
			this.FunctionBindings.Add(sw , function);
			return true;
		}

		/// <summary>
		/// Binds a flag to a given a given Func which does have a specified output
		/// Returns true if successfully bound to a flag.
		/// </summary>
		/// <typeparam name="T">The output type of the passed function</typeparam>
		/// <param name="flag">The flag to bind the function to.</param>
		/// <param name="function">The function that will be bound to the flag.</param>
		/// <returns>Returns true if successfully bound</returns>
		public bool BindFlag<T>(string flag , Func<T> function) {
			if (!this.ArgumnetParser.IsFlag(flag) || this.ArgumnetParser.IsAlias(flag)) return false;
			this.FunctionBindings.Add(flag , function);
			return true;
		}

		/// <summary>
		/// Binds a flag to a given a given Action which does not have output specified. 
		/// Returns true if successfully bound to a flag.
		/// </summary>
		/// <param name="flag">The flag to bind a function to.</param>
		/// <param name="function">The function to bind to the flag</param>
		/// <returns>Return true if bound to a flag.</returns>
		public bool BindFlag(string flag , Action function) {
			if (!this.ArgumnetParser.IsFlag(flag) || this.ArgumnetParser.IsAlias(flag)) return false;
			this.FunctionBindings.Add(flag , function);
			return true;
		}

		/// <summary>
		/// Invokes a function that has been assigned to a switch from a given switch or alias
		/// </summary>
		/// <param name="sw">The switch which will invoke a method</param>
		/// <returns>Returns a result if an output has been specified.</returns>
		public object InvokeSwitchFunction(string sw) {
			if (!this.FunctionBindings.Keys.Contains(sw) && 
				!this.ArgumnetParser.IsSwitch(sw)) return null;

			if (this.ArgumnetParser.IsAlias(sw))
				sw = this.ArgumnetParser.FindSwitchOrFlagFromAlias(sw); 

			var arg = this.ArgumnetParser.GetArgument(sw);

			return this.FunctionBindings[sw].DynamicInvoke(arg);
		}

		/// <summary>
		/// Invokes a function that has been assigned to a flag from a given flag or alias
		/// </summary>
		/// <param name="flag">The flag which will invoke a method</param>
		/// <returns>Returns a result if an output has been specified.</returns>
		public object InvokeFlagFunction(string flag) {
			if (!this.FunctionBindings.Keys.Contains(flag) &&
				!this.ArgumnetParser.IsFlag(flag)) return null;

			if (this.ArgumnetParser.IsAlias(flag))
				flag = this.ArgumnetParser.FindSwitchOrFlagFromAlias(flag);

			return this.FunctionBindings[flag].DynamicInvoke();
		}
	}
}
