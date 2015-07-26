using Boxxy.Core;

namespace Boxxy
{
    internal static class Program
    {
        private static void Main(string[] args) {
            var store = new ProxyStore(@".");
            var proxy = new HttpProxy(store);
            proxy.Start("http://localhost:8080/", "http://requestb.in/pa43a9pa");

            try {
                new MainScreen(store).Run();
            } catch (QuitMenu) {
                // Intentionally ignored, the exception is only used for control flow.
            }
        }
    }
}