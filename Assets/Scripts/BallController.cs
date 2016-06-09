using UnityEngine;
using System.Collections;

namespace dbga
{
    public class BallController : MonoBehaviour
    {
        [SerializeField]
        private GameObject paddleOne;
        [SerializeField]
        private GameObject paddleTwo;
        [SerializeField]
        private Vector2 speed = new Vector2(5.0f, 5.0f);

        [SerializeField]
        private float resetTime = 1.0f;
        [SerializeField]
        private float angle = 10.0f;

        [SerializeField]
        private AudioClip wallCollisionSound;
        [SerializeField]
        private AudioClip paddleCollisionSound;
        [SerializeField]
        private AudioClip scoreSound;

        private Vector2 direction;
        public Vector2 MoveDirection
        {
            get { return direction; }
        }
        private int internalState;
        private float timer = 0;

        // speed will increase by 1% of the actual speed on each collision with paddles. It will be reset to initial value on Reset call
        private Vector2 realSpeed;

        private Transform xform;
        private Collider2D ballCollider;
        private Collider2D paddleOneCollider;
        private Collider2D paddleTwoCollider;

        private Bounds leftBorder;
        private Bounds rightBorder;
        private Bounds topBorder;
        private Bounds bottomBorder;
        private float leftX;
        private float rightX;
        private float topY;
        private float bottomY;

        private AudioSource wallAudioSource;
        private AudioSource paddleAudioSource;
        private AudioSource scoreAudioSource;

        void Awake()
        {
            xform = GetComponent<Transform>();
            ballCollider = GetComponent<Collider2D>();

            wallAudioSource = gameObject.AddComponent<AudioSource>();
            if (wallAudioSource != null)
            {
                wallAudioSource.playOnAwake = false;
            }
            paddleAudioSource = gameObject.AddComponent<AudioSource>();
            if (paddleAudioSource != null)
            {
                paddleAudioSource.playOnAwake = false;
                paddleAudioSource.volume = 0.8f;
            }
            scoreAudioSource = gameObject.AddComponent<AudioSource>();
            if (scoreAudioSource != null)
            {
                scoreAudioSource.playOnAwake = false;
            }
        }

        void Start()
        {
            SetupBorders();

            SetupPaddles();

            Reset();
        }

