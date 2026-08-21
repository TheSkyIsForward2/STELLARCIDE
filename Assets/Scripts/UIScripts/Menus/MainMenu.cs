using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void Play()
    {
        // check for tutorial flag
        // if (PlayerPrefs.GetInt("Tutorial") == 0) SceneManager.LoadScene("Tutorial");
        // otherwise play game again
        SceneManager.LoadScene("ShowcaseScene");
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
