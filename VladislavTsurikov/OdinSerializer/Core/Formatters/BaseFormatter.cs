﻿//-----------------------------------------------------------------------
// <copyright file="BaseFormatter.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using OdinSerializer.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;
#if (UNITY_EDITOR || UNITY_STANDALONE) && !ENABLE_IL2CPP
#define CAN_EMIT
#endif

namespace OdinSerializer
{
    /// <summary>
    ///     Provides common functionality for serializing and deserializing values of type <see cref="T" />, and provides
    ///     automatic support for the following common serialization conventions:
    ///     <para />
    ///     <see cref="IObjectReference" />, <see cref="ISerializationCallbackReceiver" />,
    ///     <see cref="OnSerializingAttribute" />, <see cref="OnSerializedAttribute" />,
    ///     <see cref="OnDeserializingAttribute" /> and <see cref="OnDeserializedAttribute" />.
    /// </summary>
    /// <typeparam name="T">The type which can be serialized and deserialized by the formatter.</typeparam>
    /// <seealso cref="IFormatter{T}" />
    public abstract class BaseFormatter<T> : IFormatter<T>
    {
        /// <summary>
        ///     The on serializing callbacks for type <see cref="T" />.
        /// </summary>
        protected static readonly SerializationCallback[] OnSerializingCallbacks;

        /// <summary>
        ///     The on serialized callbacks for type <see cref="T" />.
        /// </summary>
        protected static readonly SerializationCallback[] OnSerializedCallbacks;

        /// <summary>
        ///     The on deserializing callbacks for type <see cref="T" />.
        /// </summary>
        protected static readonly SerializationCallback[] OnDeserializingCallbacks;

        /// <summary>
        ///     The on deserialized callbacks for type <see cref="T" />.
        /// </summary>
        protected static readonly SerializationCallback[] OnDeserializedCallbacks;

        /// <summary>
        ///     Whether the serialized value is a value type.
        /// </summary>
        protected static readonly bool IsValueType = typeof(T).IsValueType;

        protected static readonly bool ImplementsISerializationCallbackReceiver =
            typeof(T).ImplementsOrInherits(typeof(ISerializationCallbackReceiver));

        protected static readonly bool ImplementsIDeserializationCallback =
            typeof(T).ImplementsOrInherits(typeof(IDeserializationCallback));

        protected static readonly bool ImplementsIObjectReference =
            typeof(T).ImplementsOrInherits(typeof(IObjectReference));

        static BaseFormatter()
        {
            if (typeof(T).ImplementsOrInherits(typeof(Object)))
            {
                DefaultLoggers.DefaultLogger.LogWarning(
                    "A formatter has been created for the UnityEngine.Object type " + typeof(T).Name +
                    " - this is *strongly* discouraged. Unity should be allowed to handle serialization and deserialization of its own weird objects. Remember to serialize with a UnityReferenceResolver as the external index reference resolver in the serialization context.\n\n Stacktrace: " +
                    new StackTrace());
            }

            MethodInfo[] methods =
                typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var callbacks = new List<SerializationCallback>();

            OnSerializingCallbacks = GetCallbacks(methods, typeof(OnSerializingAttribute), ref callbacks);
            OnSerializedCallbacks = GetCallbacks(methods, typeof(OnSerializedAttribute), ref callbacks);
            OnDeserializingCallbacks = GetCallbacks(methods, typeof(OnDeserializingAttribute), ref callbacks);
            OnDeserializedCallbacks = GetCallbacks(methods, typeof(OnDeserializedAttribute), ref callbacks);
        }

        /// <summary>
        ///     Gets the type that the formatter can serialize.
        /// </summary>
        /// <value>
        ///     The type that the formatter can serialize.
        /// </value>
        public Type SerializedType => typeof(T);

        /// <summary>
        ///     Serializes a value using a specified <see cref="IDataWriter" />.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        void IFormatter.Serialize(object value, IDataWriter writer) => Serialize((T)value, writer);

        /// <summary>
        ///     Deserializes a value using a specified <see cref="IDataReader" />.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>
        ///     The deserialized value.
        /// </returns>
        object IFormatter.Deserialize(IDataReader reader) => Deserialize(reader);

