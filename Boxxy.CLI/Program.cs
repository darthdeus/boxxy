using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Boxxy.Core;

namespace Boxxy
{
    internal interface IScreen
    {
        void Run();
    }

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
                    Console.WriteLine("{0}) {1} {2}", i + 1, request.HttpMethod, request.Uri);
                }

                Console.WriteLine("---------------------------------------");
                Console.WriteLine("d) Delete all requests");
                Console.WriteLine("b) Go back");

                string input = Console.ReadLine();
                if (input != null) {
                    input = input.Trim();

                    if (input == "b") {
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
                            _store.Requests.Clear();                            
                        }
                    }
                } else {
                    // TODO - better exception
                    throw new ArgumentException();
                }
            }
        }
    }

    internal class RequestDetailScreen : IScreen
    {
        private readonly ProxyStore _store;
        private readonly int _index;

        public RequestDetailScreen(ProxyStore store, int index) {
            _store = store;
            _index = index;
        }

        public void Run() {
            var request = _store.Requests[_index];
            while (true) {
                Console.Clear();
                Console.WriteLine("Detail for request:\n{0} {1}\n", request.HttpMethod, request.Uri);

                for (int i = 0; i < request.Headers.Count; i++) {
                    var header = request.Headers[i];
                    Console.WriteLine("{0}) {1}: {2}", i + 1, header.Name, header.Value);
                }

                Console.WriteLine("---------------------------------------");
                Console.WriteLine("b) Go back");
                Console.WriteLine("d) Delete request");
                Console.WriteLine("m) Change method");
                Console.WriteLine("u) Change URL");
                Console.WriteLine("p) Replay the request (send it to the destination as is)");
                Console.WriteLine();

                string input = Console.ReadLine();
                if (input != null) {
                    input = input.Trim();

                    if (input == "b") {
                        break;
                    } else if (input == "m") {
                        new ChangeMethodScreen(request).Run();
                    } else if (input == "u") {
                        new ChangeUrlScreen(request).Run();
                    } else if (input == "d") {
                        _store.Requests.RemoveAt(_index);
                        break;
                    } else if (new Regex("^\\d+$").IsMatch(input)) {
                        int index = int.Parse(input) - 1;

                        if (index >= 0 && index < request.Headers.Count) {
                            // TODO - header detail screen
                            // new RequestDetailScreen(_store, index).Run();
                        } else {
                            Console.WriteLine("Invalid header index.");
                        }
                    }
                } else {
                    // TODO - better exception
                    throw new ArgumentException();
                }
            }
        }
    }

    internal class ChangeUrlScreen : IScreen
    {
        private readonly IncomingHttpRequest _request;

        public ChangeUrlScreen(IncomingHttpRequest request) {
            _request = request;
        }

        public void Run() {
            Console.Clear();
            Console.WriteLine("Changing URL for request:\n{0} {1}\n", _request.HttpMethod, _request.Uri);

            Console.WriteLine("Type a new URL and press enter (or simply press enter to cancel.)");
            string line = Console.ReadLine();

            if (!string.IsNullOrEmpty(line)) {
                try {
                    var url = new Uri(line);
                    _request.Uri = url;
                } catch (UriFormatException) {
                    Console.WriteLine("Invalid URL format, press ENTER to continue.");
                    Console.ReadLine();

                    Run();
                }
            }
        }
    }

    internal class ChangeMethodScreen : IScreen
    {
        private readonly IncomingHttpRequest _request;

        public ChangeMethodScreen(IncomingHttpRequest request) {
            _request = request;
        }

        public void Run() {
            Console.Clear();
            Console.WriteLine("Changing method for request:\n{0} {1}\n", _request.HttpMethod, _request.Uri);

            Console.WriteLine("Type a new method and press enter (or simply press enter to cancel.)");
            string line = Console.ReadLine();

            if (!string.IsNullOrEmpty(line)) {
                _request.HttpMethod = line.Trim();
            }
        }
    }


    internal class Program
    {
        private static void Main(string[] args) {
            var store = new ProxyStore();
            var proxy = new HttpProxy(store);
            proxy.Start("http://localhost:8080/", "http://requestb.in/pa43a9pa");

            new MainScreen(store).Run();

            //while (true) {
            //    var i = Console.ReadKey();

            //    char c = i.KeyChar;
            //    Console.Clear();
            //    Console.Out.Flush();
            //    Console.WriteLine((int) c);

            //    if (c == 'a') {
            //        Console.WriteLine("jo");
            //    } else {
            //        Console.WriteLine("ne");
            //    }
            //}

            //new Application().Run();
        }
    }
}