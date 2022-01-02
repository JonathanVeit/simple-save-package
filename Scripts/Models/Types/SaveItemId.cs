using UnityEngine;
using System;

namespace SimpleSave.Models
{
	/// <summary>
    /// Unique identifier of <see cref="SaveItem"/>.
    /// </summary>
    /// <remarks>This type is serializable in the inspector.</remarks>
    [Serializable]
    public struct SaveItemId
    {
        [SerializeField] private string _value;

        public SaveItemId(string value)
        {
            if (value == string.Empty)
            {
                value = "";
            }

            this._value = value;
        }

        public static implicit operator string(SaveItemId key)
        {
            return key._value;
        }

        public static implicit operator SaveItemId(string identifier)
        {
            return new SaveItemId(identifier);
        }

        public static bool operator ==(SaveItemId a, SaveItemId b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(SaveItemId a, SaveItemId b)
        {
            return !(a == b);
        }

        public bool Equals(SaveItemId other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            return other is SaveItemId otherId && Equals(otherId);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return this == Invalid ? nameof(Invalid) : _value;
        }

        public static readonly SaveItemId Invalid = new SaveItemId();
    }
}