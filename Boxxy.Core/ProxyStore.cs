using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using Newtonsoft.Json;

namespace Boxxy.Core
{
    public class ProxyStore
    {
        private readonly string _path;
        public IList<IncomingHttpRequest> Requests { get; set; }

        public ProxyStore(string path, IList<IncomingHttpRequest> requests) {
            _path = path;
            Requests = requests;
        }

        public List<string> Sync() {
            var syncErrors = new List<string>();

            foreach (var request in Requests) {
                try {
                    string path = Path.Combine(_path, FileName(request));
                    Debug.WriteLine("Syncing request to {0}", path);

                    using (var writer = new StreamWriter(path)) {
                        string jsonString = JsonConvert.SerializeObject(request);
                        writer.Write(jsonString);
                    }
                } catch (DirectoryNotFoundException) {
                    Console.WriteLine("Directory not found at path {0}", _path);
                    Console.WriteLine();

                    // There's nothing to do here, other than terminate.
                    throw;
                } catch (SecurityException e) {
                    syncErrors.Add(e.ToString());
                } catch (UnauthorizedAccessException e) {
                    syncErrors.Add(e.ToString());
                } catch (PathTooLongException) {
                    // There's really no automatic way to recover from this without breaking
                    // the sync-on-startup.
                    throw;
                } catch (IOException e) {
                    syncErrors.Add(e.ToString());
                }
            }

            if (syncErrors.Count > 0) {
                Debug.WriteLine("Encountered errors during sync:");
                foreach (var error in syncErrors) {
                    Debug.WriteLine("\t{0}", error);
                }
            }

            return syncErrors;
        }

        private string FileName(IncomingHttpRequest request) {
            return string.Format(
                "{0}-{1}-{2}.json",
                request.CreatedAt.ToString("yyyy-M-d_HH-mm-ss-fff", CultureInfo.InvariantCulture),
                request.HttpMethod,
                request.Uri.Host);
        }

        public void RemoveRequestFile(IncomingHttpRequest request) {
            string path = Path.Combine(_path, FileName(request));
            Debug.WriteLine("Deleting file {0}", path);
            File.Delete(path);
        }

        public void Clear() {
            foreach (var request in Requests) {
                // There's no good way to recover from exceptions here, so we'll just let them all through.
                RemoveRequestFile(request);
            }

            Requests.Clear();
        }

        public void RemoveRequestAt(int index) {
            var request = Requests[index];
            RemoveRequestFile(request);
            Requests.RemoveAt(index);
        }
    }
}