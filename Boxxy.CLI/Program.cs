﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using Boxxy.Core;

namespace Boxxy
{
    internal static class Program
    {
        private static void Main(string[] args) {
            string syncPath = @".";

            string[] files = Directory.GetFiles(syncPath, "*.json");

            var requests = new List<IncomingHttpRequest>();

            foreach (var file in files) {
                try {
                    using (var reader = new StreamReader(file)) {
                        string str = reader.ReadToEnd();

                        requests.Add(IncomingHttpRequest.FromString(str));
                        Debug.WriteLine("Deserialized request from file {0}", str);
                    }
                } catch (SecurityException e) {
                    ReadFailed(file, e);
                } catch (UnauthorizedAccessException e) {
                    ReadFailed(file, e);
                } catch (IOException e) {
                    ReadFailed(file, e);
                }
            }

            // TODO - sync stored requests from disk

            var store = new ProxyStore(syncPath, requests);
            var proxy = new HttpProxy(store);
            proxy.Start("http://localhost:8080/", "http://requestb.in/pa43a9pa");

            try {
                new MainScreen(store).Run();
            } catch (QuitMenu) {
                // Intentionally ignored, the exception is only used for control flow.
            }
        }

        private static void ReadFailed(string file, Exception exception) {
            Console.WriteLine("Error reading file {0} during initial sync, ignoring.\n\n{1}", file, exception);
            throw exception;
        }
    }
}