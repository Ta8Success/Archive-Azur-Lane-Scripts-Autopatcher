using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Konpaku
{
    internal class HttpClient
    {
        private static Dictionary<string, string> Headers;

        static HttpClient()
        {
            if (Headers == null)
            {
                Headers = new Dictionary<string, string>()
                {
                    { "User-Agent", "Anon/1.0" },
                    { "Cache-Control", "no-cache" }
                };
            }
        }

        internal static IEnumerator GetBytes(string url, Action<bool, string, byte[]> callback)
        {
            yield return Get(url, delegate (bool isErr, string err, byte[] bytes)
            {
                if (isErr)
                {
                    Debugger.Log(err);
                }
                callback(isErr, err, bytes);
            });
        }

        internal static IEnumerator GetContent(string url, Action<bool, string, string> callback)
        {
            yield return Get(url, delegate (bool isErr, string err, string content)
            {
                if (isErr)
                {
                    Debugger.Log(err);
                }
                callback(isErr, err, content);
            });
        }

        internal static IEnumerator GetHeader(string url, string name, Action<bool, string, string> callback)
        {
            yield return Head(url, name, delegate (bool isErr, string err, string header)
            {
                if (isErr)
                {
                    Debugger.Log(err);
                }
                callback(isErr, err, header);
            });
        }

        internal static IEnumerator GetResponseCode(string url, Action<bool, string, long> callback)
        {
            yield return Head(url, delegate (bool isErr, string err, long code)
            {
                if (isErr)
                {
                    Debugger.Log(err);
                }
                callback(isErr, err, code);
            });
        }

        private static IEnumerator Get(string url, Action<bool, string, byte[]> callback)
        {
            using (var www = UnityWebRequest.Get(url))
            {
                foreach (var header in Headers)
                    www.SetRequestHeader(header.Key, header.Value);
                yield return www.Send();
                callback(www.isError, www.error, www.downloadHandler.data);
            }
        }

        private static IEnumerator Get(string url, Action<bool, string, string> callback)
        {
            using (var www = UnityWebRequest.Get(url))
            {
                foreach (var header in Headers)
                    www.SetRequestHeader(header.Key, header.Value);
                yield return www.Send();
                callback(www.isError, www.error, www.downloadHandler.text);
            }
        }

        private static IEnumerator Head(string url, Action<bool, string, long> callback)
        {
            using (var www = UnityWebRequest.Head(url))
            {
                foreach (var header in Headers)
                    www.SetRequestHeader(header.Key, header.Value);
                yield return www.Send();

                callback(www.isError, www.error, www.responseCode);
            }
        }

        private static IEnumerator Head(string url, string name, Action<bool, string, string> callback)
        {
            using (var www = UnityWebRequest.Head(url))
            {
                foreach (var header in Headers)
                    www.SetRequestHeader(header.Key, header.Value);
                yield return www.Send();

                callback(www.isError, www.error, www.GetResponseHeader(name));
            }
        }
    }
}