        /// <summary>
        ///     Deserializes a value of type <see cref="T" /> using a specified <see cref="IDataReader" />.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>
        ///     The deserialized value.
        /// </returns>
        public T Deserialize(IDataReader reader)
        {
            DeserializationContext context = reader.Context;
            T value = GetUninitializedObject();

            // We allow the above method to return null (for reference types) because of special cases like arrays,
            //  where the size of the array cannot be known yet, and thus we cannot create an object instance at this time.
            //
            // Therefore, those who override GetUninitializedObject and return null must call RegisterReferenceID and InvokeOnDeserializingCallbacks manually.
            if (IsValueType)
            {
                InvokeOnDeserializingCallbacks(ref value, context);
            }
            else
            {
                if (ReferenceEquals(value, null) == false)
                {
                    RegisterReferenceID(value, reader);
                    InvokeOnDeserializingCallbacks(ref value, context);

                    if (ImplementsIObjectReference)
                    {
                        try
                        {
                            value = (T)(value as IObjectReference).GetRealObject(context.StreamingContext);
                            RegisterReferenceID(value, reader);
                        }
                        catch (Exception ex)
                        {
                            context.Config.DebugContext.LogException(ex);
                        }
                    }
                }
            }

            try
            {
                DeserializeImplementation(ref value, reader);
            }
            catch (Exception ex)
            {
                context.Config.DebugContext.LogException(ex);
            }

            // The deserialized value might be null, so check for that
            if (IsValueType || ReferenceEquals(value, null) == false)
            {
                for (var i = 0; i < OnDeserializedCallbacks.Length; i++)
                {
                    try
                    {
                        OnDeserializedCallbacks[i](ref value, context.StreamingContext);
                    }
                    catch (Exception ex)
                    {
                        context.Config.DebugContext.LogException(ex);
                    }
                }

                if (ImplementsIDeserializationCallback)
                {
                    var v = value as IDeserializationCallback;
                    v.OnDeserialization(this);
                    value = (T)v;
                }

                if (ImplementsISerializationCallbackReceiver)
                {
                    try
                    {
                        var v = value as ISerializationCallbackReceiver;
                        v.OnAfterDeserialize();
                        value = (T)v;
                    }
                    catch (Exception ex)
                    {
                        context.Config.DebugContext.LogException(ex);
                    }
                }
            }

            return value;
        }

        /// <summary>
        ///     Serializes a value of type <see cref="T" /> using a specified <see cref="IDataWriter" />.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize(T value, IDataWriter writer)
        {
            SerializationContext context = writer.Context;

            for (var i = 0; i < OnSerializingCallbacks.Length; i++)
            {
                try
                {
                    OnSerializingCallbacks[i](ref value, context.StreamingContext);
                }
                catch (Exception ex)
                {
                    context.Config.DebugContext.LogException(ex);
                }
            }

            if (ImplementsISerializationCallbackReceiver)
            {
                try
                {
                    var v = value as ISerializationCallbackReceiver;
                    v.OnBeforeSerialize();
                    value = (T)v;
                }
                catch (Exception ex)
                {
                    context.Config.DebugContext.LogException(ex);
                }
            }

            try
            {
                SerializeImplementation(ref value, writer);
            }
            catch (Exception ex)
            {
                context.Config.DebugContext.LogException(ex);
            }

            for (var i = 0; i < OnSerializedCallbacks.Length; i++)
            {
                try
                {
                    OnSerializedCallbacks[i](ref value, context.StreamingContext);
                }
                catch (Exception ex)
                {
                    context.Config.DebugContext.LogException(ex);
                }
            }
        }

        private static SerializationCallback[] GetCallbacks(MethodInfo[] methods, Type callbackAttribute,
            ref List<SerializationCallback> list)
        {
            for (var i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];

                if (method.IsDefined(callbackAttribute, true))
                {
                    SerializationCallback callback = CreateCallback(method);

                    if (callback != null)
                    {
                        list.Add(callback);
                    }
                }
            }

            SerializationCallback[] result = list.ToArray();
            list.Clear();
            return result;
        }

        private static SerializationCallback CreateCallback(MethodInfo info)
        {
            ParameterInfo[] parameters = info.GetParameters();
            if (parameters.Length == 0)
            {
#if CAN_EMIT
                var action = EmitUtilities.CreateInstanceRefMethodCaller<T>(info);
                return (ref T value, StreamingContext context) => action(ref value);
#else
                return (ref T value, StreamingContext context) =>
                {
                    object obj = value;
                    info.Invoke(obj, null);
                    value = (T)obj;
                };
#endif
            }

            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(StreamingContext) &&
                parameters[0].ParameterType.IsByRef == false)
            {
#if CAN_EMIT
                var action = EmitUtilities.CreateInstanceRefMethodCaller<T, StreamingContext>(info);
                return (ref T value, StreamingContext context) => action(ref value, context);
#else
                return (ref T value, StreamingContext context) =>
                {
                    object obj = value;
                    info.Invoke(obj, new object[] { context });
                    value = (T)obj;
                };
#endif
            }

