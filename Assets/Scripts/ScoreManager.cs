using UnityEngine;
using System.Collections;

namespace dbga
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager SharedInstance;

        [SerializeField]
        private UICoordinator uiCoordinator;

        private int playerScore = 0;
        private int cpuScore = 0;

        void Awake()
        {
            SharedInstance = this;
        }

        public void IncreasePlayerScore()
        {
            playerScore++;

            if (uiCoordinator != null)
            {
                uiCoordinator.SetPlayerScore(playerScore);
            }
        }

        public void IncreaseCpuScore()
        {
            cpuScore++;

            if (uiCoordinator != null)
            {

                uiCoordinator.SetCpuScore(cpuScore);
            }
        }
    }
}