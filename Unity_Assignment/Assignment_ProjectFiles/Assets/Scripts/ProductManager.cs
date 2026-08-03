using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ProductManager : MonoBehaviour
{
    public static ProductManager Instance { get; private set; }
    private string jsonFileName = "ProductsCatalog.json";
    public FilterState CurrentFilter { get; private set; } = new FilterState();
    private List<Product> _allProducts = new List<Product>();
    public IReadOnlyList<Product> AllProducts => _allProducts;
    public event Action OnProductCatalogueLoaded;
    public event Action<List<Product>> OnFilterApplied;
    public bool isProductsLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        StartCoroutine(LoadProductCatalogueAsync());
    }

    private IEnumerator LoadProductCatalogueAsync()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        string jsonText;
        if (path.Contains("://") || path.Contains(":///"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[ProductManager] Failed to load {jsonFileName}: {request.error}");
                    yield break;
                }
                jsonText = request.downloadHandler.text;
            }
        }
        else
        {
            jsonText = File.ReadAllText(path);
            Debug.Log("jsontxt:"+jsonText);
        }
        ParseProductCatalogue(jsonText);
        OnProductCatalogueLoaded?.Invoke();
    }

    private void ParseProductCatalogue(string product)
    {
        try
        {
            ProductCatalogue catalogue = JsonUtility.FromJson<ProductCatalogue>(product);
            _allProducts = catalogue?.products ?? new List<Product>();
            Debug.Log("Loaded "+_allProducts.Count+" products.");
        }
        catch (Exception e)
        {
            Debug.LogError("JSON products parsing error: "+e.Message);
            _allProducts = new List<Product>();
        }
        isProductsLoaded = true;
    }

    public void ApplyFilter()
    {
        List<Product> results = new List<Product>();
        if(!CurrentFilter.HasAnyFilter)
        {
            results.AddRange(_allProducts);
        }
        else
        {
            for(int i=0;i<_allProducts.Count;i++)
            {
                if(CurrentFilter.Matches(_allProducts[i]))
                {
                    results.Add(_allProducts[i]);
                }
            }
        }
        if (!string.IsNullOrEmpty(CurrentFilter.SearchQuery)) results = ProductSearch.Search(results, CurrentFilter.SearchQuery);
        OnFilterApplied?.Invoke(results);
    }

    public void ResetAllFilters()
    {
        CurrentFilter.Reset();
        OnFilterApplied?.Invoke(new List<Product>(_allProducts));
    }

    public List<Product> GetProductsForSubcategory(string category, string subcategory)
    {
        List<Product> matches = new List<Product>();
        for (int i = 0; i < _allProducts.Count; i++)
        {
            var prod = _allProducts[i];
            if (prod.category == category && prod.subcategory == subcategory)
                matches.Add(prod);
        }
        return matches;
    }
}


[Serializable]
public class Product
{
    public string productId;
    public string name;
    public string category;
    public string subcategory;
    public string description;
    public string thumbnailUrl;
    public string modelUrl;
    public Vector3 scale;
    public Vector3 rotation;
    public Vector3 position;
}
[Serializable]
public class ProductCatalogue
{
    public List<Product> products;
}