            DefaultLoggers.DefaultLogger.LogWarning("The method " + info.GetNiceName() +
                                                    " has an invalid signature and will be ignored by the serialization system.");
            return null;
        }

        /// <summary>
        ///     Get an uninitialized object of type <see cref="T" />. WARNING: If you override this and return null, the object's
        ///     ID will not be automatically registered and its OnDeserializing callbacks will not be automatically called, before
        ///     deserialization begins.
        ///     You will have to call <see cref="BaseFormatter{T}.RegisterReferenceID(T, IDataReader)" /> and
        ///     <see cref="BaseFormatter{T}.InvokeOnDeserializingCallbacks(ref T, DeserializationContext)" /> immediately after
        ///     creating the object yourself during deserialization.
        /// </summary>
        /// <returns>An uninitialized object of type <see cref="T" />.</returns>
        protected virtual T GetUninitializedObject()
        {
            if (IsValueType)
            {
                return default;
            }

            return (T)FormatterServices.GetUninitializedObject(typeof(T));
        }

        /// <summary>
        ///     Registers the given object reference in the deserialization context.
        ///     <para />
        ///     NOTE that this method only does anything if <see cref="T" /> is not a value type.
        /// </summary>
        /// <param name="value">The value to register.</param>
        /// <param name="reader">The reader which is currently being used.</param>
        protected void RegisterReferenceID(T value, IDataReader reader)
        {
            if (!IsValueType)
            {
                var id = reader.CurrentNodeId;

                if (id < 0)
                {
                    reader.Context.Config.DebugContext.LogWarning(
                        "Reference type node is missing id upon deserialization. Some references may be broken. This tends to happen if a value type has changed to a reference type (IE, struct to class) since serialization took place.");
                }
                else
                {
                    reader.Context.RegisterInternalReference(id, value);
                }
            }
        }

        /// <summary>
        ///     Invokes all methods on the object with the [OnDeserializing] attribute.
        ///     <para />
        ///     WARNING: This method will not be called automatically if you override GetUninitializedObject and return null! You
        ///     will have to call it manually after having created the object instance during deserialization.
        /// </summary>
        /// <param name="value">The value to invoke the callbacks on.</param>
        /// <param name="context">The deserialization context.</param>
        [Obsolete(
            "Use the InvokeOnDeserializingCallbacks variant that takes a ref T value instead. This is for struct compatibility reasons.",
            false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void InvokeOnDeserializingCallbacks(T value, DeserializationContext context) =>
            InvokeOnDeserializingCallbacks(ref value, context);

        /// <summary>
        ///     Invokes all methods on the object with the [OnDeserializing] attribute.
        ///     <para />
        ///     WARNING: This method will not be called automatically if you override GetUninitializedObject and return null! You
        ///     will have to call it manually after having created the object instance during deserialization.
        /// </summary>
        /// <param name="value">The value to invoke the callbacks on.</param>
        /// <param name="context">The deserialization context.</param>
        protected void InvokeOnDeserializingCallbacks(ref T value, DeserializationContext context)
        {
            for (var i = 0; i < OnDeserializingCallbacks.Length; i++)
            {
                try
                {
                    OnDeserializingCallbacks[i](ref value, context.StreamingContext);
                }
                catch (Exception ex)
                {
                    context.Config.DebugContext.LogException(ex);
                }
            }
        }

        /// <summary>
        ///     Provides the actual implementation for deserializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">
        ///     The uninitialized value to serialize into. This value will have been created earlier using
        ///     <see cref="BaseFormatter{T}.GetUninitializedObject" />.
        /// </param>
        /// <param name="reader">The reader to deserialize with.</param>
        protected abstract void DeserializeImplementation(ref T value, IDataReader reader);

        /// <summary>
        ///     Provides the actual implementation for serializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="writer">The writer to serialize with.</param>
        protected abstract void SerializeImplementation(ref T value, IDataWriter writer);

        protected delegate void SerializationCallback(ref T value, StreamingContext context);
    }

    /// <summary>
    ///     Provides common functionality for serializing and deserializing weakly typed values of a given type, and provides
    ///     automatic support for the following common serialization conventions:
    ///     <para />
    ///     <see cref="IObjectReference" />, <see cref="ISerializationCallbackReceiver" />,
    ///     <see cref="OnSerializingAttribute" />, <see cref="OnSerializedAttribute" />,
    ///     <see cref="OnDeserializingAttribute" /> and <see cref="OnDeserializedAttribute" />.
    /// </summary>
    /// <seealso cref="IFormatter" />
    public abstract class WeakBaseFormatter : IFormatter
    {
        protected readonly bool ImplementsIDeserializationCallback;
        protected readonly bool ImplementsIObjectReference;

        protected readonly bool ImplementsISerializationCallbackReceiver;
        protected readonly bool IsValueType;
        protected readonly SerializationCallback[] OnDeserializedCallbacks;
        protected readonly SerializationCallback[] OnDeserializingCallbacks;
        protected readonly SerializationCallback[] OnSerializedCallbacks;

        protected readonly SerializationCallback[] OnSerializingCallbacks;
        protected readonly Type SerializedType;

        public WeakBaseFormatter(Type serializedType)
        {
            SerializedType = serializedType;
            ImplementsISerializationCallbackReceiver =
                SerializedType.ImplementsOrInherits(typeof(ISerializationCallbackReceiver));
            ImplementsIDeserializationCallback = SerializedType.ImplementsOrInherits(typeof(IDeserializationCallback));
            ImplementsIObjectReference = SerializedType.ImplementsOrInherits(typeof(IObjectReference));

            if (SerializedType.ImplementsOrInherits(typeof(Object)))
            {
                DefaultLoggers.DefaultLogger.LogWarning(
                    "A formatter has been created for the UnityEngine.Object type " + SerializedType.Name +
                    " - this is *strongly* discouraged. Unity should be allowed to handle serialization and deserialization of its own weird objects. Remember to serialize with a UnityReferenceResolver as the external index reference resolver in the serialization context.\n\n Stacktrace: " +
                    new StackTrace());
            }

            MethodInfo[] methods =
                SerializedType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var callbacks = new List<SerializationCallback>();

            OnSerializingCallbacks = GetCallbacks(methods, typeof(OnSerializingAttribute), ref callbacks);
            OnSerializedCallbacks = GetCallbacks(methods, typeof(OnSerializedAttribute), ref callbacks);
            OnDeserializingCallbacks = GetCallbacks(methods, typeof(OnDeserializingAttribute), ref callbacks);
            OnDeserializedCallbacks = GetCallbacks(methods, typeof(OnDeserializedAttribute), ref callbacks);
        }

        Type IFormatter.SerializedType => SerializedType;

        /// <summary>
        ///     Serializes a value using a specified <see cref="IDataWriter" />.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        public void Serialize(object value, IDataWriter writer)
        {
            SerializationContext context = writer.Context;

            for (var i = 0; i < OnSerializingCallbacks.Length; i++)
            {
                try
                {
                    OnSerializingCallbacks[i](value, context.StreamingContext);
                }
                catch (Exception ex)
                {
                    context.Config.DebugContext.LogException(ex);
                }
            }

            if (ImplementsISerializationCallbackReceiver)
            {
                try
                {
                    var v = value as ISerializationCallbackReceiver;
                    v.OnBeforeSerialize();
                    value = v;
                }
                catch (Exception ex)
                {
                    context.Config.DebugContext.LogException(ex);
                }
            }

            try
            {
                SerializeImplementation(ref value, writer);
            }
            catch (Exception ex)
            {
                context.Config.DebugContext.LogException(ex);
            }

            for (var i = 0; i < OnSerializedCallbacks.Length; i++)
            {
                try
                {
                    OnSerializedCallbacks[i](value, context.StreamingContext);
                }
                catch (Exception ex)
                {
                    context.Config.DebugContext.LogException(ex);
                }
            }
        }

        /// <summary>
        ///     Deserializes a value using a specified <see cref="IDataReader" />.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>
        ///     The deserialized value.
        /// </returns>
        public object Deserialize(IDataReader reader)
        {
            DeserializationContext context = reader.Context;
            var value = GetUninitializedObject();

            // We allow the above method to return null (for reference types) because of special cases like arrays,
            //  where the size of the array cannot be known yet, and thus we cannot create an object instance at this time.
            //
            // Therefore, those who override GetUninitializedObject and return null must call RegisterReferenceID and InvokeOnDeserializingCallbacks manually.
            if (IsValueType)
            {
                if (ReferenceEquals(null, value))
                {
                    value = Activator.CreateInstance(SerializedType);
                }

                InvokeOnDeserializingCallbacks(value, context);
            }
            else
            {
                if (ReferenceEquals(value, null) == false)
                {
                    RegisterReferenceID(value, reader);
                    InvokeOnDeserializingCallbacks(value, context);

                    if (ImplementsIObjectReference)
                    {
                        try
                        {
                            value = (value as IObjectReference).GetRealObject(context.StreamingContext);
                            RegisterReferenceID(value, reader);
                        }
                        catch (Exception ex)
                        {
                            context.Config.DebugContext.LogException(ex);
                        }
                    }
                }
            }

            try
            {
                DeserializeImplementation(ref value, reader);
            }
            catch (Exception ex)
            {
                context.Config.DebugContext.LogException(ex);
            }

            // The deserialized value might be null, so check for that
            if (IsValueType || ReferenceEquals(value, null) == false)
            {
                for (var i = 0; i < OnDeserializedCallbacks.Length; i++)
                {
                    try
                    {
                        OnDeserializedCallbacks[i](value, context.StreamingContext);
                    }
                    catch (Exception ex)
                    {
                        context.Config.DebugContext.LogException(ex);
                    }
                }

                if (ImplementsIDeserializationCallback)
                {
                    var v = value as IDeserializationCallback;
                    v.OnDeserialization(this);
                    value = v;
                }

                if (ImplementsISerializationCallbackReceiver)
                {
                    try
                    {
                        var v = value as ISerializationCallbackReceiver;
                        v.OnAfterDeserialize();
                        value = v;
                    }
                    catch (Exception ex)
                    {
                        context.Config.DebugContext.LogException(ex);
                    }
                }
            }

            return value;
        }

        private static SerializationCallback[] GetCallbacks(MethodInfo[] methods, Type callbackAttribute,
            ref List<SerializationCallback> list)
        {
            for (var i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];

                if (method.IsDefined(callbackAttribute, true))
                {
                    SerializationCallback callback = CreateCallback(method);

                    if (callback != null)
                    {
                        list.Add(callback);
                    }
                }
            }

            SerializationCallback[] result = list.ToArray();
            list.Clear();
            return result;
        }

        private static SerializationCallback CreateCallback(MethodInfo info)
        {
            ParameterInfo[] parameters = info.GetParameters();
            if (parameters.Length == 0)
            {
                return (value, context) => { info.Invoke(value, null); };
            }

            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(StreamingContext) &&
                parameters[0].ParameterType.IsByRef == false)
            {
                return (value, context) => { info.Invoke(value, new object[] { context }); };
            }

            DefaultLoggers.DefaultLogger.LogWarning("The method " + info.GetNiceName() +
                                                    " has an invalid signature and will be ignored by the serialization system.");
            return null;
        }

        /// <summary>
        ///     Registers the given object reference in the deserialization context.
        ///     <para />
        ///     NOTE that this method only does anything if <see cref="T" /> is not a value type.
        /// </summary>
        /// <param name="value">The value to register.</param>
        /// <param name="reader">The reader which is currently being used.</param>
        protected void RegisterReferenceID(object value, IDataReader reader)
        {
            if (!IsValueType)
            {
                var id = reader.CurrentNodeId;

                if (id < 0)
                {
                    reader.Context.Config.DebugContext.LogWarning(
                        "Reference type node is missing id upon deserialization. Some references may be broken. This tends to happen if a value type has changed to a reference type (IE, struct to class) since serialization took place.");
                }
                else
                {
                    reader.Context.RegisterInternalReference(id, value);
                }
            }
        }

        /// <summary>
        ///     Invokes all methods on the object with the [OnDeserializing] attribute.
        ///     <para />
        ///     WARNING: This method will not be called automatically if you override GetUninitializedObject and return null! You
        ///     will have to call it manually after having created the object instance during deserialization.
        /// </summary>
        /// <param name="value">The value to invoke the callbacks on.</param>
        /// <param name="context">The deserialization context.</param>
        protected void InvokeOnDeserializingCallbacks(object value, DeserializationContext context)
        {
            for (var i = 0; i < OnDeserializingCallbacks.Length; i++)
            {
                try
                {
                    OnDeserializingCallbacks[i](value, context.StreamingContext);
                }
                catch (Exception ex)
                {
                    context.Config.DebugContext.LogException(ex);
                }
            }
        }

        protected virtual object GetUninitializedObject() =>
            IsValueType
                ? Activator.CreateInstance(SerializedType)
                : FormatterServices.GetUninitializedObject(SerializedType);

        /// <summary>
        ///     Provides the actual implementation for deserializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">
        ///     The uninitialized value to serialize into. This value will have been created earlier using
        ///     <see cref="BaseFormatter{T}.GetUninitializedObject" />.
        /// </param>
        /// <param name="reader">The reader to deserialize with.</param>
        protected abstract void DeserializeImplementation(ref object value, IDataReader reader);

        /// <summary>
        ///     Provides the actual implementation for serializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="writer">The writer to serialize with.</param>
        protected abstract void SerializeImplementation(ref object value, IDataWriter writer);

        protected delegate void SerializationCallback(object value, StreamingContext context);
    }
}
