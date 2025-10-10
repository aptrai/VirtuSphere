using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages the UI Scroll View to function as a responsive image carousel.
/// Handles page navigation via buttons and updates the visual page indicators.
/// </summary>
public class CarouselManager : MonoBehaviour
{
    [Header("Core UI Components")]
    [Tooltip("The ScrollRect component on this GameObject.")]
    public ScrollRect scrollRect;
    [Tooltip("The 'Content' RectTransform that holds all the carousel items (images).")]
    public RectTransform content;

    [Header("Indicator Setup")]
    [Tooltip("The container GameObject (e.g., Indicators) where the dots will be parented.")]
    public Transform indicatorContainer;
    [Tooltip("The prefab for a single indicator dot (e.g., IndicObj).")]
    public GameObject indicatorPrefab;

    [Header("Scrolling Settings")]
    [Tooltip("The speed at which the content moves between pages.")]
    public float scrollSpeed = 10f;

    // --- Private State Variables ---
    private List<GameObject> indicators = new List<GameObject>();
    private int pageCount = 0;
    private float pageWidth = 0f;
    private int currentPageIndex = 0;
    private float targetXPosition = 0f;

    void Start()
    {
        // Safety check
        if (scrollRect == null || content == null || indicatorPrefab == null || indicatorContainer == null)
        {
            Debug.LogError("CarouselManager: One or more UI references are missing. Please assign them in the Inspector.");
            return;
        }

        // 1. Calculate the number of pages (children of the Content object)
        pageCount = content.childCount;
        if (pageCount == 0)
        {
            Debug.LogWarning("CarouselManager: Content has no children/pages. Carousel will not function.");
            return;
        }

        // 2. Calculate the width of a single page/item.
        // We assume each page takes up the full width of the viewport.
        pageWidth = scrollRect.viewport.rect.width;

        // 3. Set up the indicators
        SetupIndicators();

        // 4. Initialize the current page and position
        currentPageIndex = 0;
        targetXPosition = content.anchoredPosition.x; // Start at current position
        UpdateIndicators();
    }

    void Update()
    {
        // Smoothly move the content towards the target position
        Vector2 newPosition = content.anchoredPosition;
        newPosition.x = Mathf.Lerp(newPosition.x, targetXPosition, Time.deltaTime * scrollSpeed);
        content.anchoredPosition = newPosition;

        // Update indicators continuously to show the active page
        UpdateIndicators();
    }

    /// <summary>
    /// Creates the indicator dots based on the number of pages.
    /// </summary>
    private void SetupIndicators()
    {
        // Clear any existing indicators (useful for editor testing)
        indicators.Clear();
        foreach (Transform child in indicatorContainer)
        {
            Destroy(child.gameObject);
        }

        // Instantiate a new indicator for every page
        for (int i = 0; i < pageCount; i++)
        {
            GameObject indicator = Instantiate(indicatorPrefab, indicatorContainer);
            indicators.Add(indicator);
        }
    }

    /// <summary>
    /// Updates the visual state of the indicators (filled vs. empty).
    /// </summary>
    private void UpdateIndicators()
    {
        // Determine the current page based on the content's scroll position
        // The - sign is because scrolling content left moves the anchoredPosition to the negative X values
        float contentX = Mathf.Abs(content.anchoredPosition.x);
        
        // Calculate which page is closest to the center of the viewport
        int closestPage = Mathf.RoundToInt(contentX / pageWidth);
        
        // Clamp the index to prevent errors
        currentPageIndex = Mathf.Clamp(closestPage, 0, pageCount - 1);

        // Update the visual state of all indicators
        for (int i = 0; i < pageCount; i++)
        {
            // IMPORTANT: Based on your hierarchy image, we look for "Fill" and "Empty" GameObjects
            Transform fill = indicators[i].transform.Find("Fill");
            Transform empty = indicators[i].transform.Find("Empty");

            if (fill != null && empty != null)
            {
                if (i == currentPageIndex)
                {
                    // This is the current page: show Fill, hide Empty
                    fill.gameObject.SetActive(true);
                    empty.gameObject.SetActive(false);
                }
                else
                {
                    // This is not the current page: hide Fill, show Empty
                    fill.gameObject.SetActive(false);
                    empty.gameObject.SetActive(true);
                }
            }
        }
    }

    // --- Public methods for Button Clicks ---

    /// <summary>
    /// Scrolls the carousel to the next page. Call this from your Next button.
    /// </summary>
    public void NextPage()
    {
        if (currentPageIndex < pageCount - 1)
        {
            currentPageIndex++;
            SetTargetPosition(currentPageIndex);
        }
    }

    /// <summary>
    /// Scrolls the carousel to the previous page. Call this from your Previous button.
    /// </summary>
    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            SetTargetPosition(currentPageIndex);
        }
    }

    /// <summary>
    /// Calculates and sets the target scroll position for a given page index.
    /// </summary>
    private void SetTargetPosition(int pageIndex)
    {
        // Calculate the required negative X position to show the target page.
        // For page 0, position is 0. For page 1, position is -pageWidth.
        targetXPosition = -pageWidth * pageIndex;
    }
}

