using UnityEngine;
using System.Collections;

namespace dbga
{
    public class CollisionsCoordinator : MonoBehaviour
    {
        public static CollisionsCoordinator SharedInstance = null;

        [SerializeField]
        private Collider2D topBorder = null;
        public Bounds TopBorderBounds
        {
            get { return topBorder != null ? topBorder.bounds : new Bounds(); }
        }

        [SerializeField]
        private Collider2D bottomBorder = null;
        public Bounds BottomBorderBounds
        {
            get { return bottomBorder != null ? bottomBorder.bounds : new Bounds(); }
        }

        [SerializeField]
        private Collider2D leftBorder = null;
        public Bounds LeftBorderBounds
        {
            get { return leftBorder != null ? leftBorder.bounds : new Bounds(); }
        }

        [SerializeField]
        private Collider2D rightBoder = null;
        public Bounds RightBorderBounds
        {
            get { return rightBoder != null ? rightBoder.bounds : new Bounds(); }
        }

        private Vector2 leftBorderNormal = new Vector2(1.0f, 0.0f);
        public Vector2 LeftBorderNormal
        {
            get { return leftBorderNormal; }
        }

        private Vector2 rightBorderNormal = new Vector2(-1.0f, 0.0f);
        public Vector2 RightBorderNormal
        {
            get { return rightBorderNormal; }
        }

        private Vector2 topBorderNormal = new Vector2(0.0f, -1.0f);
        public Vector2 TopBorderNormal
        {
            get { return topBorderNormal; }
        }

        private Vector2 bottomBorderNormal = new Vector2(0.0f, 1.0f);
        public Vector2 BottomBorderNormal
        {
            get { return bottomBorderNormal; }
        }

        void Awake()
        {
            SharedInstance = this;
        }

        void Start()
        {
        }
    }
}
