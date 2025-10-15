using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for IEndDragHandler
using System.Collections.Generic;

/// <summary>
/// Manages the UI Scroll View as a carousel, handling page snapping, 
/// indicator updates, and button navigation.
/// </summary>
public class CarouselManager : MonoBehaviour, IEndDragHandler
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
    [Tooltip("The minimum drag distance (as a fraction of page width) to trigger a page change.")]
    public float swipeThreshold = 0.2f;

    // --- Private State Variables ---
    private List<GameObject> indicators = new List<GameObject>();
    private int pageCount = 0;
    private float pageWidth = 0f;
    private int currentPageIndex = 0;
    private float targetXPosition = 0f;
    
    // Constant for snapping when close to the target to eliminate floating-point error
    private const float snapDistance = 0.5f; 

    void Start()
    {
        if (scrollRect == null || content == null || indicatorPrefab == null || indicatorContainer == null)
        {
            Debug.LogError("CarouselManager: One or more UI references are missing. Please assign them in the Inspector.");
            return;
        }

        pageCount = content.childCount;
        if (pageCount == 0)
        {
            Debug.LogWarning("CarouselManager: Content has no children/pages. Carousel will not function.");
            return;
        }

        // Calculate the width of a single page/item based on the viewport.
        pageWidth = scrollRect.viewport.rect.width;

        SetupIndicators();

        // Initialize the current page and position to the clean start (Page 0).
        currentPageIndex = 0;
        targetXPosition = 0f;
        
        // Immediately snap to the start position to avoid initial offset
        content.anchoredPosition = new Vector2(targetXPosition, content.anchoredPosition.y);

        UpdateIndicators();
    }

    void Update()
    {
        Vector2 newPosition = content.anchoredPosition;

        // 1. CHECK FOR HARD SNAP: If we are close enough to the target, snap exactly.
        if (Mathf.Abs(newPosition.x - targetXPosition) < snapDistance)
        {
            newPosition.x = targetXPosition;
        }
        else
        {
            // 2. SMOOTH MOVE: Otherwise, continue to smoothly move the content towards the target.
            newPosition.x = Mathf.Lerp(newPosition.x, targetXPosition, Time.deltaTime * scrollSpeed);
        }

        content.anchoredPosition = newPosition;

        // Update indicators continuously based on actual scroll position
        UpdateIndicators();
    }

    // --- Core Logic for Page Snapping (Swipe) ---

    /// <summary>
    /// Unity UI event handler called when the user stops dragging the scroll view.
    /// This handles the snapping and swipe threshold logic.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // The negative of the content's X position is the scroll offset.
        float currentContentX = Mathf.Abs(content.anchoredPosition.x);
        
        // Start by calculating the page index that the user is currently closest to (e.g., if they scrolled halfway).
        int nearestPage = Mathf.RoundToInt(currentContentX / pageWidth);

        // Horizontal drag difference (how far and in what direction the user dragged)
        float dragDifference = eventData.position.x - eventData.pressPosition.x; 
        float thresholdInUnits = pageWidth * swipeThreshold;

        // If the drag was intentional (a quick swipe past the threshold), prioritize the swipe direction
        if (Mathf.Abs(dragDifference) > thresholdInUnits)
        {
            // Swiped Right (Content moves left, target page is Previous)
            if (dragDifference > 0) 
            {
                // Go to the previous page
                nearestPage = Mathf.Max(0, currentPageIndex - 1);
            }
            // Swiped Left (Content moves right, target page is Next)
            else 
            {
                // Go to the next page
                nearestPage = Mathf.Min(pageCount - 1, currentPageIndex + 1);
            }
        }
        
        // Finalize the movement by setting the target page
        SetTargetPage(nearestPage);
    }
    
    // --- Public methods for Button Clicks ---

    /// <summary>
    /// Scrolls the carousel to the next page. Call this from your Next button.
    /// </summary>
    public void NextPage()
    {
        // Use centralized setter to handle clamping and calculation
        SetTargetPage(currentPageIndex + 1);
    }

    /// <summary>
    /// Scrolls the carousel to the previous page. Call this from your Previous button.
    /// </summary>
    public void PreviousPage()
    {
        // Use centralized setter to handle clamping and calculation
        SetTargetPage(currentPageIndex - 1);
    }

    /// <summary>
    /// Calculates the target position, updates the currentPageIndex state, and initiates movement.
    /// This is the single source of truth for page changes.
    /// </summary>
    /// <param name="targetIndex">The desired page index.</param>
    private void SetTargetPage(int targetIndex)
    {
        // 1. Clamp the new index to a valid range (0 to pageCount-1)
        currentPageIndex = Mathf.Clamp(targetIndex, 0, pageCount - 1);
        
        // 2. Calculate the target position using clean, precise math
        targetXPosition = -pageWidth * currentPageIndex;
        
        // Stop any current scrolling movement when a button is clicked or a new drag ends
        scrollRect.StopMovement();
    }
    
    // --- Helper Functions (Setup and Indicator logic) ---

    /// <summary>
    /// Creates the indicator dots based on the number of pages.
    /// </summary>
    private void SetupIndicators()
    {
        indicators.Clear();
        foreach (Transform child in indicatorContainer)
        {
            Destroy(child.gameObject);
        }

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
        float contentX = Mathf.Abs(content.anchoredPosition.x);
        
        // Calculate which page is closest to the center of the viewport
        int closestPage = Mathf.RoundToInt(contentX / pageWidth);
        
        int indicatorIndex = Mathf.Clamp(closestPage, 0, pageCount - 1);

        // Update the visual state of all indicators
        for (int i = 0; i < pageCount; i++)
        {
            Transform fill = indicators[i].transform.Find("Fill");
            Transform empty = indicators[i].transform.Find("Empty");

            if (fill != null && empty != null)
            {
                bool isActive = (i == indicatorIndex);
                fill.gameObject.SetActive(isActive);
                empty.gameObject.SetActive(!isActive);
            }
        }
    }
}
