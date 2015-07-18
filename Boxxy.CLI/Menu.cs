using System;
using System.Collections.Generic;
using System.Linq;

namespace Boxxy
{
    internal class Menu
    {
        public IList<MenuItem> Items { get; set; }

        private void Print() {
            Console.WriteLine();
            for (int i = 0; i < Items.Count; i++) {
                Console.WriteLine("{0}) {1}", Items[i].Shortcut, Items[i].Text);
            }
        }

        private void HandleCommand(string line) {
            var selectedItem = Items.FirstOrDefault(x => x.Shortcut == line.Trim());
            if (selectedItem != null && selectedItem.Action != null) {
                selectedItem.Action();
            }
        }

        public void Run() {
            Items.Add(new MenuItem("Quit", "q", () => { throw new QuitMenu(); }));

            try {
                while (true) {
                    Print();
                    var line = Console.ReadLine();
                    HandleCommand(line);
                }
            } catch (QuitMenu) {
                // Intentionally ignored
            }
        }
    }
}