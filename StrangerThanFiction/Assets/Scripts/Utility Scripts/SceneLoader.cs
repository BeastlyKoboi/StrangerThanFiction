using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
// Implementation mostly draws from this source.
// In the future, I imagine each scene will have their own scene loader. 
// And when the scene loader is passed to the next scene, it destroys itself. 
// https://gamedevbeginner.com/how-to-load-a-new-scene-in-unity-with-a-loading-screen/#load_vs_load_async
public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public GameObject canvas;
    public CanvasGroup canvasGroup;
    public string sceneToLoad;
    public UnityEvent fadeFinished;
    public GameObject transitionTemplate;
    public TransitionData transitionData;

    [SerializeField] private float _fadeDuration = .3f;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene()
    {
        StartCoroutine(StartLoad(sceneToLoad));
    }
    public void LoadScene(string gameScene)
    {
        StartCoroutine(StartLoad(gameScene));
    }

    public void SetTemplate(GameObject transition)
    {
        transitionTemplate = transition;
    }

    public void SetTemplateData(TransitionData data)
    {
        transitionData = data;
    }

    IEnumerator StartLoad(string gameScene)
    {
        loadingScreen.SetActive(true);
        yield return StartCoroutine(FadeLoadingScreen(1, _fadeDuration));
        DataPersistenceManager.instance.SaveGame();
        AsyncOperation load = SceneManager.LoadSceneAsync(gameScene);

        // Check if given transition template
        if (transitionTemplate != null)
        {
            // Create template as child
            GameObject transitionsParent = Instantiate(transitionTemplate, canvas.transform);
            transitionsParent.transform.SetAsFirstSibling();

            // loop through each card in template,
            foreach (Transform card in transitionsParent.transform)
            {
                // set card to inactive
                card.gameObject.SetActive(false);
            }

            for (int i = 0; i < transitionsParent.transform.childCount; i++)
            {
                // setting the card active and then start their animations
                GameObject transition = transitionsParent.transform.GetChild(i).gameObject;
                transition.SetActive(true);

                yield return StartCoroutine(FadeLoadingScreen(0, _fadeDuration));

                TransitionCard transitionCard = transition.GetComponent<TransitionCard>();
                transitionCard.StartTransition();

                while (!transitionCard.HasAnimationFinished)
                {
                    yield return null;
                }

                // wait for animations to finish before fading to black
                yield return StartCoroutine(FadeLoadingScreen(1, _fadeDuration));

                transitionsParent.transform.GetChild(i).gameObject.SetActive(false);
            }

            // fade from black
            yield return StartCoroutine(FadeLoadingScreen(1, _fadeDuration));

            // once all animations are done, load the scene
        }


        while (!load.isDone)
        {
            yield return null;
        }
        yield return StartCoroutine(FadeLoadingScreen(0, _fadeDuration));
        loadingScreen.SetActive(false);
        fadeFinished?.Invoke();
        Destroy(gameObject);
    }

    IEnumerator FadeLoadingScreen(float targetValue, float duration)
    {
        float startValue = canvasGroup.alpha;
        float time = 0;
        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetValue;
    }
}
