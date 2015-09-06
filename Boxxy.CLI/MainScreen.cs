using System;
using Boxxy.Core;
using Boxxy.Screens;

namespace Boxxy
{
    internal class MainScreen : IScreen
    {
        private readonly ProxyStore _store;

        public MainScreen(ProxyStore store) {
            _store = store;
        }

        public void Run() {
            bool started = false;

            while (true) {
                Console.Clear();

                if (started) {
                    Console.WriteLine("s) Stop proxy");
                } else {
                    Console.WriteLine("s) Start proxy");
                }

                Console.WriteLine("l) List all requests");
                Console.WriteLine("q) Quit");

                var key = Console.ReadKey();

                switch (key.KeyChar) {
                    case 's':
                        started = !started;
                        break;

                    case 'l':
                        new ListRequestsScreen(_store).Run();
                        break;

                    case 'q':
                        throw new QuitMenu();
                }
            }
        }
    }
}