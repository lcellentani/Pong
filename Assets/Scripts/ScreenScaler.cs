using UnityEngine;
using System.Collections;

namespace dbga
{
    [ExecuteInEditMode]
    public class ScreenScaler : MonoBehaviour
    {
        public float designScreenWidth = 1136.0f;
        public float designScreenHeight = 640.0f;
        public float pixelsToUnits = 100.0f;

        private float desiredRatio;
        private float currentRatio;
        private float screenWidth;
        private float screenHeight;
        private float desiredWidth;

        void Start()
        {

        }

        void Update()
        {
            relayout();  
        }

        private void relayout()
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;

            desiredRatio = designScreenWidth / designScreenHeight;
            currentRatio = screenWidth / screenHeight;

            if (currentRatio >= desiredRatio)
            {
                Camera.main.orthographicSize = designScreenHeight * 0.5f / pixelsToUnits;
            }
            else
            {
                float differenceInSize = desiredRatio / currentRatio;
                Camera.main.orthographicSize = designScreenHeight * 0.5f / pixelsToUnits * differenceInSize;
            }

            //desiredWidth = (designResolutionWidth / designResolutionHeight) * screenHeight;
            //Screen.SetResolution((int)desiredWidth, (int)screenHeight, true);
        }
    }
}