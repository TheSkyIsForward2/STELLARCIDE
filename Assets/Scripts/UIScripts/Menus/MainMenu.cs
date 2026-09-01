using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        if (!PlayerPrefs.HasKey("TutorialFinished"))
        {
            PlayerPrefs.SetString("TutorialFinished", "no");
        }
    }

    public void Play()
    {
        // check for tutorial flag
        if (PlayerPrefs.GetString("TutorialFinished") == "no") SceneManager.LoadScene("Tutorial");
        // otherwise play game again
        else SceneManager.LoadScene("ShowcaseScene");
    }

    public void Options()
    {
        SceneManager.LoadScene("Options");
    }
    
    public void Leaderboard() 
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void Quit()
    {
        Application.Quit();
    }

    
}
