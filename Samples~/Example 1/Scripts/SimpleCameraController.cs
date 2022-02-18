using UnityEngine;

namespace SimpleSave.Samples.Example1
{
    public class SimpleCameraController : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private Transform root;
        [SerializeField] private float moveSmoothness;
        [SerializeField] private float rotSmoothness;

        private void FixedUpdate()
        {
            if (UiManager.Instance.MenuIsOpen)
            {
                return;
            }

            transform.position = Vector3.Lerp(transform.position, root.position, moveSmoothness * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, root.rotation, rotSmoothness * Time.deltaTime);
        }
    }
}