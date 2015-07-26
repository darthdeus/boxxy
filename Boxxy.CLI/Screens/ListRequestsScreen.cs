using System;
using System.Text.RegularExpressions;
using Boxxy.Core;

namespace Boxxy.Screens
{
    internal class ListRequestsScreen : IScreen
    {
        private readonly ProxyStore _store;

        public ListRequestsScreen(ProxyStore store) {
            _store = store;
        }

        public void Run() {
            while (true) {
                Console.Clear();
                for (int i = 0; i < _store.Requests.Count; i++) {
                    var request = _store.Requests[i];
                    Console.WriteLine("{0} {1}) {2} {3}",
                        request.IsSent ? "+" : "?",
                        i + 1,
                        request.HttpMethod,
                        request.Uri);
                }

                Console.WriteLine("---------------------------------------");
                Console.WriteLine("d) Delete all requests");
                Console.WriteLine("r) Refresh");
                Console.WriteLine("b) Go back");

                string input = Console.ReadLine();
                if (input != null) {
                    input = input.Trim();

                    if (input == "r") {
                        continue;
                    } else if (input == "b") {
                        break;
                    } else if (new Regex("^\\d+$").IsMatch(input)) {
                        int index = int.Parse(input) - 1;

                        if (index >= 0 && index < _store.Requests.Count) {
                            new RequestDetailScreen(_store, index).Run();
                        } else {
                            Console.WriteLine("Invalid request index.");
                        }
                    } else if (input == "d") {
                        Console.WriteLine("Are you sure? Y/N");
                        var key = Console.ReadKey().KeyChar;
                        if (key == 'Y' || key == 'y') {
                            _store.Clear();
                        }
                    }

                    _store.Sync();
                } else {
                    throw new InputProcessingError("Unexpected end of input.");
                }
            }
        }
    }
}