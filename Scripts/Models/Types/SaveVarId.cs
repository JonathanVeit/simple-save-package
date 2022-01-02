using UnityEngine;
using System;

namespace SimpleSave.Models
{
    /// <summary>
    /// Unique identifier of <see cref="SaveVarInfo"/>.
    /// </summary>
    /// <remarks>This type is serializable in the inspector.</remarks>
    [Serializable]
    public struct SaveVarId
    {
        [SerializeField] private int _value;

        public SaveVarId(int value)
        {
            if (value == 0)
            {
                throw new ArgumentException("Invalid identifier");
            }

            this._value = value;
        }

        public static implicit operator int(SaveVarId key)
        {
            return key._value;
        }

        public static implicit operator SaveVarId(int identifier)
        {
            return new SaveVarId(identifier);
        }

        public static bool operator ==(SaveVarId a, SaveVarId b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(SaveVarId a, SaveVarId b)
        {
            return !(a == b);
        }

        public bool Equals(SaveVarId other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            return other is SaveVarId otherId && Equals(otherId);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return this == Invalid ? nameof(Invalid) : _value.ToString();
        }

        public static readonly SaveVarId Invalid = new SaveVarId();
    }
}