using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class StoreHandler : MonoBehaviour
{
    [SerializeField] private RecycleScroll recycleScroll;
    [SerializeField] private GameObject emptyMsg;
    //detailproductview tbh
    [SerializeField] private const int portraitColumns = 2;
    [SerializeField] private const int tabletLandscapeColumns = 3;
    [SerializeField] private float tabletAspectThreshold = 1.2f;
    [SerializeField] GameObject notifyFilter;
    [SerializeField] ProductDetailPanelController productviewPanel;
    [SerializeField] private TMP_InputField searchInput;
    private void Awake()
    {
        recycleScroll.OnCardClicked += HandleCardClicked;
        recycleScroll.OnBindCard = (product, cardObj) => cardObj.GetComponent<ProductCardDisplayer>().Bind(product);
    }
    private void Start()
    {
        ProductManager.Instance.OnProductCatalogueLoaded += HandleCatalogueLoaded;
        ProductManager.Instance.OnFilterApplied += HandleFilterApplied;
        ApplyResponsiveColumnCountForDevice();
        if(ProductManager.Instance.isProductsLoaded) HandleCatalogueLoaded();
        searchInput.onValueChanged.AddListener(OnSearchTextChanged);
    }
    public void ApplyResponsiveColumnCountForDevice()
    {
        float aspect = (float)Screen.width / Screen.height;
        int columns =(aspect >= tabletAspectThreshold)? tabletLandscapeColumns : portraitColumns;
        recycleScroll.SetColumnCount(columns);
    }

    private void HandleCatalogueLoaded()
    {
        var all = new List<Product>(ProductManager.Instance.AllProducts);
        ShowResults(all);
        // notifyFilter.SetActive(false);
    }

    private void HandleFilterApplied(List<Product> results)
    {
        ShowResults(results);
    }

    private void OnSearchTextChanged(string query)
    {
        ProductManager.Instance.CurrentFilter.SearchQuery = query;
        ProductManager.Instance.ApplyFilter();
    }

    private void ShowResults(List<Product> results)
    {
        if(results.Count == ProductManager.Instance.AllProducts.Count) notifyFilter.SetActive(false);
        else notifyFilter.SetActive(true);
        bool empty = results.Count == 0;
        emptyMsg.SetActive(empty);
        recycleScroll.gameObject.SetActive(!empty);

        if (!empty)
            recycleScroll.SetData(results);

    }

    private void HandleCardClicked(Product product)
    {
        productviewPanel.Show(product);//maindetail panel tbh
    }
}
