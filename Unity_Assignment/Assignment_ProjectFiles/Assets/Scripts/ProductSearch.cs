using UnityEngine;
using System.Collections.Generic;

//This is a straight forward or no brainer search solution used due totime constraints
public class ProductSearch : MonoBehaviour
{
    public static List<Product> Search(List<Product> allProducts, string searchquery)
    {
        List<Product> results = new List<Product>();
        if (string.IsNullOrEmpty(searchquery)) 
        {
            results.AddRange(allProducts);
            return results;
        }

        string lowerQuery = searchquery.ToLower();
        foreach (Product p in allProducts)
        {
            if (p.name.ToLower().Contains(lowerQuery) || p.category.ToLower().Contains(lowerQuery))
                results.Add(p);
        }
        return results;
    }
}
