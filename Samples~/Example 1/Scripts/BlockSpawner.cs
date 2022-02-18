using UnityEngine;

namespace SimpleSave.Samples.Example1
{
    public class BlockSpawner : SimpleSaveBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject _blockPrefab;
        [SerializeField] private Vector3 _cubeSize;

        [SerializeField] private int _buildingCount;
        [SerializeField] private Vector2 _spawnArea;

        [SerializeField] private int _minHeight;
        [SerializeField] private int _maxHeight;

        [SerializeField] private int _minWidth;
        [SerializeField] private int _maxWidth;

        [SerializeField] private int _minLength;
        [SerializeField] private int _maxLength;

        protected override void AwakeNormal()
        {
            for (int i = 0; i < _buildingCount; i++)
            {
                SpawnBuilding();
            }
        }

        private void SpawnBuilding()
        {
            var spawnPos = GetRandomSpawnPosition();

            var randomHeight = Random.Range(_minHeight, _maxHeight + 1);
            var randomWidth = Random.Range(_minWidth, _maxWidth + 1);
            var randomLength = Random.Range(_minLength, _maxLength + 1);

            for (int heightLevel = 0; heightLevel < randomHeight; heightLevel++)
            {
                for (int widthLevel = 0; widthLevel < randomWidth; widthLevel++)
                {
                    for (int lengthLevel = 0; lengthLevel < randomLength; lengthLevel++)
                    {
                        SpawnCubeAt(spawnPos + new Vector3(widthLevel , heightLevel , lengthLevel));
                    }
                }
            }
        }

        private void SpawnCubeAt(Vector3 position)
        {
            Instantiate(_blockPrefab, position, Quaternion.identity, transform);
        }

        private Vector3 GetRandomSpawnPosition()
        {
            var spawnPosX = Random.Range(-_spawnArea.x, _spawnArea.x);
            var spawnPosZ = Random.Range(-_spawnArea.y, _spawnArea.y);

            var spawnPos = new Vector3(spawnPosX, 0, spawnPosZ);
            return spawnPos + new Vector3(0, _cubeSize.y / 2, 0);
        }
    }
}
