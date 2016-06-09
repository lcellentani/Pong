using UnityEngine;
using System.Collections;

namespace dbga
{
    public class CpuPlayerController : MonoBehaviour
    {
        [SerializeField]
        private BallController ball;
        [SerializeField]
        private float minTrackingTreshold = 0.0f;
        [SerializeField]
        private float maxTrackingTreshold = 0.0f;
        [SerializeField]
        private float steadyThreshold = 0.1f;

        private Transform xform;
        private PaddleController paddleController;
        private Transform ballXform;
        private float trackingTreshold;

        public float direction;

        void Awake()
        {
            xform = transform;
            paddleController = GetComponent<PaddleController>();
            if (ball != null)
            {
                ballXform = ball.transform;
            }
        }

        void Start()
        {
#if UNITY_EDITOR
            if (paddleController == null)
            {
                Debug.LogWarning("Cannot find PaddleController component");
                this.enabled = false;
            }
#endif
            UpdateTrackingThreshold();
        }

        void Update()
        {
            float moveDirection = CalculateMoveDirection();

            paddleController.direction = moveDirection;
        }

        private float CalculateMoveDirection()
        {
            direction = 0.0f;

            Vector3 position = xform.position;
            Vector3 ballPosition = ballXform.position;

            float targetY = 0.0f;
            if (ballPosition.x >= trackingTreshold && ball.MoveDirection.x > 0.0f)
            {
                targetY = ballPosition.y;
            }

            float diff = targetY - position.y;
            float sign = diff > 0.0f ? 1.0f : -1.0f;
            float err = Mathf.Abs(diff);
            if (err > steadyThreshold)
            {
                direction = sign;
            }
            return direction;
        }

        private void UpdateTrackingThreshold()
        {
            trackingTreshold = Random.Range(minTrackingTreshold, maxTrackingTreshold);
        }

        private void CollidedWithBall()
        {
            UpdateTrackingThreshold();
        }
    }
}
