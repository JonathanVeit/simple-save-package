using UnityEngine;
using System;

namespace SimpleSave.Models
{
	/// <summary>
    /// Unique identifier of <see cref="ComponentInfo"/>.
    /// </summary>
    /// <remarks>This type is serializable in the inspector.</remarks>
    [Serializable]
    public struct ComponentId
    {
        [SerializeField] private string _value;

        public ComponentId(string value)
        {
            if (value == string.Empty)
            {
                value = "";
            }

            this._value = value;
        }

        public static implicit operator string(ComponentId key)
        {
            return key._value;
        }

        public static implicit operator ComponentId(string identifier)
        {
            return new ComponentId(identifier);
        }

        public static bool operator ==(ComponentId a, ComponentId b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(ComponentId a, ComponentId b)
        {
            return !(a == b);
        }

        public bool Equals(ComponentId other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            return other is ComponentId otherId && Equals(otherId);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return this == Invalid ? nameof(Invalid) : _value;
        }

        public static readonly ComponentId Invalid = new ComponentId();
    }
}