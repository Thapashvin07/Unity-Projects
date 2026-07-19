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
    [SerializeField] private List<ModelCategoryMapping> modelMappings;
    [SerializeField] private GameObject fallbackModelPrefab;

    [Header("Gesture input")]
    [SerializeField] private ModelGestureController gestureController;
    [SerializeField] private RectTransform gestureInputArea; // usually same rect as viewerDisplay

    private GameObject currentModelInstance;
    private string currentModelCategory;
    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        panelRoot.SetActive(false);
    }

    public void Open(Product product)
    {
        SpawnModel(product.modelCategory);
        panelRoot.SetActive(true);
    }

    public void Close()
    {
        panelRoot.SetActive(false);
        gestureController.UpdateResetLerp();
    }
    private void SpawnModel(string modelCategory)
    {
        if (currentModelInstance != null && currentModelCategory == modelCategory)
        {
            gestureController.SetTarget(currentModelInstance.transform);
            return;
        }

        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
            currentModelInstance = null;
        }

        GameObject prefabToSpawn = fallbackModelPrefab;
        foreach (var mapping in modelMappings)
        {
            if (mapping.modelCategory == modelCategory)
            {
                prefabToSpawn = mapping.modelPrefab;
                break;
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"[ModelViewerPanelController] No model found for category '{modelCategory}' and no fallback assigned.");
            return;
        }

        currentModelInstance = Instantiate(prefabToSpawn, prefabToSpawn.transform.position, prefabToSpawn.transform.rotation, modelSpawnPoint);
        currentModelCategory = modelCategory;

        SetLayerForGOHierarchy(currentModelInstance, modelSpawnPoint.gameObject.layer);

        gestureController.SetTarget(currentModelInstance.transform);
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
