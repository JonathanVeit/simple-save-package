using UnityEngine;

namespace SimpleSave.Samples.Example2
{
    public class Ball : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private float speed;
        [SerializeField] private float detectionRange;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private int randomness;
        [SerializeField] private TrailRenderer trail;

        private void Start()
        {
            trail.Clear();
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance.IsPaused)
            {
                return;
            }
            
            trail.enabled = true;

            Move();
            CheckObstacles();
        }

        private void Move()
        {
            var newPosition = transform.position + transform.forward * speed * Time.deltaTime;
            transform.position = newPosition;
        }

        private void CheckObstacles()
        {
            var origin = transform.position;
            var direction = transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit rayCastHit, detectionRange, obstacleLayer))
            {
                var newDir = Vector3.Reflect(direction, rayCastHit.normal);
                if (Random.Range(0, randomness) == 0)
                {
                    newDir.x += Random.Range(-0.1f, 0.1f);
                    newDir.z += Random.Range(-0.1f, 0.1f);
                }

                transform.forward = newDir;

                if (rayCastHit.collider.gameObject.GetComponent<Block>())
                {
                    Destroy(rayCastHit.collider.gameObject);
                    GameManager.Instance.BlockDestroyed();
                }
                else if (rayCastHit.collider.gameObject.GetComponent<GroundBlock>())
                {
                    GameManager.Instance.GameOver();
                }
            }
        }
    }
}