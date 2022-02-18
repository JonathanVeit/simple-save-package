using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleSave.Extensions;
using SimpleSave.Models.Serializables;
using Object = UnityEngine.Object;

namespace SimpleSave.Services
{
    /// <inheritdoc cref="IComponentSerializer"/>
    internal class JsonComponentSerializer : BaseService, IComponentSerializer
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings
            = new JsonSerializerSettings()
            {
                ContractResolver = new IgnoreUnityObjectsResolver(),
            };

        #region Serialize

        /// <inheritdoc/>
        public string Serialize(Component component)
        {
            switch (component)
            {
                case MonoBehaviour asMonoBehaviour:
                    return SerializeAsMonoBehaviour(asMonoBehaviour);

                #region Engine Types

                case Transform asTransform:
                    return SerializeAsTransform(asTransform);

                case Rigidbody asRigidbody:
                    return SerializeAsRigidbody(asRigidbody);

                case Rigidbody2D asRigidbody2D:
                    return SerializeAsRigidbody2D(asRigidbody2D);

                case Animator asAnimator:
                    return SerializeAsAnimator(asAnimator);

                #endregion

                default:
                    Logger.LogInternal(
                        $"Components of type \"{component.GetType().Name}\" cannot be serialized. Please check out documentation to find a list of all supported component types " +
                        $"or write your own {nameof(IComponentSerializer)} implementation to support it.");
                    return string.Empty;
            }
        }

        private static string SerializeAsMonoBehaviour(MonoBehaviour monoBehaviour)
        {
            return JsonUtility.ToJson(monoBehaviour);
        }

        private static string SerializeAsTransform(Transform transform)
        {
            var sTransform = new SerializableTransform
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale,
            };

