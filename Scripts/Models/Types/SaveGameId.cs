using UnityEngine;
using System;

namespace SimpleSave.Models
{
	/// <summary>
    /// Unique identifier of <see cref="SerializableSaveGame"/>.
    /// </summary>
    /// <remarks>This type is serializable in the inspector.</remarks>
    [Serializable]
    public struct SaveGameId
    {
        [SerializeField] private string _value;

        public SaveGameId(string value)
        {
            if (value == string.Empty)
            {
                value = "";
            }

            this._value = value;
        }

        public static implicit operator string(SaveGameId key)
        {
            return key._value;
        }

        public static implicit operator SaveGameId(string identifier)
        {
            return new SaveGameId(identifier);
        }

        public static bool operator ==(SaveGameId a, SaveGameId b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(SaveGameId a, SaveGameId b)
        {
            return !(a == b);
        }

        public bool Equals(SaveGameId other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            return other is SaveGameId otherId && Equals(otherId);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return this == Invalid ? nameof(Invalid) : _value;
        }

        public static readonly SaveGameId Invalid = new SaveGameId();
    }
}