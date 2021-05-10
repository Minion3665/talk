using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace talk.abc
{
    /// <summary>
    /// A cog, a set of commands and listeners, they are similar to cogs in discord.py. [definition from spec 0.0.1] 
    /// </summary>
    [PublicAPI]
    public class Cog
    {
        /// <summary>
        /// An attribute that marks a function as a command
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        [PublicAPI]
        protected class Command: Attribute
        {
            /// <summary>
            /// The name of the command, commonly used in help commands & similar
            /// Defaults to null, which should be interpreted as the name of the command function
            /// </summary>
            public readonly string? CommandName;

            /// <summary>
            /// Should this cog be hidden in help commands, even to users who have permission to use it?
            /// Defaults to no
            /// </summary>
            public readonly bool Hidden;

            /// <inheritdoc />
            public Command(string? commandName = null, bool hidden = false)
            {
                CommandName = commandName;
                Hidden = hidden;
            }
        }
        
        /// <summary>
        /// An attribute that marks a function as an event listener
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        [PublicAPI]
        protected class Listener: Attribute
        {
            /// <summary>
            /// The event that this function will listen to
            /// </summary>
            public readonly string? Event;

            /// <inheritdoc />
            public Listener(string? eventName = null)
            {
                Event = eventName;
            }
        }

        /// <summary>
        /// Get all of the commands in this cog
        /// </summary>
        /// <returns>An IEnumerable of command functions</returns>
        public IEnumerable<MethodInfo> GetCommands(bool showHidden = true)
        {
            var commands = GetType()
                .GetMethods()
                .Where(info => info.GetCustomAttributes(typeof(Command), true) is Command[] 
                    {Length: 1} attrs && (showHidden || !attrs[0].Hidden));
            return commands;
        }
        
        /// <summary>
        /// Get all of the listeners in this cog
        /// </summary>
        /// <returns>An IEnumerable of event listener functions</returns>
        public IEnumerable<MethodInfo> GetListeners(string? eventName = null)
        {
            var events = GetType()
                .GetMethods()
                .Where(info => info.GetCustomAttributes(typeof(Listener), true) is Listener[] 
                    {Length: 1} attrs && (eventName != null || attrs[0].Event == eventName));
            return events;
        }
    }
}