using System;
using System.Threading.Tasks;

namespace talk
{
    public class Bot
    {
        private Plugin[] plugins;
        public async void RunAsync(string[] tokens)
        {
            
        }
        public void Run(string[] tokens)
        {
            Task.Run(this.RunAsync(tokens)).Wait();
        }
    }
}