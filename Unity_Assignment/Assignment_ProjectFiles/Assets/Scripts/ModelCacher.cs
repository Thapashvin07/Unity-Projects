using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GLTFast;
public class ModelCacher : MonoBehaviour
{
    private static readonly Dictionary<string, GameObject> modelCache = new Dictionary<string, GameObject>();
    public void LoadAndSpawnModel(string modelUrl, Transform parent, Action<GameObject> onSpawned)
    {
        StartCoroutine(LoadModelRoutine(modelUrl, parent, onSpawned));
    }

    private IEnumerator LoadModelRoutine(string modelUrl, Transform parent, Action<GameObject> onSpawned)
    {
        if (string.IsNullOrEmpty(modelUrl))
        {
            Debug.LogWarning("Empty URL no model can be found!");
            onSpawned?.Invoke(null);
            yield break;
        }

        if (modelCache.TryGetValue(modelUrl, out GameObject cachedModel) && cachedModel != null)
        {
            GameObject model = Instantiate(cachedModel, parent);
            model.SetActive(true);
            onSpawned?.Invoke(model);
            yield break;
        }
        using (UnityWebRequest request = UnityWebRequest.Get(modelUrl))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning("Error URL no model can be found!");
                    onSpawned?.Invoke(null);
                    yield break;
                }

                byte[] modelBytes = request.downloadHandler.data;

                var gltfImport = new GltfImport();
                var loadTask = gltfImport.Load(modelBytes, new Uri(modelUrl));

                while (!loadTask.IsCompleted)
                    yield return null;

                bool success = loadTask.Result;
                if (!success)
                {
                    Debug.LogError("Failed to parse glTF/glb data from model downloaded");
                    onSpawned?.Invoke(null);
                    yield break;
                }

                GameObject template = new GameObject("Model_" + modelUrl.GetHashCode());
                var instantiator = new GameObjectInstantiator(gltfImport, template.transform);
                var instantiateTask = gltfImport.InstantiateMainSceneAsync(instantiator);

                while (!instantiateTask.IsCompleted)
                    yield return null;

                template.SetActive(false);
                template.transform.SetParent(this.transform);
                modelCache[modelUrl] = template;

                GameObject spawnModel = Instantiate(template, parent);
                spawnModel.SetActive(true);
                onSpawned?.Invoke(spawnModel);
            }
        }

}
