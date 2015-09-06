using System;
using System.Text.RegularExpressions;
using Boxxy.Core;

namespace Boxxy.Screens
{
    internal class AddHeaderScreen
    {
        private readonly IncomingHttpRequest _request;

        public AddHeaderScreen(IncomingHttpRequest request) {
            _request = request;
        }

        public void Run() {
            Console.Clear();
            Console.WriteLine(
                "Type a new header in the format 'Name: Value' and press ENTER to add it, or simply press ENTER without typing anything to cancel.");
            Console.WriteLine();

            var regex = new Regex("^.+:.+$");
            while (true) {
                string line = Console.ReadLine();
                if (line != null && regex.IsMatch(line)) {
                    int i = line.IndexOf(':');

                    var header = new Header(line.Substring(0, i).Trim(), line.Substring(i + 1).Trim());
                    _request.Headers.Add(header);
                    break;
                } else {
                    if (line == "") {
                        break;
                    } else {
                        Console.WriteLine("Invalid header, please try again in the 'Name: Value' format.");
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}