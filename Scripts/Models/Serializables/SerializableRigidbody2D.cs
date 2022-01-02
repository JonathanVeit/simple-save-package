using System;
using UnityEngine;

namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable <see cref="Rigidbody2D"/>.
    /// </summary>
    [Serializable]
    internal struct SerializableRigidbody2D
    {
        public float AngularDrag;
        public float AngularVelocity;
        public RigidbodyType2D BodyType;
        public Vector2 CenterOfMass;
        public CollisionDetectionMode2D CollisionDetectionMode;
        public RigidbodyConstraints2D Constraints;
        public float Drag;
        public bool FreezeRotation;
        public float GravityScale;
        public float Inertia;
        public RigidbodyInterpolation2D Interpolation;
        public bool IsKinematic;
        public float Mass;
        public bool Simulated;
        public RigidbodySleepMode2D SleepMode;
        public bool UseAutoMass;
        public bool UseFullKinematicContacts;
        public Vector2 Velocity;
    }
}