using System;
using UnityEngine;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable <see cref="Rigidbody"/>.
    /// </summary>
    [Serializable]
    internal struct SerializableRigidbody
    {
        public float AngularDrag;
        public Vector3 AngularVelocity;
        public Vector3 CenterOfMass;
        public CollisionDetectionMode CollisionDetectionMode;
        public RigidbodyConstraints Constraints;
        public bool DetectCollisions;
        public float Drag;
        public bool FreezeRotation;
        public Vector3 InertiaTensor;
        public Quaternion InertiaTensorRotation;
        public RigidbodyInterpolation Interpolation;
        public bool IsKinematic;
        public float Mass;
        public float MaxAngularVelocity;
        public float MaxDepenetrationVelocity;
        public float SleepThreshold;
        public int SolverIterations;
        public int SolverVelocityIterations;
        public bool UseGravity;
        public Vector3 Velocity;
    }
}