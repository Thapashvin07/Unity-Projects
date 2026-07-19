using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class RecycleScroll : MonoBehaviour
{
    [SerializeField] private int columnCount = 2;
    [SerializeField] private float cardWidth = 300f;
    [SerializeField] private float cardHeight = 350f;
    [SerializeField] private float spacing = 16f;
    [SerializeField] private int rowBuffer = 2;

    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject cardPrefab;
    private ObjectPooler pool;
    private List<Product> data = new List<Product>();
    private readonly Dictionary<int, GameObject> activeCards = new Dictionary<int, GameObject>();
    private int firstVisibleRow = -1;
    private int lastVisibleRow = -1;

    public System.Action<Product, GameObject> OnBindCard;
    public System.Action<Product> OnCardClicked;

    private void Awake() {
        pool = new ObjectPooler(cardPrefab, content, columnCount * (rowBuffer * 2 + 3));
        scrollRect.onValueChanged.AddListener(_ => UpdateVisibleProductCards());
    }

    public void SetData(List<Product> products)
    {
        foreach (var kvp in activeCards) pool.Release(kvp.Value);
        activeCards.Clear();
        firstVisibleRow = -1;
        lastVisibleRow = -1;

        data = products;
        RefreshLayout();
        // scrollRect.verticalNormalizedPosition = 1f;
        UpdateVisibleProductCards();
    }

    public void SetColumnCount(int columns)
    {
        columnCount = Mathf.Max(1, columns);
        RefreshLayout();
    }

    private void RefreshLayout()
    {
        int rowCount = Mathf.CeilToInt(data.Count / (float)columnCount);
        float totalHeight = rowCount * (cardHeight + spacing) + spacing;
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
    }

    private void UpdateVisibleProductCards()
    {
        if (data.Count == 0) return;

        float scrollY = content.anchoredPosition.y;
        float viewportHeight = viewport.rect.height;

        int firstRow = Mathf.Max(0, Mathf.FloorToInt(scrollY / (cardHeight + spacing)) - rowBuffer);
        int lastRow = Mathf.CeilToInt((scrollY + viewportHeight) / (cardHeight + spacing)) + rowBuffer;

        int totalRows = Mathf.CeilToInt(data.Count / (float)columnCount);
        lastRow = Mathf.Min(lastRow, totalRows - 1);

        if (firstRow == firstVisibleRow && lastRow == lastVisibleRow) return;

        int firstIndex = firstRow * columnCount;
        int lastIndex = Mathf.Min(data.Count - 1, (lastRow + 1) * columnCount - 1);
        var toRemove = new List<int>();
        foreach (var kvp in activeCards)
        {
            if (kvp.Key < firstIndex || kvp.Key > lastIndex)
            {
                pool.Release(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }
        foreach (var idx in toRemove) activeCards.Remove(idx);

        for (int i = firstIndex; i <= lastIndex; i++)
        {
            if (activeCards.ContainsKey(i)) continue;

            GameObject card = pool.Get();
            PositionCard(card, i);
            BindCard(card, i);
            activeCards[i] = card;
        }

        firstVisibleRow = firstRow;
        lastVisibleRow = lastRow;
    }

    private void BindCard(GameObject card, int index)
    {
        Product product = data[index];
        OnBindCard?.Invoke(product, card);

        var button = card.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnCardClicked?.Invoke(product));
        }
    }
    private void PositionCard(GameObject card, int index)
    {
        int row = index / columnCount;
        int col = index % columnCount;

        var rt = card.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(
            spacing + col * (cardWidth + spacing),
            -(spacing + row * (cardHeight + spacing))
        );
        rt.sizeDelta = new Vector2(cardWidth, cardHeight);
    }
}
