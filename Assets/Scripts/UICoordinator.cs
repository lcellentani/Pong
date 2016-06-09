using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace dbga
{
    public class UICoordinator : MonoBehaviour
    {
        [SerializeField]
        private Text playerScoreText;
        [SerializeField]
        private Text cpuScoreText;

        public void SetPlayerScore(int newScore)
        {
            if (playerScoreText != null)
            {
                playerScoreText.text = string.Format("{0:00}", newScore);
            }
        }

        public void SetCpuScore(int newScore)
        {
            if (cpuScoreText != null)
            {
                cpuScoreText.text = string.Format("{0:00}", newScore);
            }
        }
    }
}