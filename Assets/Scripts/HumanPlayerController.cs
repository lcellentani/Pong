using UnityEngine;
using System.Collections;

namespace dbga
{
    public class HumanPlayerController : MonoBehaviour
    {
        private PaddleController paddleController;

        void Awake()
        {
            paddleController = GetComponent<PaddleController>();
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
        }

        void Update()
        {
            // read changes on the Vertical input axis - see Edit | Project Settings | Input for details about axis mapping
            float moveDirection = Input.GetAxis("Vertical") * -1.0f;

            paddleController.direction = moveDirection;
        }
    }
}
