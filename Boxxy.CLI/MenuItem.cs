using System;

namespace Boxxy
{
    internal class MenuItem
    {
        public Action Action { get; set; }
        public string Text { get { return _render(); } }
        public string Shortcut { get; private set; }
        private readonly Func<string> _render; 


        public MenuItem(string text, string shortcut, Action action = null): this(() => text, shortcut, action) {}

        public MenuItem(Func<string> renderFunc, string shortcut, Action action = null) {
            _render = renderFunc;
            Shortcut = shortcut;
            Action = action;
        }
    }
}