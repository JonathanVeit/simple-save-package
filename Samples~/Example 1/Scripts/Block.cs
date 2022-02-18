using UnityEngine;

namespace SimpleSave.Samples.Example1
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Rigidbody))]
    public class Block : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material selectedMaterial;

        private MeshRenderer meshRenderer;
        private Rigidbody rigidbody;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            rigidbody = GetComponent<Rigidbody>();
            Deselect();
        }

        public void Select()
        {
            meshRenderer.material = selectedMaterial;
        }

        public void Deselect()
        {
            meshRenderer.material = defaultMaterial;
        }

        public void Grab()
        {
            rigidbody.isKinematic = true;
        }

        public void Release(Vector3 force)
        {
            rigidbody.isKinematic = false;
            rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
