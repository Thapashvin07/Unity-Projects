using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterPanelManager : MonoBehaviour
{
    [SerializeField] 
    private Toggle watchesToggle;
    [SerializeField]
    private Toggle clothesToggle;
    [SerializeField]
    private Toggle jewelleryToggle;
    [SerializeField] 
    private Toggle maleToggle;
    [SerializeField]
    private Toggle femaleToggle;
    [SerializeField]
    private Toggle kidsBoyToggle;
    [SerializeField]
    private Toggle kidsGirlToggle;
    [SerializeField] 
    private Button applyButton;
    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private Button closeButton;
    [SerializeField] 
    private GameObject subcategoryContainer;
    [SerializeField]
    FilterItemScrollManager itemScrollManager;
    [SerializeField] RectTransform rect;
    [SerializeField] Animator animator;
    const string Close = "close";
    void Start()
    {
        watchesToggle.onValueChanged.AddListener(_ => OnCategoryToggled("Watches"));
        clothesToggle.onValueChanged.AddListener(_ => OnCategoryToggled("Clothes"));
        jewelleryToggle.onValueChanged.AddListener(_ => OnCategoryToggled("Jewellery"));

        maleToggle.onValueChanged.AddListener(_ => OnSubcategoryToggled("Male"));
        femaleToggle.onValueChanged.AddListener(_ => OnSubcategoryToggled("Female"));
        kidsBoyToggle.onValueChanged.AddListener(_ => OnSubcategoryToggled("Kids-Boy"));
        kidsGirlToggle.onValueChanged.AddListener(_ => OnSubcategoryToggled("Kids-Girl"));

        applyButton.onClick.AddListener(OnApplyClicked);
        resetButton.onClick.AddListener(OnResetClicked);
        closeButton.onClick.AddListener(CloseAnim);        
        UpdateSubcategoryVisibility();
    }

    public void CloseAnim()
    {
        animator.SetBool(Close,true);
    }

    public void CloseFilterPanel()
    {
        this.gameObject.SetActive(false);
        rect.anchoredPosition = new Vector2(0,0);
    }

    public void OpenFilterPanel()
    {
        this.gameObject.SetActive(true);
    }

    private void OnCategoryToggled(string category)
    {
        ProductManager.Instance.CurrentFilter.ToggleCategory(category);
        UpdateSubcategoryVisibility();
        UpdateItemScrollList();
    }

    private void OnSubcategoryToggled(string subcategory)
    {
        ProductManager.Instance.CurrentFilter.ToggleSubcategory(subcategory);
        UpdateItemScrollList();
    }

    private void UpdateSubcategoryVisibility()
    {
        bool anyCategorySelected = ProductManager.Instance.CurrentFilter.SelectedCategories.Count > 0;
        subcategoryContainer.SetActive(anyCategorySelected);
    }
    void UpdateItemScrollVisibility()
    {
        if(ProductManager.Instance.CurrentFilter.SelectedSubcategories.Count > 0) itemScrollManager.gameObject.SetActive(true);
        else itemScrollManager.gameObject.SetActive(false);
    }

    public void UpdateItemScrollList()
    {
        UpdateItemScrollVisibility();
        itemScrollManager.RefreshItems
        (
            ProductManager.Instance.CurrentFilter.SelectedCategories,
            ProductManager.Instance.CurrentFilter.SelectedSubcategories
        );
    }

    private void OnApplyClicked()
    {
        ProductManager.Instance.ApplyFilter();
        CloseFilterPanel();
    }

    private void OnResetClicked()
    {
        watchesToggle.SetIsOnWithoutNotify(false);
        clothesToggle.SetIsOnWithoutNotify(false);
        jewelleryToggle.SetIsOnWithoutNotify(false);
        maleToggle.SetIsOnWithoutNotify(false);
        femaleToggle.SetIsOnWithoutNotify(false);
        kidsBoyToggle.SetIsOnWithoutNotify(false);
        kidsGirlToggle.SetIsOnWithoutNotify(false);

        ProductManager.Instance.ResetAllFilters();
        itemScrollManager.ClearItems();
        UpdateSubcategoryVisibility();
    }
}
