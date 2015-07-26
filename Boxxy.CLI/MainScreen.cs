using System;

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
                    Console.WriteLine("s) stop proxy\nq) quit\nl) list all requests");
                } else {
                    Console.WriteLine("s) start proxy\nq) quit\nl) list all requests");
                }

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
                        break;
                }
            }
        }
    }
}