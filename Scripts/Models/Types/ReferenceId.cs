using UnityEngine;
using System;

namespace SimpleSave.Models
{
    /// <summary>
    /// Unique identifier of <see cref="ReferenceInfo"/>.
    /// </summary>
    /// <remarks>This type is serializable in the inspector.</remarks>
    [Serializable]
    public struct ReferenceId
    {
        [SerializeField] private int _value;

        public ReferenceId(int value)
        {
            if (value == 0)
            {
                throw new ArgumentException("Invalid identifier");
            }

            this._value = value;
        }

        public static implicit operator int(ReferenceId key)
        {
            return key._value;
        }

        public static implicit operator ReferenceId(int identifier)
        {
            return new ReferenceId(identifier);
        }

        public static bool operator ==(ReferenceId a, ReferenceId b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(ReferenceId a, ReferenceId b)
        {
            return !(a == b);
        }

        public bool Equals(ReferenceId other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            return other is ReferenceId otherId && Equals(otherId);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static readonly ReferenceId Invalid = new ReferenceId();
    }
}