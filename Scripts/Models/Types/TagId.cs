using UnityEngine;
using System;

namespace SimpleSave.Models
{
	/// <summary>
    /// Unique identifier of a <see cref="TagInfo"/>.
    /// </summary>
    /// <remarks>This type is serializable in the inspector.</remarks>
    [Serializable]
    public struct TagId
    {
        [SerializeField] private string _value;

        public TagId(string value)
        {
            if (value == string.Empty)
            {
                value = "";
            }

            this._value = value;
        }

        public static implicit operator string(TagId key)
        {
            return key._value;
        }

        public static implicit operator TagId(string identifier)
        {
            return new TagId(identifier);
        }

        public static bool operator ==(TagId a, TagId b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(TagId a, TagId b)
        {
            return !(a == b);
        }

        public bool Equals(TagId other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            return other is TagId otherId && Equals(otherId);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return this == Invalid ? nameof(Invalid) : _value;
        }

        public static readonly TagId Invalid = new TagId();
    }
}