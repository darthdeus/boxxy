using System;
using System.Collections.Generic;
using System.Linq;
using Boxxy.Core;

namespace Boxxy
{
    //internal class CommandLineInterface
    //{
        

    //    public void Run() {
    //        var mainMenu = new Menu(Console.Out, Console.In);

    //        var proxy = new HttpProxy("http://localhost:8080/", "http://requestb.in/pa43a9pa");

    //        //var startItem = new MenuItem("Start listening", "s");
    //        //var stopItem = new MenuItem("Stop listening", "s");

    //        //startItem.Action = () => {
    //        //    mainMenu.Items[0] = stopItem;
    //        //    proxy.Start();
    //        //};
    //        //stopItem.Action = () => {
    //        //    mainMenu.Items[0] = startItem;
    //        //    proxy.Stop();
    //        //};

    //        proxy.Start();

    //        mainMenu.Items = new List<MenuItem> {
    //            //startItem,
    //            new MenuItem(
    //                "Print requests",
    //                "p",
    //                () => { RequestMenu(proxy.IncomingRequests); })
    //        };

    //        mainMenu.Run();
    //    }

    //    private void RequestMenu(IList<IncomingHttpRequest> requests) {
    //        var menuItems =
    //            requests.Select(
    //                (request, i) => {
    //                    var item = new MenuItem(
    //                        () => string.Format("{0} {1}", request.HttpMethod, request.Uri),
    //                        i.ToString(),
    //                        () => { MenuForRequest(requests, i); });
    //                    return item;
    //                }).ToList();

    //        var menu = new Menu {Items = menuItems};
    //        menu.Run();
    //    }

    //    private void MenuForRequest(IList<IncomingHttpRequest> requests, int i) {
    //        var menu = new Menu {
    //            Items = new List<MenuItem> {
    //                new MenuItem("Delete", "d", () => { requests.RemoveAt(i); }),
    //                new MenuItem("Edit", "e", () => { EditMenuFor(requests[i]); }),
    //            }
    //        };

    //        menu.Run();
    //    }

    //    private void EditMenuFor(IncomingHttpRequest request) {
    //        var menuItems = new List<MenuItem> {
    //            new MenuItem(() => request.Uri.ToString(), "u", () => { EditUri(request); }),
    //            new MenuItem(() => request.HttpMethod, "m", () => { EditMethod(request); })
    //        };

    //        var additionalItems = request.Headers.Select(
    //            (header, i) => new MenuItem(
    //                () => string.Format("{0}: {1}", header.Name, header.Value),
    //                (i + 1).ToString(),
    //                () => { EditHeader(request.Headers, i); }));

    //        foreach (var item in additionalItems) {
    //            menuItems.Add(item);
    //        }

    //        new Menu {Items = menuItems}.Run();
    //    }

    //    private void EditHeader(IList<Header> headers, int i) {
    //        Console.WriteLine(
    //            "Enter 'd' to delete the header, or press enter without entering anything to cancel, or simpyl enter the new header value, including the name and press enter to update.");

    //        string line = Console.ReadLine();

    //        if (!string.IsNullOrEmpty(line)) {
    //            line = line.Trim();
    //            if (line == "d") {
    //                headers.RemoveAt(i);
    //            } else {
    //                int index = line.IndexOf(':');
    //                if (index > 0) {
    //                    headers[i] = new Header(line.Substring(0, index), line.Substring(index));
    //                }
    //            }
    //        }
    //    }

    //    private void EditMethod(IncomingHttpRequest request) {
    //        Console.WriteLine(
    //            "Enter a new HTTP method and press enter, or press enter without entering anything to cancel.");

    //        string line = Console.ReadLine();
    //        if (!string.IsNullOrEmpty(line)) {
    //            // TODO - validate method value
    //            request.HttpMethod = line;
    //        }
    //    }

    //    private void EditUri(IncomingHttpRequest request) {
    //        Console.WriteLine(
    //            "Enter a new URI and press enter to update, or press enter without entering anything to cancel.");

    //        string line = Console.ReadLine();
    //        if (!string.IsNullOrEmpty(line)) {
    //            // TODO - validate URI format
    //            request.Uri = new Uri(line.Trim());
    //        }
    //    }
    //}
}