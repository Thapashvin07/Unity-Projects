using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TextureCacher : MonoBehaviour
{
    public static TextureCacher Instance { get; private set; }
    private readonly Dictionary<string,Texture2D> cache = new Dictionary<string,Texture2D>();
    private readonly Dictionary<string, List<Action<Texture2D>>> pendingCallbacks = new Dictionary<string, List<Action<Texture2D>>>();


    private void Awake() {
        if(Instance!=null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance =this;
    }

    public void ClearCache()
    {
        foreach (var texture in cache.Values)
        {
            if (texture != null) Destroy(texture);
        }
        cache.Clear();
    }

    public void GetTexture(string url, Action<Texture2D> onTexLoaded)
    {
        if(string.IsNullOrEmpty(url)) {
            onTexLoaded?.Invoke(null);
            return;
        }
        if(cache.TryGetValue(url, out Texture2D texture))
        {
            onTexLoaded?.Invoke(texture);
            return;
        }
        if(pendingCallbacks.TryGetValue(url,out var callbackList))
        {
            callbackList.Add(onTexLoaded);
            return;
        }
        pendingCallbacks[url] = new List<Action<Texture2D>>
        {
            onTexLoaded
        };
        StartCoroutine(DownloadTexture(url));
        
    }
    private IEnumerator DownloadTexture(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();
            Debug.Log($"[TextureCache] Request finished. Result: {request.result}, Error: {request.error}"+" :"+url);
            Texture2D result = null;

            if (request.result == UnityWebRequest.Result.Success)
            {
                result = DownloadHandlerTexture.GetContent(request);
                cache[url] = result;
            }
            else
            {
                Debug.LogWarning("[TextureCacher] Failed to loading texture from {url}: {request.error}");
            }

            if (pendingCallbacks.TryGetValue(url, out var callbacks))
            {
                foreach (var callback in callbacks)
                    callback?.Invoke(result);

                pendingCallbacks.Remove(url);
            }
        }
    }
}
