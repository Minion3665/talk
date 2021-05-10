using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using talk.abc;

namespace example_bot
{
    [PublicAPI]
    public class Commands : Cog
    {
        [Command]
        public async Task HelloWorld()
        {
            Console.WriteLine("Hello Minion!");
        }
    }
}