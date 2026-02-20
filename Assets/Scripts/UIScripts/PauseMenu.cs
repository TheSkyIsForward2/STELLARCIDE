using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public UITest script;
    public GameObject pauseMenu;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public static bool optionsFromGame = false;
    PlayerController playerController;
    void Awake()
    {
        Time.timeScale = 0f;
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        playerController.ToggleControls(false);
    }

    public void ReturnToMainMenu()
    {
        pauseMenu.SetActive(false);
        mainMenu.SetActive(true);
        UITest.mainMenuActive = true;
        optionsFromGame = false;
        playerController.ToggleControls(true);
        AudioManager.Instance.RestartBGM();
    }

    public void Options()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        UITest.optionsMenuActive = true;
        optionsFromGame = true;
        playerController.ToggleControls(true);
    }

    // TODO: move into InputActions Events
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("Esc Pressed");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            UITest.gameActive = true;
            UITest.pauseMenuActive = false;
            playerController.ToggleControls(true);
        }

        if (!UITest.gameActive)
        {
            Time.timeScale = 0f;
        }
    }
}
