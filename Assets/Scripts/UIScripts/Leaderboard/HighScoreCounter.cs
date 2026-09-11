using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts.Leaderboard
{
    public class HighScoreCounter : MonoBehaviour
    {
        [Header("References")] 
        public GameObject topLayer;
        public GameObject leaderboardgrid;
        public TextMeshProUGUI enemyCounter;
        public TextMeshProUGUI finalScoreCounter;
        public GameObject prompt;
        public Button returnButton;

        private string nameToSave;
        public int finalScore;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            prompt.SetActive(false);
            int enemyPoints = GameManager.Instance.scoreManager.enemiesDefeated * 20;
            enemyCounter.text = "ENEMIES SLAIN: " + GameManager.Instance.scoreManager.enemiesDefeated + " [" + enemyPoints + "]";
            finalScore = enemyPoints;
            List<HighScoreEntry> targets = GameManager.Instance.xmlManager.LoadScores();
            finalScoreCounter.text = "TOTAL SCORE: " + finalScore;
            if (targets.Count < GameManager.Instance.xmlManager.leaderboard.leaderboardMaximum || targets[^1].score < finalScore)
            {
                PromptName();
            }
        }

        public void Continue()
        {
            leaderboardgrid.SetActive(true);
            topLayer.SetActive(false);
        }
        
        public void ChangeName(string name)
        {
            nameToSave = name;
            returnButton.interactable = true;
            GameManager.Instance.xmlManager.AddNewScore(nameToSave, finalScore);
            prompt.SetActive(false);
        }

        public void PromptName()
        {
            returnButton.interactable = false;
            prompt.SetActive(true);
        }
    }
}