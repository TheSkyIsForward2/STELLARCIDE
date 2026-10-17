using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UIScripts.Leaderboard
{
    public class HighScoreCounter : MonoBehaviour
    {
        [Header("References")] 
        public GameObject topLayer;
        public GameObject leaderboardgrid;
        public TextMeshProUGUI redDwarfCounter;
        public TextMeshProUGUI yellowDwarfCounter;
        public TextMeshProUGUI redGiantCounter;
        public TextMeshProUGUI totalMapsCounter;
        public TextMeshProUGUI finalMultiplier;
        public TextMeshProUGUI finalScoreCounter;
        public GameObject prompt;
        public Button returnButton;
        
        [Header("Scalable Variables")]
        [Range(0.5f, 10f)] public float redDwarfMultiplier = 1f;
        [Range(0.5f, 10f)] public float redGiantMultiplier = 4f;
        [Range(0.5f, 10f)] public float yellowDwarfMultiplier = 2f;
        [Range(1f, 100f)] public float mapModifier = 20f;
        
        public int finalScore;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // catch for menu version of scene
            if (!topLayer.activeInHierarchy) return;

            #region Game End Score Calc And Text Adjustment

            // scoring + activating high score prompt
            prompt.SetActive(false);
            int redDwarfPoints = 0;
            int redGiantPoints = 0;
            int yellowDwarfPoints = 0;
            

            if (GameManager.Instance.scoreManager != null)
            {
                if (GameManager.Instance.scoreManager.enemiesDefeated != 0)
                {
                    redDwarfPoints = (int)(GameManager.Instance.scoreManager.redDwarfDefeated * GameManager.Instance.difficultySum/2.0f * redDwarfMultiplier);
                    redGiantPoints = (int)(GameManager.Instance.scoreManager.redGiantDefeated * GameManager.Instance.difficultySum/2.0f * redGiantMultiplier);
                    yellowDwarfPoints = (int)(GameManager.Instance.scoreManager.yellowDwarfDefeated * GameManager.Instance.difficultySum/2.0f * yellowDwarfMultiplier);
                }
            }
            // final score calc
            finalScore = redDwarfPoints + redGiantPoints + yellowDwarfPoints + 
                         (int)(GameManager.Instance.scoreManager.totalMapsCleared*GameManager.Instance.difficultySum*mapModifier);
            // text adjustments
            redDwarfCounter.text = "RED DWARVES SLAIN: \t\t" + GameManager.Instance.scoreManager.redDwarfDefeated;
            redGiantCounter.text = "RED GIANTS SLAIN: \t\t" + GameManager.Instance.scoreManager.redGiantDefeated;
            yellowDwarfCounter.text = "YELLOW DWARVES SLAIN: \t" + GameManager.Instance.scoreManager.yellowDwarfDefeated;
            totalMapsCounter.text = "TOTAL MAPS CLEARED: \t\t" + GameManager.Instance.scoreManager.totalMapsCleared;
            finalMultiplier.text = "FINAL MULTIPLIER: \t\t" + GameManager.Instance.difficultySum;
            finalScoreCounter.text = "TOTAL SCORE: \t\t\t" + finalScore;
            
            // score adjusting if reached high score
            List<HighScoreEntry> targets = GameManager.Instance.xmlManager.LoadScores();
            if (targets.Count < GameManager.Instance.xmlManager.leaderboard.leaderboardMaximum || targets[^1].score < finalScore)
            {
                PromptName();
            }

            #endregion
            
        }

        // move from scoring to leaderboard
        public void Continue()
        {
            leaderboardgrid.SetActive(true);
            topLayer.SetActive(false);
        }
        
        // prompt change name method
        public void ChangeName(string name)
        {
            returnButton.interactable = true;
            GameManager.Instance.xmlManager.AddNewScore(name, finalScore);
            print(finalScore);
            prompt.SetActive(false);
        }

        // turn prompt on method
        public void PromptName()
        {
            returnButton.interactable = false;
            prompt.SetActive(true);
        }

        public void Restart()
        {
            SelectorMapGenerator smg = gameObject.AddComponent<SelectorMapGenerator>();
            smg.ClearJson();
            // TODO should the game be started on the map screen or on a first mission?
            //SceneManager.LoadScene("");
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}