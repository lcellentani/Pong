using UnityEngine;
using System.Collections;

namespace dbga
{
    public class PaddleController : MonoBehaviour
    {
        public float speed = 2.0f;

        public float direction = 0.0f;
        private Transform xform;
        private Collider2D paddleCollider;
        private Bounds topBounds;
        private Bounds bottomBounds;

        private Animator animator;

        void Awake()
        {
            xform = GetComponent<Transform>();
            paddleCollider = GetComponent<Collider2D>();
            animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
#if UNITY_EDITOR
            if (paddleCollider == null)
            {
                Debug.LogWarning("Missing collider on paddle " + name);
                this.enabled = false;
            }
#endif
            topBounds = CollisionsCoordinator.SharedInstance.TopBorderBounds;
            bottomBounds = CollisionsCoordinator.SharedInstance.BottomBorderBounds;

            if (animator != null)
            {
                animator.speed = Random.Range(0.7f, 1.0f);
            }
        }

        void Update()
        {
            UpdatePosition();

            CheckCollisions();
        }

        private void UpdatePosition()
        {
            // compute the new position based on the current direction [-1, 1], current speed and actual dt (frame independent movement variation)
            xform.position += xform.right * direction * speed * Time.deltaTime;
        }

        private void CheckCollisions()
        {
            Vector3 currentPosition = xform.position;
            Bounds paddleBounds = paddleCollider.bounds;
            // Bounds extents property is always half of the size.
            float paddleHalfHeight = paddleBounds.extents.y;

            float maxY = topBounds.center.y - topBounds.extents.y;
            float minY = bottomBounds.center.y + bottomBounds.extents.y;
            float y = currentPosition.y;

            if (y + paddleHalfHeight >= maxY)
            {
                y = maxY - paddleHalfHeight;
            }
            else if (y - paddleHalfHeight <= minY)
            {
                y = minY + paddleHalfHeight;
            }

            currentPosition.y = y;

            xform.position = currentPosition;
        }
    }
}