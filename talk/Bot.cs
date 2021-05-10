using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using talk.abc;

namespace talk
{
    [PublicAPI]
    public class Bot
    {
        //TODO: Proper events (dispatch, wait for, etc)
        //TODO: See https://stackoverflow.com/questions/38996593/promise-equivalent-in-c-sharp
        protected List<Bridge> Bridges = new();
        protected List<Cog> Cogs = new();
        
        protected Dictionary<string, Func<string, Task>> Handlers = new();

        private IEnumerable<string> _commandNameCache = Enumerable.Empty<string>();

        /// <summary>
        /// Constructs a bot
        /// </summary>
        public Bot()
        {
            Handlers.Add("message", OnMessage);
        }

        protected bool ValidateCommands(Cog cog, bool warnOnly)
        {
            var names = new List<string>();
            foreach (var info in cog.GetCommands())
            {
                if (_commandNameCache.Contains(info.Name))
                {
                    Console.WriteLine($"Command {info.Name} in cog {cog.GetType().Name} is already registered");
                    if (!warnOnly) return false;
                }
                if (info.ReturnType != typeof(Task))
                {
                    Console.WriteLine($"Command {info.Name} in cog {cog.GetType().Name} is not a task (it's a {info.ReturnType.Name}) and will not be executed");
                    if (!warnOnly) return false;
                }
                names.Add(info.Name);
            }
            _commandNameCache = _commandNameCache.Concat(names);
            return true;
        }

        /// <summary>
        /// Load a cog, adding its commands and listeners into the bot
        /// </summary>
        /// <param name="cog">The cog that you would like to load</param>
        /// <param name="force">A boolean dictating if you'd like to force the cog to load ignoring validation errors</param>
        public void LoadCog(Cog cog, bool force = false)
        {
            if (ValidateCommands(cog, force))
            {
                Cogs.Add(cog);
            }
            else
            {
                Console.WriteLine($"The cog {cog.GetType().Name} was not loaded due to validation errors");
            }
        }

        /// <summary>
        /// Process commands for a specific message
        /// </summary>
        /// <param name="message"></param>
        public async Task ProcessCommands(string message)
        {
            IEnumerable<Tuple<Cog, MethodInfo>> commands = Cogs.Aggregate(
                Enumerable.Empty<Tuple<Cog, MethodInfo>>(), 
                (current, cog) => current.Concat(cog.GetCommands().Select(info => (cog, info).ToTuple())));
            // TODO: Do some fancy parsing here
            IEnumerable<Tuple<Cog, MethodInfo>> filteredCommands =
                commands.Where(info => info.Item2.Name == message && info.Item2.ReturnType == typeof(Task));
            await Task.WhenAll(filteredCommands.Select(command => (Task)command.Item2.Invoke(command.Item1, new object[]{})).ToList()).ConfigureAwait(false);
        }
        
        /// <summary>
        /// The default handler for the message event; figures out what commands need to be run, if any, and runs them
        /// </summary>
        /// <param name="message"></param>
        public async Task OnMessage(string message)
        {
            await ProcessCommands(message);
        }
    }
}