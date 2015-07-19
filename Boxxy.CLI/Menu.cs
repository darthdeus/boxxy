using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Boxxy.Core;

namespace Boxxy
{
    internal class Application
    {
        public void Run()
        {
            var store = new ProxyStore();

            var proxy = new HttpProxy(store);
            proxy.Start("http://localhost:8080/", "http://requestb.in/pa43a9pa");

            var menu = new Menu(Console.Out, Console.In);
            menu.SingleLoop(Menu.MainMenu(store));
        }
    }

    public enum MenuItemResultType
    {
        Continue,
        Break
    };

    internal class Menu
    {
        private readonly TextWriter _writer;
        private readonly TextReader _reader;

        public Menu(TextWriter writer, TextReader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public void SingleLoop(IList<IMenuItem> items)
        {
            _writer.Write(Render(items));
            var input = _reader.ReadLine();

            if (input != null)
            {
                var match = items.FirstOrDefault(x => x.CurrentPredicate()(input));
                if (match != null)
                {
                    match.Run(input, _writer, _reader);
                }
                else
                {
                    _writer.WriteLine("Invalid input, please only enter one of the hotkeys from the menu.");
                }
            }
        }

        public string Render(IList<IMenuItem> items)
        {
            var buffer = new StringWriter();

            foreach (var item in items)
            {
                buffer.WriteLine(item.Render());
            }

            return buffer.ToString();
        }

        public static IList<IMenuItem> MainMenu(ProxyStore store)
        {
            return new List<IMenuItem> {
                new SimpleMenuItem("a) List all requests", "a",
                    _ => {
                        /* TODO - show index menu */;
                        return MenuItemResultType.Continue;
                    }),
                new SimpleMenuItem("q) Quit", "q",
                    _ => {
                        /* TODO - show index menu */;
                        return MenuItemResultType.Break;
                    }),

            };
        }
    }

    public class SimpleMenuItem : IMenuItem
    {
        private readonly string _text;
        private readonly string _hotkey;
        private readonly Func<string, MenuItemResultType> _action;

        public SimpleMenuItem(string text, string hotkey, Func<string, MenuItemResultType> action)
        {
            _text = text;
            _hotkey = hotkey;
            _action = action;
        }

        public string Render()
        {
            return _text;
        }

        public Func<string, bool> CurrentPredicate()
        {
            return x => x == _hotkey;
        }

        public void Run(string input, TextWriter writer, TextReader reader)
        {
            _action(input);
        }
    }

    internal interface IMenuItem
    {
        string Render();
        Func<string, bool> CurrentPredicate();
        void Run(string input, TextWriter writer, TextReader reader);
    }


    //internal class MenuItem
    //{
    //    public Action Action { get; set; }
    //    public string Text { get { return _render(); } }
    //    public string Shortcut { get; private set; }
    //    private readonly Func<string> _render;


    //    public MenuItem(string text, string shortcut, Action action = null) : this(() => text, shortcut, action) { }

    //    public MenuItem(Func<string> renderFunc, string shortcut, Action action = null)
    //    {
    //        _render = renderFunc;
    //        Shortcut = shortcut;
    //        Action = action;
    //    }
    //}

    //internal class Menu
    //{
    //    public IList<IMenuModel> Models { get; set; } 

    //    private void Print() {
    //        Console.WriteLine();
    //        var items = Models.Select(x => x.Render()).ToList();
    //        items.Add(new MenuItem("Quit", "q", () => { throw new QuitMenu(); }));
            
    //        for (int i = 0; i < items.Count; i++) {
    //            Console.WriteLine("{0}) {1}", items[i].Shortcut, items[i].Text);
    //        }
    //    }

    //    private void HandleCommand(string line) {
    //        var items = Models.Select(x => x.Render()).ToList();

    //        var selectedItem = items.FirstOrDefault(x => x.Shortcut == line.Trim());
    //        if (selectedItem != null && selectedItem.Action != null) {
    //            selectedItem.Action();
    //        }
    //    }

    //    public void Run() {
    //        try {
    //            while (true) {
    //                Print();
    //                var line = Console.ReadLine();
    //                HandleCommand(line);
    //            }
    //        } catch (QuitMenu) {
    //            // Intentionally ignored
    //        }
    //    }
    //}
    //
    //internal interface IMenuModel
    //{
    //    MenuItem Render();
    //}
}