        void Update()
        {
            switch(internalState)
            {
                case 0:
                    timer -= Time.deltaTime;
                    if (timer < 0)
                    {
                        internalState = 1;
                    }
                    break;
                case 1:
                    UpdatePosition();

                    CheckWallsCollisions();

                    // possiamo escludere che se la ballina collide con la racchetta a sinistra allora nello stesso
                    // momento collida con la racchetta a destra...la cosa e' fisicamente impossibile perche'
                    // le due racchette stanno agli opposti del campo
                    if (paddleOne != null)
                    {
                        // controlliamo la racchetta a sinistra (paddleOne)
                        if (CheckPaddleCollisions(paddleOne.transform.up, paddleOneCollider.bounds))
                        {
                            paddleOne.SendMessage("CollidedWithBall", null, SendMessageOptions.DontRequireReceiver);
                        }
                    }

                    if (paddleTwo!= null)
                    {
                        // controlliamo la racchetta a destra (paddleTwo)
                        if (CheckPaddleCollisions(paddleTwo.transform.up, paddleTwoCollider.bounds))
                        {
                            paddleTwo.SendMessage("CollidedWithBall", null, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                    break;
            }
        }

        private void SetupBorders()
        {
            leftBorder = CollisionsCoordinator.SharedInstance.LeftBorderBounds;
            rightBorder = CollisionsCoordinator.SharedInstance.RightBorderBounds;
            topBorder = CollisionsCoordinator.SharedInstance.TopBorderBounds;
            bottomBorder = CollisionsCoordinator.SharedInstance.BottomBorderBounds;

            leftX = leftBorder.center.x + leftBorder.extents.x;
            rightX = rightBorder.center.x - rightBorder.extents.x;
            topY = topBorder.center.y - topBorder.extents.y;
            bottomY = bottomBorder.center.y + bottomBorder.extents.y;
        }

        private void SetupPaddles()
        {
            if (paddleOne != null)
            {
                paddleOneCollider = paddleOne.GetComponent<Collider2D>();
            }
            if (paddleTwo != null)
            {
                paddleTwoCollider = paddleTwo.GetComponent<Collider2D>();
            }
        }

        private void UpdatePosition()
        {
            //
            // Calcoliamo di quanto si sposta la pallina in base a velocita' e direzione attuale
            float dx = realSpeed.x * direction.x * Time.deltaTime;
            float dy = realSpeed.y * direction.y * Time.deltaTime;

            Vector3 newPosition = xform.position;

            newPosition.x += dx;
            newPosition.y += dy;

            // aggiorniamo la posizione
            xform.position = newPosition;
        }

        private void CheckWallsCollisions()
        {
            Bounds ballBounds = ballCollider.bounds;
            Vector3 position = xform.position;

            float x = position.x;
            float y = position.y;
            // la proprieta' Bounds.extents riporta sempre meta' della dimesione (x = larghezza, y = altezza) dello sprite per ogni asse
            float halfWidth = ballBounds.extents.x;
            float halfHeight = ballBounds.extents.y;

            // bisogna sempre tenere presente che lo sprite della pallina ha una prefissata estensione (larghezza, altezza)

            if ((x < leftX + halfWidth) || (x > rightX - halfWidth))
            {
                if (x < 0)
                {
                    ScoreManager.SharedInstance.IncreaseCpuScore();
                }
                else
                {
                    ScoreManager.SharedInstance.IncreasePlayerScore();
                }

                Reset();

                if (scoreAudioSource != null)
                {
                    scoreAudioSource.PlayOneShot(scoreSound);
                }
            }
            else
            {
                bool collided = false;
                if (y > topY - halfHeight)
                {
                    ReflectDirection(CollisionsCoordinator.SharedInstance.TopBorderNormal);
                    // serve riposizionare lo sprite una volta determinata la nuova direzione
                    y = topY - halfHeight;

                    collided = true;
                }
                else if (y < bottomY + halfHeight)
                {
                    ReflectDirection(CollisionsCoordinator.SharedInstance.BottomBorderNormal);
                    // serve riposizionare lo sprite una volta determinata la nuova direzione
                    y = bottomY + halfHeight;

                    collided = true;
                }

                // una volta gestite le collisione, aggiorniamo la posizione della pallina
                position.x = x;
                position.y = y;
                xform.position = position;

                if (collided)
                {
                    if (wallAudioSource != null)
                    {
                        wallAudioSource.PlayOneShot(wallCollisionSound);
                    }
                }
            }
        }

        private bool CheckPaddleCollisions(Vector3 upVector, Bounds paddleBounds)
        {
            bool result = false;

            Bounds ballBounds = ballCollider.bounds;

            if (paddleBounds.Intersects(ballBounds))
            {
                // leggiamo nuovamente la posizione della pallina in quanto potrebbe essere stata aggiornata dai passaggi precedenti (UpdatePosition o CheckWallsCollisions)
                Vector3 position = xform.position;

                float x = position.x;
                float y = position.y;
                // la proprieta' Bounds.extents riporta sempre meta' della dimesione (x = larghezza, y = altezza) dello sprite per ogni asse
                float halfWidth = ballBounds.extents.x;
                float halfHeight = ballBounds.extents.y;

                float maxX = paddleBounds.center.x + paddleBounds.extents.x;
                float minX = paddleBounds.center.x - paddleBounds.extents.x;
                float maxY = paddleBounds.center.y + paddleBounds.extents.y;
                float minY = paddleBounds.center.y - paddleBounds.extents.y;

                //@note: manage condition for PaddleTwo...now considering only PaddleOne
                if (x - halfWidth < maxX && direction.x < 0.0f)
                {
                    if (y >= maxY || y <= minY)
                    {
                        direction.x = -direction.x;
                        direction.y = -direction.y;
                    }
                    else
                    {
                        Vector2 normal = CalculatePaddleNormal(upVector, y, paddleBounds);

                        ReflectDirection(normal);
                        x = maxX + halfWidth;
                    }

                    result = true;
                }
                else if (x + halfWidth > minX && direction.x > 0.0f)
                {
                    Debug.Log("OPS!");
                    //@note: questo caso non dovrebbe mai succedere nel momento in cui colpire il muro dietro produce punto per avversario
                    ReflectDirection(new Vector2(-1.0f, 0.0f));
                    x = minX - halfWidth;

                    result = true;
                }

                position.x = x;
                position.y = y;
                xform.position = position;

                realSpeed *= 1.01f;

                if (paddleAudioSource != null)
                {
                    paddleAudioSource.PlayOneShot(paddleCollisionSound);
                }
            }

            return result;
        }

        private void ReflectDirection(Vector2 normal)
        {
            //
            // Questa e' la formula che calcola la perfetta riflessione di un vettore rispetto alla normale nel punto
            // v' = -2 * (dot(v, n)) * n + v
            //
            Vector2 newDirection = -2.0f * Vector2.Dot(direction, normal) * normal + direction;
            direction = newDirection;
        }

        private Vector2 CalculatePaddleNormal(Vector3 n, float ballY, Bounds paddleBounds)
        {
            float maxY = paddleBounds.center.y + paddleBounds.extents.y;
            float minY = paddleBounds.center.y - paddleBounds.extents.y;

            float paddleHeight = paddleBounds.size.y;
            float y = (ballY - minY);

            float normalizedY = y / paddleHeight;
            float tetha = -angle * (1 - normalizedY) + angle * normalizedY;

            // rotate normal
            float a = -(tetha * Mathf.Deg2Rad);
            float nx = n.x * Mathf.Cos(a) - n.y * Mathf.Sin(a);
            float ny = n.x * Mathf.Sin(a) + n.y * Mathf.Cos(a);
            Vector2 rotateNormal = new Vector2(nx, ny);
            return rotateNormal;
        }

        private void Reset()
        {
            xform.position = Vector3.zero;

            direction.x = Random.Range(0, 100) < 50 ? -1.0f : 1.0f;
            direction.y = Random.Range(0, 100) < 50 ? -1.0f : 1.0f;
            direction.Normalize();

            realSpeed = speed;

            internalState = 0;
            timer = resetTime;
        }
    }
}