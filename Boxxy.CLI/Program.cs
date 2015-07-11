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
using Boxxy.Core;

namespace Boxxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = new HttpProxy("http://localhost:8080/", "http://requestb.in/pa43a9pa");
            proxy.Start();
            Console.ReadLine();
        }
    }

}
