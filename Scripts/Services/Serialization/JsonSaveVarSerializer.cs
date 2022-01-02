using System;
using SimpleSave.Extensions;
using UnityEngine;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="ISaveVarSerializer"/>
    internal sealed class JsonSaveVarSerializer : BaseService, ISaveVarSerializer
    {
        /// <inheritdoc />
        public string Serialize(object saveVar)
        {
            switch (saveVar)
            {
                case string asString:
                    return asString;
                case int asInt:
                    return asInt.ToString();
                case bool asBool:
                    return asBool.ToString();
                case float asFloat:
                    return asFloat.ToString();
                case Enum asEnum:
                    return asEnum.ToString();
                case byte asByte:
                    return asByte.ToString();
                case Char asChar:
                    return asChar.ToString();
                case DateTime asDateTime:
                    return asDateTime.ToString();
                case Double asDouble:
                    return asDouble.ToString();
                case Int16 asInt16:
                    return asInt16.ToString();
                case Int64 asInt64:
                    return asInt64.ToString();
                case UInt16 asUInt16:
                    return asUInt16.ToString();
                case UInt32 asUInt32:
                    return asUInt32.ToString();
                case UInt64 asUInt64:
                    return asUInt64.ToString();

                default:
                    try
                    {
                        return JsonUtility.ToJson(saveVar);
                    }
                    catch 
                    {
                        Logger.LogInternal($"Unable to serialize {nameof(SaveVar)} of type {saveVar.GetType().Name} with value {saveVar}.");
                        return string.Empty;
                    }
            }
        }

        /// <inheritdoc />
        public object Deserialize(string serializedVar, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return serializedVar;
                case TypeCode.Int32:
                    return type.IsEnum ? Enum.Parse(type, serializedVar) : int.Parse(serializedVar);
                case TypeCode.Boolean:
                    return bool.Parse(serializedVar);
                case TypeCode.Decimal:
                    return float.Parse(serializedVar);
                case TypeCode.Byte:
                    return byte.Parse(serializedVar);
                case TypeCode.Char:
                    return Char.Parse(serializedVar);
                case TypeCode.DateTime:
                    return DateTime.Parse(serializedVar);
                case TypeCode.Double:
                    return Double.Parse(serializedVar);
                case TypeCode.Int16:
                    return type.IsEnum ? Enum.Parse(type, serializedVar) : Int16.Parse(serializedVar);
                case TypeCode.Int64:
                    return type.IsEnum ? Enum.Parse(type, serializedVar) : Int64.Parse(serializedVar);
                case TypeCode.Single:
                    return Single.Parse(serializedVar);
                case TypeCode.UInt16:
                    return UInt16.Parse(serializedVar);
                case TypeCode.UInt32:
                    return UInt32.Parse(serializedVar);
                case TypeCode.UInt64:
                    return UInt64.Parse(serializedVar);

                default:
                    try
                    {
                        return JsonUtility.FromJson(serializedVar, type);
                    }
                    catch 
                    {
                        Logger.LogInternal($"Unable to deserialize {nameof(SaveVar)} of type {type.Name} with value {serializedVar}.");
                        return string.Empty;
                    }
            }
        }
    }
}