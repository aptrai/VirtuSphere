using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TMP_Text Prev;
    public TMP_Text Next;
    public GameObject TutorialPanel;
    public GameObject[] Pages = new GameObject[6];
    public int currentPage = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnEnable();
    }
    private void OnEnable()
    {
        currentPage = 0;
        Prev.text = "Cancel";
        Next.text = "Next";
        foreach (GameObject go in Pages)
        {
            go.SetActive(false);
        }
        Pages[0].SetActive(true);
    }

    public void NextPage()
    {
        if (currentPage < Pages.Length - 1)
        {
            Pages[currentPage + 1].SetActive(true);
            Pages[currentPage].SetActive(false);
            currentPage++;
        }
        else
        {
            TutorialPanel.SetActive(false);
            OnEnable();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            Pages[currentPage - 1].SetActive(true);
            Pages[currentPage].SetActive(false);
            currentPage--;
        }
        else
        {
            TutorialPanel.SetActive(false);
            OnEnable();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (currentPage == 0)
        {
            Prev.text = "Cancel";
        }
        else if (currentPage == Pages.Length - 1)
        {
            Next.text = "Finish";
        }
        else
        {
            Prev.text = "Back";
            Next.text = "Next";
        }


    }
}
