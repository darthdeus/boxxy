using System;
using System.Text.RegularExpressions;
using Boxxy.Core;

namespace Boxxy.Screens
{
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
                Console.WriteLine("REQUEST:");

                for (int i = 0; i < request.Headers.Count; i++) {
                    var header = request.Headers[i];
                    Console.WriteLine("{0}) {1}: {2}", i + 1, header.Name, header.Value);
                }

                if (request.IsSent) {
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("RESPONSE:");

                    foreach (var header in request.ResponseHeaders) {
                        Console.WriteLine("{0}: {1}", header.Name, header.Value);
                    }

                    Console.WriteLine();
                    const int limit = 200;
                    if (request.ResponseBody.Length > limit) {
                        Console.WriteLine(request.ResponseBody.Substring(0, limit));
                    } else {
                        Console.WriteLine(request.ResponseBody);
                    }
                }

                Console.WriteLine("---------------------------------------");
                Console.WriteLine("a) Add a new header");
                Console.WriteLine("d) Delete request");
                Console.WriteLine("m) Change method");
                Console.WriteLine("u) Change URL");
                Console.WriteLine("p) Replay the request (send it to the destination as is)");
                Console.WriteLine("b) Go back");
                Console.WriteLine();
                
                string input = Console.ReadLine();
                if (input != null) {
                    input = input.Trim();

                    if (input == "a") {
                        new AddHeaderScreen(request).Run();
                    } else if (input == "b") {
                        break;
                    } else if (input == "d") {
                        _store.RemoveRequestAt(_index);
                        break;
                    } else if (input == "m") {
                        new ChangeMethodScreen(request).Run();
                    } else if (input == "u") {
                        new ChangeUrlScreen(request).Run();
                    } else if (input == "p") {
                        int timeout = 20;

                        Console.Clear();
                        Console.WriteLine("Replaying the request, {0}s timeout", timeout);
                        try {
                            bool ok = request.Play().Wait(TimeSpan.FromSeconds(20));

                            if (ok) {
                                new ResponseDetailScreen(request).Run();
                                // After the request has been replayed, we don't want to stick around on this screen anymore,
                                // as the user will most likely want to replay another request right after that.
                                break;
                            } else {
                                Console.WriteLine();
                                Console.WriteLine("Request replay failed, press ENTER to continue.");
                            }
                        } catch (AggregateException e) {
                            Console.WriteLine("There was an error processing the request, press ENTER to continue.");
                            Console.WriteLine(e);
                            Console.ReadLine();
                        }
                    } else if (new Regex("^\\d+$").IsMatch(input)) {
                        int index = int.Parse(input) - 1;

                        if (index >= 0 && index < request.Headers.Count) {
                            new HeaderDetailScreen(request, index).Run();
                        } else {
                            Console.WriteLine("Invalid header index.");
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