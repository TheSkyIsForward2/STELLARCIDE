using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        // set tutorial preferences
        if (!PlayerPrefs.HasKey("TutorialFinished"))
        {
            PlayerPrefs.SetString("TutorialFinished", "no");
        }

        // set option preferences for first time
        if (!PlayerPrefs.HasKey("ResolutionWidth"))
        {
            PlayerPrefs.SetInt("ResolutionWidth", Screen.width);
        }
        if (!PlayerPrefs.HasKey("ResolutionHeight"))
        {
            PlayerPrefs.SetInt("ResolutionHeight", Screen.height);
        }
        
        if (!PlayerPrefs.HasKey("Fullscreen"))
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
        }

        if (!PlayerPrefs.HasKey("Vsync"))
        {
            PlayerPrefs.SetInt("Vsync", 1);
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
        SceneManager.LoadScene("LeaderboardMenu");
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
