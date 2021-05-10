using System;
using talk;
using talk.abc;

namespace example_bot
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Bot bot = new();
            bot.LoadCog(new Commands());
            Console.WriteLine(">>> HelloWorld");
            bot.OnMessage("HelloWorld");
        }
    }
}