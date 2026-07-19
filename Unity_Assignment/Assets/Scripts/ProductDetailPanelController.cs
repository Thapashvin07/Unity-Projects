using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProductDetailPanelController : MonoBehaviour
{
    [SerializeField] private GameObject panelObj;
    [SerializeField] private RectTransform rect;
    [SerializeField] private RawImage largeImage;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text categoryLabel;
    [SerializeField] private TMP_Text subcategoryLabel;
    [SerializeField] private TMP_Text descriptionLabel;
    [SerializeField] private Button view3DButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private ModelViewPanelController modelViewerPanel;
    [SerializeField] Animator animator;
    private Product currentProduct;
    public const string Close = "close";

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseAnim);
        view3DButton.onClick.AddListener(Open3DViewer);
        panelObj.SetActive(false);
    }

   public void Show(Product product)
    {
        currentProduct = product;
        nameLabel.text = product.name;
        categoryLabel.text = product.category;
        subcategoryLabel.text = product.subcategory;
        descriptionLabel.text = product.description;

        largeImage.texture = null;
        TextureCacher.Instance.GetTexture(product.thumbnailUrl, (tex) =>
        {
            if (currentProduct == product && tex != null)
                largeImage.texture = tex;
        });

        panelObj.SetActive(true);
    }

    public void CloseAnim()
    {
        animator.SetBool(Close,true);
    }
    public void ClosePanel()
    {
        panelObj.SetActive(false);
        rect.anchoredPosition = new Vector2(0,0);
    }

    private void Open3DViewer()
    {
        modelViewerPanel.Open(currentProduct);
    }
}
