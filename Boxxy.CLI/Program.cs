using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using Boxxy.Core;

namespace Boxxy
{
    internal class Program
    {
        private static void Main(string[] args) {
            var cli = new CommandLineInterface();
            cli.Run();

            var proxy = new HttpProxy("http://localhost:8080/", "http://requestb.in/pa43a9pa");
            proxy.Start();
            Console.ReadLine();
        }
    }
}