            return JsonUtility.ToJson(sTransform);
        }

        private static string SerializeAsRigidbody(Rigidbody rigidbody)
        {
            var sTransform = new SerializableRigidbody
            {
                AngularDrag = rigidbody.angularDrag,
                AngularVelocity = rigidbody.angularVelocity,
                CenterOfMass = rigidbody.centerOfMass,
                CollisionDetectionMode = rigidbody.collisionDetectionMode,
                Constraints = rigidbody.constraints,
                DetectCollisions = rigidbody.detectCollisions,
                Drag = rigidbody.drag,
                FreezeRotation = rigidbody.freezeRotation,
                InertiaTensor = rigidbody.inertiaTensor,
                InertiaTensorRotation = rigidbody.inertiaTensorRotation,
                Interpolation = rigidbody.interpolation,
                IsKinematic = rigidbody.isKinematic,
                Mass = rigidbody.mass,
                MaxAngularVelocity = rigidbody.maxAngularVelocity,
                MaxDepenetrationVelocity = rigidbody.maxDepenetrationVelocity,
                SleepThreshold = rigidbody.sleepThreshold,
                SolverIterations = rigidbody.solverIterations,
                SolverVelocityIterations = rigidbody.solverVelocityIterations,
                UseGravity = rigidbody.useGravity,
                Velocity = rigidbody.velocity,
            };

            return JsonUtility.ToJson(sTransform);
        }

        private static string SerializeAsRigidbody2D(Rigidbody2D rigidbody2D)
        {
            var sTransform = new SerializableRigidbody2D
            {
                AngularDrag = rigidbody2D.angularDrag,
                AngularVelocity = rigidbody2D.angularVelocity,
                BodyType = rigidbody2D.bodyType,
                CenterOfMass = rigidbody2D.centerOfMass,
                CollisionDetectionMode = rigidbody2D.collisionDetectionMode,
                Constraints = rigidbody2D.constraints,
                Drag = rigidbody2D.drag,
                FreezeRotation = rigidbody2D.freezeRotation,
                GravityScale = rigidbody2D.gravityScale,
                Inertia = rigidbody2D.inertia,
                Interpolation = rigidbody2D.interpolation,
                IsKinematic = rigidbody2D.isKinematic,
                Mass = rigidbody2D.mass,
                Simulated = rigidbody2D.simulated,
                SleepMode = rigidbody2D.sleepMode,
                UseAutoMass = rigidbody2D.useAutoMass,
                UseFullKinematicContacts = rigidbody2D.useFullKinematicContacts,
                Velocity = rigidbody2D.velocity,
            };

            return JsonUtility.ToJson(sTransform);
        }

        private static string SerializeAsAnimator(Animator asAnimator)
        {
            var layerCount = asAnimator.layerCount;
            var serializableAnimatorLayers = new SerializableAnimatorLayer[layerCount];
            
            for (int i = 0; i < layerCount; i++)
            {
                serializableAnimatorLayers[i] = new SerializableAnimatorLayer
                {
                    Index = i,
                    StateHash = asAnimator.GetCurrentAnimatorStateInfo(i).fullPathHash,
                    NormalizedTime = asAnimator.GetCurrentAnimatorStateInfo(i).normalizedTime,
                };
            }

            var parameterCount = asAnimator.parameterCount;
            var serializableAnimatorParameters = new SerializableAnimatorParameter[parameterCount];

            for (int i = 0; i < parameterCount; i++)
            {
                string parameterValue;

                switch (asAnimator.parameters[i].type)
                {
                    case AnimatorControllerParameterType.Float:
                        parameterValue = asAnimator.GetFloat(asAnimator.parameters[i].name).ToString();
                        break;
                    case AnimatorControllerParameterType.Int:
                        parameterValue = asAnimator.GetInteger(asAnimator.parameters[i].name).ToString();
                        break;
                    case AnimatorControllerParameterType.Bool:
                        parameterValue = asAnimator.GetBool(asAnimator.parameters[i].name).ToString();
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        parameterValue = asAnimator.GetBool(asAnimator.parameters[i].name).ToString();
                        break;
                    default:
                        Logger.LogInternal(
                            $"{nameof(JsonComponentSerializer)} has no implementation to serialize AnimatorControllerType {asAnimator.parameters[i].type}.");
                        continue;
                }

                serializableAnimatorParameters[i] = new SerializableAnimatorParameter
                {
                    Name = asAnimator.parameters[i].name,
                    Type = asAnimator.parameters[i].type,
                    Value = parameterValue,
                };
            }

            var serializableAnimator = new SerializableAnimator
            {
                Layers = serializableAnimatorLayers,
                Parameters = serializableAnimatorParameters,
            };
            return JsonUtility.ToJson(serializableAnimator);
        }

        #endregion

        #region Populate

        /// <inheritdoc/>
        public void PopulateTo(string serializedComponent, Component targetComponent)
        {
            switch (targetComponent)
            {
                case MonoBehaviour asMonoBehaviour:
                    PopulateToMonoBehaviour(serializedComponent, asMonoBehaviour);
                    break;

                #region Engine Types

                case Transform asTransform:
                    PopulateToTransform(serializedComponent, asTransform);
                    break;

                case Rigidbody asRigidbody:
                    PopulateToRigidbody(serializedComponent, asRigidbody);
                    break;

                case Rigidbody2D asRigidbody2D:
                    PopulateToRigidbody2D(serializedComponent, asRigidbody2D);
                    break;

                case Animator asAnimator:
                    PopulateToAnimator(serializedComponent, asAnimator);
                    break;

                #endregion

                default:
                    Logger.LogInternal(
                        $"Components of type \"{targetComponent.GetType().Name}\" cannot be deserialized. Please check out documentation to find a list of all supported component types " +
                        $"or write your own {nameof(IComponentSerializer)} implementation to support it.");
                    break;
            }
        }

        private static void PopulateToMonoBehaviour(string serialized, MonoBehaviour monoBehaviour)
        {
            JsonConvert.PopulateObject(serialized, monoBehaviour, JsonSerializerSettings);
        }

        private static void PopulateToTransform(string serialized, Transform transform)
        {
            var sTransform = JsonUtility.FromJson<SerializableTransform>(serialized);

            transform.position = sTransform.Position;
            transform.rotation = sTransform.Rotation;
            transform.localScale = sTransform.Scale;
        }

        private static void PopulateToRigidbody(string serialized, Rigidbody rigidbody)
        {
            var sRigidBody = JsonUtility.FromJson<SerializableRigidbody>(serialized);

            rigidbody.angularDrag = sRigidBody.AngularDrag;
            rigidbody.angularVelocity = sRigidBody.AngularVelocity;
            rigidbody.centerOfMass = sRigidBody.CenterOfMass;
            rigidbody.collisionDetectionMode = sRigidBody.CollisionDetectionMode;
            rigidbody.constraints = sRigidBody.Constraints;
            rigidbody.detectCollisions = sRigidBody.DetectCollisions;
            rigidbody.drag = sRigidBody.Drag;
            rigidbody.freezeRotation = sRigidBody.FreezeRotation;
            rigidbody.inertiaTensor = sRigidBody.InertiaTensor;
            rigidbody.inertiaTensorRotation = sRigidBody.InertiaTensorRotation;
            rigidbody.interpolation = sRigidBody.Interpolation;
            rigidbody.isKinematic = sRigidBody.IsKinematic;
            rigidbody.mass = sRigidBody.Mass;
            rigidbody.maxAngularVelocity = sRigidBody.MaxAngularVelocity;
            rigidbody.maxDepenetrationVelocity = sRigidBody.MaxDepenetrationVelocity;
            rigidbody.sleepThreshold = sRigidBody.SleepThreshold;
            rigidbody.solverIterations = sRigidBody.SolverIterations;
            rigidbody.solverVelocityIterations = sRigidBody.SolverVelocityIterations;
            rigidbody.useGravity = sRigidBody.UseGravity;
            rigidbody.velocity = sRigidBody.Velocity;
        }

        private static void PopulateToRigidbody2D(string serialized, Rigidbody2D rigidbody2D)
        {
            var sRigidBody2d = JsonUtility.FromJson<SerializableRigidbody2D>(serialized);

            rigidbody2D.angularDrag = sRigidBody2d.AngularDrag;
            rigidbody2D.angularVelocity = sRigidBody2d.AngularVelocity;
            rigidbody2D.bodyType = sRigidBody2d.BodyType;
            rigidbody2D.centerOfMass = sRigidBody2d.CenterOfMass;
            rigidbody2D.collisionDetectionMode = sRigidBody2d.CollisionDetectionMode;
            rigidbody2D.constraints = sRigidBody2d.Constraints;
            rigidbody2D.drag = sRigidBody2d.Drag;
            rigidbody2D.freezeRotation = sRigidBody2d.FreezeRotation;
            rigidbody2D.gravityScale = sRigidBody2d.GravityScale;
            rigidbody2D.inertia = sRigidBody2d.Inertia;
            rigidbody2D.interpolation = sRigidBody2d.Interpolation;
            rigidbody2D.isKinematic = sRigidBody2d.IsKinematic;
            rigidbody2D.mass = sRigidBody2d.Mass;
            rigidbody2D.simulated = sRigidBody2d.Simulated;
            rigidbody2D.sleepMode = sRigidBody2d.SleepMode;
            rigidbody2D.useAutoMass = sRigidBody2d.UseAutoMass;
            rigidbody2D.useFullKinematicContacts = sRigidBody2d.UseFullKinematicContacts;
            rigidbody2D.velocity = sRigidBody2d.Velocity;
        }

        private static void PopulateToAnimator(string serializedComponent, Animator animator)
        {
            var serializableAnimator = JsonUtility.FromJson<SerializableAnimator>(serializedComponent);

            for (int i = 0; i < serializableAnimator.Layers.Length; i++)
            {
                var serializableLayer = serializableAnimator.Layers[i];

                animator.Play(serializableLayer.StateHash, serializableLayer.Index, serializableLayer.NormalizedTime);
            }

            for (int i = 0; i < serializableAnimator.Parameters.Length; i++)
            {
                var serializableParameter = serializableAnimator.Parameters[i];

                switch (serializableParameter.Type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(serializableParameter.Name, float.Parse(serializableParameter.Value));
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(serializableParameter.Name, int.Parse(serializableParameter.Value));
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(serializableParameter.Name, bool.Parse(serializableParameter.Value));
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        if (bool.Parse(serializableParameter.Value))
                        {
                            animator.SetTrigger(serializableParameter.Name);
                        }
                        break;
                    default:
                        Logger.LogInternal(
                            $"{nameof(JsonComponentSerializer)} has no implementation to load AnimatorControllerType of type {serializableParameter.Type}.");
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// All fields or properties with the <see cref="Object"/> type will be ignored during the (de)serialization.
        /// </summary>
        private class IgnoreUnityObjectsResolver : DefaultContractResolver
        {
            private static readonly Type ObjectType = typeof(Object);
            private static readonly Type MonoBehaviourType = typeof(MonoBehaviour);
            private static readonly Type GenericListType = typeof(List<>);
            private static readonly Type LayerMaskType = typeof(LayerMask);

            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                var result = new List<MemberInfo>();
                GetFieldsRecursively(objectType, result);

                return result;
            }

            private void GetFieldsRecursively(Type type, List<MemberInfo> result)
            {
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance |
                                            BindingFlags.Public | BindingFlags.GetField | BindingFlags.DeclaredOnly);

                result.AddRange(fields);

                if (type.BaseType == null ||
                    type.BaseType == MonoBehaviourType ||
                    type.BaseType == ObjectType)
                {
                    return;
                }

                GetFieldsRecursively(type.BaseType, result);
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, MemberSerialization.Fields);

                if (IsIgnored(property.PropertyType))
                {
                    property.ShouldSerialize = i => false;
                    property.Ignored = true;
                }

                return property;
            }

            private bool IsIgnored(Type type)
            {
                return type == ObjectType ||
                       type.IsSubclassOf(ObjectType) ||
                       type == LayerMaskType ||
                       IsArray(type) ||
                       IsList(type);
            }

            private bool IsArray(Type type)
            {
                if (type.IsArray)
                {
                    var elementType = type.GetElementType();

                    return elementType == ObjectType ||
                           elementType.IsSubclassOf(ObjectType) ||
                           elementType == LayerMaskType;
                }

                return false;
            }

            private bool IsList(Type type)
            {
                if (type.IsArray ||
                    !type.IsGenericType ||
                    type.GetGenericTypeDefinition() != GenericListType)
                {
                    return false;
                }

                var genericArgument = type.GetGenericArguments()[0];
                return genericArgument == ObjectType ||
                       genericArgument.IsSubclassOf(ObjectType) ||
                       genericArgument == LayerMaskType;
            }
        }
    }
}