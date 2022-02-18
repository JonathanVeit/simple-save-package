using UnityEngine;

namespace SimpleSave.Samples.Example1
{
    public class PlayerGrab : MonoBehaviour, ISimpleSaveCustomSerialization
    {
        [Header("Settings")] 
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float range;
        [SerializeField] private Transform blockRoot;
        [SerializeField] private float holdPosSpeed;
        [SerializeField] private float holdRotSpeed;
        [SerializeField] private float throwForce;

        [Header("Runtime")]
        [SerializeField] [SaveRef] private Block selectedBlock;
        [SerializeField] [SaveRef] private Block currentBlock;

        // Update is called once per frame
        void Update()
        {
            if (UiManager.Instance.MenuIsOpen)
            {
                return;
            }

            if (currentBlock == null)
            {
                GrabBlock();
            }
            else
            {
                ThrowBlock();
            }
        }

        void FixedUpdate()
        {
            if (UiManager.Instance.MenuIsOpen || currentBlock == null)
            {
                return;
            }

            currentBlock.transform.position =
                Vector3.Lerp(currentBlock.transform.position, blockRoot.position, holdPosSpeed * Time.deltaTime);
            currentBlock.transform.rotation =
                Quaternion.Lerp(currentBlock.transform.rotation, blockRoot.rotation, holdRotSpeed * Time.deltaTime);
        }


        private void ThrowBlock()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var dir = Camera.main.transform.forward;
                currentBlock.Release(dir * throwForce);
                currentBlock = null;
            }
        }

        private void GrabBlock()
        {
            var origin = Camera.main.transform.position;
            var direction = Camera.main.transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit rayCastHit, range, targetLayer))
            {
                var blockComponent = rayCastHit.collider.gameObject.GetComponent<Block>();
                if (blockComponent && blockComponent != selectedBlock)
                {
                    selectedBlock?.Deselect();
                    selectedBlock = blockComponent;
                    selectedBlock.Select();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (blockComponent)
                    {
                        currentBlock = blockComponent;
                        currentBlock.Grab();

                        selectedBlock.Deselect();
                        selectedBlock = null;
                    }
                }
            }
            else if (selectedBlock)
            {
                selectedBlock.Deselect();
                selectedBlock = null;
            }
        }

        public string Serialize()
        {
            return string.Empty;
        }

        public void Populate(string serialized)
        {
        }
    }
}
