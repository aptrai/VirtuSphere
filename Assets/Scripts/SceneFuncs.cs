using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFuncs : MonoBehaviour
{
    public GameObject Orange;
    public GameObject Main;
    public CanvasGroup Fade;
    public CanvasGroup Logo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Orange.SetActive(true);
        Main.SetActive(true);

        StartCoroutine(Loading());
    }

    private IEnumerator Loading() {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
