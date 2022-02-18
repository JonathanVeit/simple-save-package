using UnityEngine;

namespace SimpleSave.Samples.Example1
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleFPSController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float walkingSpeed = 7.5f;
        [SerializeField] private float runningSpeed = 11.5f;
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float lookSpeed = 2.0f;
        [SerializeField] private float lookXLimit = 45.0f;

        [HideInInspector] [SerializeField] private Vector3 moveDirection = Vector3.zero;
        [HideInInspector] [SerializeField] private float rotationX = 0;
        [HideInInspector] [SerializeField] private bool canMove = true;

        private CharacterController characterController;
        
        private void Start()
        {
            characterController = GetComponent<CharacterController>();

            if (!characterController.enabled)
            {
                characterController.enabled = true;
            }
        }

        private void Update()
        {
            if (UiManager.Instance.MenuIsOpen)
            {
                return;
            }

            Move();
        }

        private void Move()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;

            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            characterController.Move(moveDirection * Time.deltaTime);

            if (canMove)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                cameraRoot.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }
    }
}