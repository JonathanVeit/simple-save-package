using UnityEngine;

namespace SimpleSave.Samples.Example2
{
    public class Paddle : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private float speed;
        [SerializeField] private float maxXPos;
        [SerializeField] private float minXPos;
        [SerializeField] private float yPos;
        [SerializeField] private float zPos;

        private void FixedUpdate()
        {
            if (GameManager.Instance.IsPaused)
            {
                return;
            }

            Move();
        }

        private void Move()
        { 
            var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(camRay, out RaycastHit rayCastHit, float.MaxValue))
            {
                var targetPosition = new Vector3(0, yPos, zPos);
                targetPosition.x = Mathf.Clamp(rayCastHit.point.x, minXPos, maxXPos);

                transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            }
        }
    }
}