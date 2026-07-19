using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FilterItemScrollManager : MonoBehaviour
{
    [SerializeField] private RectTransform itemContent;
    [SerializeField] private GameObject itemRowPrefab;
    private readonly List<GameObject> spawnedItems = new List<GameObject>();

    public void RefreshItems(IEnumerable<string> selectedCategories, IEnumerable<string> selectedSubcategories)
    {
        ClearItems();

        foreach (string category in selectedCategories)
        {
            foreach (string subcategory in selectedSubcategories)
            {
                List<Product> matches = ProductManager.Instance.GetProductsForSubcategory(category, subcategory);
                foreach (Product product in matches)
                    SpawnItemRow(product);
            }
        }
    }

    public void ClearItems()
    {
        foreach (var item in spawnedItems) Destroy(item);
        spawnedItems.Clear();
    }

    public void SpawnItemRow(Product product)
    {
        GameObject row = Instantiate(itemRowPrefab, itemContent);
        spawnedItems.Add(row);

        var thumbnail = row.GetComponentInChildren<RawImage>();
        var label = row.GetComponentInChildren<TMP_Text>();
        var button = row.GetComponent<Button>();

        label.text = product.name;

        TextureCacher.Instance.GetTexture(product.thumbnailUrl, (tex) =>
        {
            if (tex != null) thumbnail.texture = tex;
        });

        bool isSelected = ProductManager.Instance.CurrentFilter.SelectedItemIds.Contains(product.productId);
        SetRowHighlight(row, isSelected);

        button.onClick.AddListener(() =>
        {
            ProductManager.Instance.CurrentFilter.ToggleItem(product.productId);
            bool nowSelected = ProductManager.Instance.CurrentFilter.SelectedItemIds.Contains(product.productId);
            SetRowHighlight(row, nowSelected);
        });
    }

    private void SetRowHighlight(GameObject row, bool selected)
    {
        var image = row.GetComponent<Image>();
        if (image != null) image.color = selected ? new Color(0.3f, 0.6f, 1f, 0.4f) : Color.white;
    }
}
