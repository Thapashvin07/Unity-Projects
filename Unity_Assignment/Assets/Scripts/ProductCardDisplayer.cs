using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProductCardDisplayer : MonoBehaviour
{
    [SerializeField] 
    private RawImage thumbnail_icon;
    [SerializeField] 
    private TMP_Text productName;
    [SerializeField] 
    private TMP_Text categoryName;
    [SerializeField] 
    private TMP_Text subcategoryName;

    private string curUrl;

    public void Bind(Product product)
    {
        productName.text = product.name;
        categoryName.text = product.category;
        subcategoryName.text = product.subcategory;
        thumbnail_icon.texture = null;
        curUrl = product.thumbnailUrl;

        // if (loadingSpinner != null) loadingSpinner.SetActive(true);
        TextureCacher.Instance.GetTexture(product.thumbnailUrl, (tex) =>
        {
            if (curUrl != product.thumbnailUrl) return;

            // if (loadingSpinner != null) loadingSpinner.SetActive(false);
            if (tex != null) thumbnail_icon.texture = tex;
        });
    }
}
