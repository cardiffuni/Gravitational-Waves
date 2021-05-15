using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Game.Extensions;


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
    }
}
