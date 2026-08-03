using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
public class ModelViewPanelController : MonoBehaviour
{
     [SerializeField] private GameObject panelRoot;      // the whole overlay panel, inactive by default
    [SerializeField] private RawImage viewerDisplay;    // shows the Viewer Camera's RenderTexture
    [SerializeField] private Button closeButton;

    [Header("Stage / spawn")]
    [SerializeField] private Transform modelSpawnPoint; // empty Transform on the ProductViewer layer
    [SerializeField] private GameObject fallbackModelPrefab;

    [Header("Gesture input")]
    [SerializeField] private ModelGestureController gestureController;
    [SerializeField] private RectTransform gestureInputArea; // usually same rect as viewerDisplay

    private GameObject currentModelInstance;
    string curModelUrl;
    [SerializeField]
    ModelCacher modelCacher;
    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        panelRoot.SetActive(false);
    }

    public void Open(Product product)
    {
        SpawnModel(product.modelUrl,product.scale,product.rotation,product.position);
        panelRoot.SetActive(true);
    }

    public void Close()
    {
        panelRoot.SetActive(false);
        gestureController.ResetScaleAndRotationWhenDisable();
    }
    private void SpawnModel(string modelUrl, Vector3 scale, Vector3 rotation, Vector3 position)
    {
        if(string.IsNullOrEmpty(modelUrl))
        {
            return;
        }
        if (currentModelInstance != null && modelUrl == curModelUrl)
        {
            gestureController.SetTarget(currentModelInstance.transform);
            return;
        }

        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
            currentModelInstance = null;
        }

        curModelUrl = modelUrl;

        modelCacher.LoadAndSpawnModel(curModelUrl,modelSpawnPoint,(spawnedModel)=>
        {
            if(spawnedModel == null)
            {
                Debug.LogWarning("Failed to load model");
                return;
            }
            spawnedModel.transform.localPosition = position;
            spawnedModel.transform.localRotation = Quaternion.Euler(rotation.x,rotation.y,rotation.z);
            spawnedModel.transform.localScale = scale;
            currentModelInstance = spawnedModel;
            SetLayerForGOHierarchy(currentModelInstance, modelSpawnPoint.gameObject.layer);
            gestureController.SetTarget(currentModelInstance.transform);
        });
    }

    private void SetLayerForGOHierarchy(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerForGOHierarchy(child.gameObject, layer);
    }

}
[Serializable]
public class ModelCategoryMapping
{
    public string modelCategory; // must match Product.modelCategory in JSON, e.g. "Watch", "Clothing"
    public GameObject modelPrefab;
}
