using UnityEngine;
using System.Collections.Generic;

public class FilterState
{
    public HashSet<string> SelectedCategories { get; private set; } = new HashSet<string>();
    public HashSet<string> SelectedSubcategories { get; private set; } = new HashSet<string>();
    public HashSet<string> SelectedItemIds { get; private set; } = new HashSet<string>();

    public string SearchQuery { get; set; } = string.Empty;

    public void ToggleCategory(string category)
    {
        if (!SelectedCategories.Remove(category))
            SelectedCategories.Add(category);
    }

    public void ToggleSubcategory(string subcategory)
    {
        if (!SelectedSubcategories.Remove(subcategory))
            SelectedSubcategories.Add(subcategory);
    }

    public void ToggleItem(string productId)
    {
        if (!SelectedItemIds.Remove(productId))
            SelectedItemIds.Add(productId);
    }

    public bool HasAnyFilter =>
        SelectedCategories.Count > 0 ||
        SelectedSubcategories.Count > 0 ||
        SelectedItemIds.Count > 0 ||
        !string.IsNullOrEmpty(SearchQuery);

    public void Reset()
    {
        SelectedCategories.Clear();
        SelectedSubcategories.Clear();
        SelectedItemIds.Clear();
        SearchQuery = string.Empty;
    }

    public bool Matches(Product product)
    {
        if (SelectedItemIds.Count > 0 && SelectedItemIds.Contains(product.productId))
            return true;

        bool Hascategory = SelectedCategories.Count == 0 || SelectedCategories.Contains(product.category);
        bool Hassubcategory = SelectedSubcategories.Count == 0 || SelectedSubcategories.Contains(product.subcategory);
        if (SelectedItemIds.Count > 0)
            return false;

        return (Hascategory && Hassubcategory);
    }
}
