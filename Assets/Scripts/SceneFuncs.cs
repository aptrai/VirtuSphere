using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class SceneFuncs : MonoBehaviour
{
    public GameObject Orange;
    public GameObject Main;
    public GameObject Catalog;
    public GameObject Tutorial;
    public CanvasGroup Fade;
    public CanvasGroup Logo;
    public GameObject TouchProt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Orange.SetActive(true);
        Main.SetActive(true);
        Tutorial.SetActive(false);
        StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(FadeOut());

        Orange.SetActive(false);

    }

    public void arMode()
    {
        SceneManager.LoadScene(1);
    }

    private IEnumerator FadeOut()
    {

        float elapsed = 0f;
        float startAlpha = Fade.alpha;
        float fadeDuration = 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            Fade.alpha = Mathf.Lerp(startAlpha, 0f, t);
            Logo.alpha = Mathf.Lerp(1f, 0f, t * 20f);
            Debug.Log(Fade.alpha);
            yield return null;
        }

        Fade.alpha = 0f;
    }

    public void OpenCatalog()
    {
        Main.SetActive(false);
        Catalog.SetActive(true);
    }
    public void OpenTutorial()
    {
        Tutorial.SetActive(true);
        TouchProt.SetActive(true);
    }
    public void BackToMenu()
    {
        Main.SetActive(true);
        Catalog.SetActive(false);
        Tutorial.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Tutorial.activeSelf == false)
        {
            TouchProt.SetActive(false);
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                BackToMenu();
            }
        }
    }
}
