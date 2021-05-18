using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Game.Extensions;
using Open.Nat;
using System.Threading;
using System.Net;

namespace Game.Utility {
    public static class Networking {
        public static IEnumerator CheckIP(Action<string> callback) {
            string uri = "http://checkip.dyndns.org";
            yield return GetRequest(uri, webRequest => {
                string ExtIP = webRequest.downloadHandler.text;
                ExtIP = ExtIP.Substring(ExtIP.IndexOf(":") + 1);
                ExtIP = ExtIP.Substring(0, ExtIP.IndexOf("<"));
                ExtIP = ExtIP.Trim();
                callback(ExtIP);
            });
        }
        public static IEnumerator GetRequest(string uri, Action<UnityWebRequest> callback) {
            Debug.LogFormat("Get Query: {0}", uri);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result) {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        callback(webRequest);
                        break;
                }
            }
        }

        public static IEnumerator PostRequest(string uri, WWWForm form, Action<UnityWebRequest> callback) {
            Debug.LogFormat("Post Query: {0}, {1}", uri, form.data.ToString());
            using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, form)) {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result) {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        callback(webRequest);
                        break;
                }
            }
        }

        public static Task UPnP() {
            var nat = new NatDiscoverer();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);

            NatDevice device = null;
            var sb = new StringBuilder();
            IPAddress ip = null;

            Debug.LogFormat("Starting punch test");

            return nat.DiscoverDeviceAsync(PortMapper.Upnp, cts)
                .ContinueWith(task => {
                    device = task.Result;
                    return device.GetExternalIPAsync();

                })
                .Unwrap()
                .ContinueWith(task => {
                    ip = task.Result;
                    sb.AppendFormat("\nYour IP: {0}", ip);
                    return device.CreatePortMapAsync(new Mapping(Protocol.Tcp, 7777, 7777, 0, "myGame Server (TCP)"));
                })
                .Unwrap()
                .ContinueWith(task => {
                    return device.CreatePortMapAsync(
                        new Mapping(Protocol.Udp, 7777, 7777, 0, "myGame Server (UDP)"));
                })
                .Unwrap()
                .ContinueWith(task => {
                    sb.AppendFormat("\nAdded mapping: {0}:1700 -> 127.0.0.1:1600\n", ip);
                    sb.AppendFormat("\n+------+-------------------------------+--------------------------------+------------------------------------+-------------------------+");
                    sb.AppendFormat("\n| PORT | PUBLIC (Reacheable)           | PRIVATE (Your computer)        | Description                        |                         |");
                    sb.AppendFormat("\n+------+----------------------+--------+-----------------------+--------+------------------------------------+-------------------------+");
                    sb.AppendFormat("\n|      | IP Address           | Port   | IP Address            | Port   |                                    | Expires                 |");
                    sb.AppendFormat("\n+------+----------------------+--------+-----------------------+--------+------------------------------------+-------------------------+");
                    return device.GetAllMappingsAsync();
                })
                .Unwrap()
                .ContinueWith(task => {
                    foreach (var mapping in task.Result) {
                        sb.AppendFormat("\n|  {5} | {0,-20} | {1,6} | {2,-21} | {3,6} | {4,-35}|{6,25}|",
                            ip, mapping.PublicPort, mapping.PrivateIP, mapping.PrivatePort, mapping.Description,
                            mapping.Protocol == Protocol.Tcp ? "TCP" : "UDP", mapping.Expiration.ToLocalTime());
                    }
                    sb.AppendFormat("\n+------+----------------------+--------+-----------------------+--------+------------------------------------+-------------------------+");
                    sb.AppendFormat("\n[Removing TCP mapping] {0}:1700 -> 127.0.0.1:1600", ip);
                    return device.DeletePortMapAsync(new Mapping(Protocol.Tcp, 1600, 1700));
                })
                .Unwrap()
                .ContinueWith(task => {
                    sb.AppendFormat("\n[Done]");
                    Debug.Log(sb.ToString());
                });
        }
    }
}
