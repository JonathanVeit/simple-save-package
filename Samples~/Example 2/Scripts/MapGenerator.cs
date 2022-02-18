using System.Collections.Generic;
using UnityEngine;

namespace SimpleSave.Samples.Example2
{
    public class MapGenerator : MonoBehaviour
    {
        public static MapGenerator Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private int mapBlockCount;

        [Space(5)]
        [SerializeField] private int minPosX;
        [SerializeField] private int maxPosX;
        [SerializeField] private int minPosZ;
        [SerializeField] private int maxPosZ;
        [Space(5)]
        [SerializeField] private float posY;
        [Space(5)]
        [SerializeField] private int minSizeX;
        [SerializeField] private int maxSizeX;
        [SerializeField] private int minSizeZ;
        [SerializeField] private int maxSizeZ;

        private Dictionary<int, HashSet<int>> usedPoints;

        private void Awake()
        {
            Instance = this;
        }

        public void GenerateMap()
        {
            usedPoints = new Dictionary<int, HashSet<int>>();

            for (int i = 0; i < mapBlockCount; i++)
            {
                var pointX = Random.Range(minPosX, maxPosX);
                var pointZ = Random.Range(minPosZ, maxPosZ);

                SpawnAt(new Vector3(pointX, posY, pointZ));
            }
        }

        private void SpawnAt(Vector3 point)
        {
            var sizeX = Random.Range(minSizeX, maxSizeX);
            var sizeZ = Random.Range(minSizeZ, maxSizeZ);

            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    var blockPosition = new Vector3(point.x + x, point.y, point.z + z);

                    if (PositionIsOutOfBoundaries(blockPosition))
                    {
                        continue;
                    }

                    if (PositionWasAlreadyUsed(blockPosition))
                    {
                        continue;
                    }

                    AddPositionToCache(blockPosition);
                    SpawnBlockAt(blockPosition);
                }
            }
        }

        private void AddPositionToCache(Vector3 blockPosition)
        {
            if (!usedPoints.ContainsKey((int)blockPosition.x))
            {
                usedPoints.Add((int)blockPosition.x, new HashSet<int>());
            }

            usedPoints[(int)blockPosition.x].Add((int)blockPosition.z);
        }

        private bool PositionWasAlreadyUsed(Vector3 blockPosition)
        {
            return usedPoints.TryGetValue((int)blockPosition.x, out var usedYPositions)
                   && usedYPositions.Contains((int)blockPosition.x);
        }

        private bool PositionIsOutOfBoundaries(Vector3 blockPosition)
        {
            return blockPosition.x < minPosX || blockPosition.x > maxPosX ||
                   blockPosition.z < minPosZ || blockPosition.z > maxPosZ;
        }

        private void SpawnBlockAt(Vector3 position)
        {
            Instantiate(blockPrefab, position, Quaternion.identity, transform);
        }
    }
}