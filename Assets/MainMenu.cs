using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public GameObject loadingText;
    [SerializeField] public GameObject buttonNew;
    [SerializeField] public GameObject buttonExit;
    private void Start()
    {
        Cursor.visible=true;
        Cursor.lockState=CursorLockMode.Confined;
    }
    public void NewGame()
    {
        buttonNew.SetActive(false);
        buttonExit.SetActive(false);
        loadingText.SetActive(true);
        StartCoroutine(LoadYourAsyncScene());
        //SceneManager.LoadScene("mvp — kopia");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        yield return new WaitForSeconds(0.1f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("mvp");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
