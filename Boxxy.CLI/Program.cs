using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Boxxy.Core;

namespace Boxxy
{
    internal class Program
    {
        private static void Main(string[] args) {
            //var result = Parser.String("c hel").Run("c hello");
            //Console.WriteLine(result);

            var cli = new CommandLineInterface();
            cli.Run();

            var proxy = new HttpProxy("http://localhost:8080/", "http://requestb.in/pa43a9pa");
            proxy.Start();
            Console.ReadLine();
        }
    }

    public class QuitMenu : Exception
    {
    }

    internal class CommandLineInterface
    {
        public void Run() {
            var mainMenu = new Menu();

            var proxy = new HttpProxy("http://localhost:8080/", "http://requestb.in/pa43a9pa");

            var startItem = new MenuItem("Start listening", "s");
            var stopItem = new MenuItem("Stop listening", "s");

            startItem.Action = () => {
                mainMenu.Items[0] = stopItem;
                proxy.Start();
            };
            stopItem.Action = () => {
                mainMenu.Items[0] = startItem;
                proxy.Stop();
            };

            mainMenu.Items = new[] {
                startItem,
                new MenuItem("Print requests", "p", () => { RequestMenu(proxy.IncomingRequests); }),
                new MenuItem("Quit", "q", () => { throw new QuitMenu(); })
            };

            try {
                while (true) {
                    mainMenu.SingleIteration();
                }
            } catch (QuitMenu e) {
                Console.WriteLine("Exitting ...");
            }
        }

        private void RequestMenu(IList<IncomingHttpRequest> requests) {
            var menuItems =
                requests.Select(
                    (request, i) => {
                        var item = new MenuItem(
                            string.Format("{0} {1}", request.HttpMethod, request.Uri),
                            i.ToString(),
                            () => { MenuForRequest(requests, i); });
                        return item;
                    });

            var menu = new Menu {Items = menuItems.ToList()};
            menu.SingleIteration();
        }

        private void MenuForRequest(IList<IncomingHttpRequest> requests, int i) {
            var menu = new Menu {
                Items = new[] {
                    new MenuItem("Delete", "d", () => { requests.RemoveAt(i); })
                }
            };

            menu.SingleIteration();
        }
    }

    internal class Menu
    {
        public IList<MenuItem> Items { get; set; }

        public void Print() {
            for (int i = 0; i < Items.Count; i++) {
                Console.WriteLine("{0}) {1}", Items[i].Shortcut, Items[i].Text);
            }
        }

        public void HandleCommand(string line) {
            var selectedItem = Items.FirstOrDefault(x => x.Shortcut == line.Trim());
            if (selectedItem != null && selectedItem.Action != null) {
                selectedItem.Action();
            }
        }

        public void SingleIteration() {
            Print();
            var line = Console.ReadLine();
            HandleCommand(line);
        }
    }

    internal class MenuItem
    {
        public Action Action;
        public string Text { get; set; }
        public string Shortcut { get; set; }

        public MenuItem(string text, string shortcut, Action action = null) {
            Text = text;
            Shortcut = shortcut;
            Action = action;
        }
    }
}