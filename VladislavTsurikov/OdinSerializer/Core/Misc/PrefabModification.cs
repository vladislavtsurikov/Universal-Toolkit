﻿//-----------------------------------------------------------------------
// <copyright file="PrefabModification.cs" company="Sirenix IVS">
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OdinSerializer.Utilities;
using Object = UnityEngine.Object;

namespace OdinSerializer
{
    /// <summary>
    ///     An Odin-serialized prefab modification, containing all the information necessary to apply the modification.
    /// </summary>
    public sealed class PrefabModification
    {
        /// <summary>
        ///     The dictionary keys to add.
        /// </summary>
        public object[] DictionaryKeysAdded;

        /// <summary>
        ///     The dictionary keys to remove.
        /// </summary>
        public object[] DictionaryKeysRemoved;

        /// <summary>
        ///     The type of modification to be made.
        /// </summary>
        public PrefabModificationType ModificationType;

        /// <summary>
        ///     The modified value to set.
        /// </summary>
        public object ModifiedValue;

        /// <summary>
        ///     The new list length to set.
        /// </summary>
        public int NewLength;

        /// <summary>
        ///     The deep reflection path at which to make the modification.
        /// </summary>
        public string Path;

        /// <summary>
        ///     A list of all deep reflection paths in the target object where the value referenced by this modification was also
        ///     located.
        /// </summary>
        public List<string> ReferencePaths;

        /// <summary>
        ///     Applies the modification to the given Object.
        /// </summary>
        public void Apply(Object unityObject)
        {
            if (ModificationType == PrefabModificationType.Value)
            {
                ApplyValue(unityObject);
            }
            else if (ModificationType == PrefabModificationType.ListLength)
            {
                ApplyListLength(unityObject);
            }
            else if (ModificationType == PrefabModificationType.Dictionary)
            {
                ApplyDictionaryModifications(unityObject);
            }
            else
            {
                throw new NotImplementedException(ModificationType.ToString());
            }
        }

        private void ApplyValue(Object unityObject)
        {
            Type valueType = null;

            if (!ReferenceEquals(ModifiedValue, null))
            {
                valueType = ModifiedValue.GetType();
            }

            if (valueType != null && ReferencePaths != null && ReferencePaths.Count > 0)
            {
                for (var i = 0; i < ReferencePaths.Count; i++)
                {
                    var path = ReferencePaths[i];

                    try
                    {
                        var refValue = GetInstanceFromPath(path, unityObject);

                        if (!ReferenceEquals(refValue, null) && refValue.GetType() == valueType)
                        {
                            ModifiedValue = refValue;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            SetInstanceToPath(Path, unityObject, ModifiedValue);
        }

        private void ApplyListLength(Object unityObject)
        {
            var listObj = GetInstanceFromPath(Path, unityObject);

            if (listObj == null)
                // The list has been deleted on the prefab;
                // that supersedes our length change.
            {
                return;
            }

            Type listType = listObj.GetType();

            if (listType.IsArray)
            {
                var array = (Array)listObj;

                if (NewLength == array.Length)
                    // If this happens, for some weird reason, then we can actually just not do anything
                {
                    return;
                }

                // We actually need to replace all references to this array in the entire object graph!
                // Ridiculous, we know - but there's no choice...

                // Let's create a new, modified array
                var newArray = Array.CreateInstance(listType.GetElementType(), NewLength);

                if (NewLength > array.Length)
                {
                    Array.Copy(array, 0, newArray, 0, array.Length);
                    ReplaceAllReferencesInGraph(unityObject, array, newArray);
                }
                else
                {
                    Array.Copy(array, 0, newArray, 0, newArray.Length);
                    ReplaceAllReferencesInGraph(unityObject, array, newArray);
                }
            }
            else if (typeof(IList).IsAssignableFrom(listType))
            {
                var list = (IList)listObj;
                Type listElementType = listType.ImplementsOpenGenericInterface(typeof(IList<>))
                    ? listType.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0]
                    : null;
                var elementIsValueType = listElementType != null ? listElementType.IsValueType : false;

                var count = 0;

                while (list.Count < NewLength)
                {
                    if (elementIsValueType)
                    {
                        list.Add(Activator.CreateInstance(listElementType));
                    }
                    else
                    {
                        list.Add(null);
                    }

                    count++;
                }

                while (list.Count > NewLength)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
            else if (listType.ImplementsOpenGenericInterface(typeof(IList<>)))
            {
                Type elementType = listType.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0];
                Type collectionType = typeof(ICollection<>).MakeGenericType(elementType);
                var elementIsValueType = elementType.IsValueType;

                PropertyInfo countProp = collectionType.GetProperty("Count");

                var count = (int)countProp.GetValue(listObj, null);

                if (count < NewLength)
                {
                    var add = NewLength - count;

                    MethodInfo addMethod = collectionType.GetMethod("Add");

                    for (var i = 0; i < add; i++)
                    {
                        if (elementIsValueType)
                        {
                            addMethod.Invoke(listObj, new[] { Activator.CreateInstance(elementType) });
                        }
                        else
                        {
                            addMethod.Invoke(listObj, new object[] { null });
                        }

                        count++;
                    }
                }
                else if (count > NewLength)
                {
                    var remove = count - NewLength;

                    Type listInterfaceType = typeof(IList<>).MakeGenericType(elementType);
                    MethodInfo removeAtMethod = listInterfaceType.GetMethod("RemoveAt");

                    for (var i = 0; i < remove; i++)
                    {
                        removeAtMethod.Invoke(listObj, new object[] { count - (remove + 1) });
                    }
                }
            }
        }

        private void ApplyDictionaryModifications(Object unityObject)
        {
            var dictionaryObj = GetInstanceFromPath(Path, unityObject);

            if (dictionaryObj == null)
                // The dictionary has been deleted on the prefab;
                // that supersedes our dictionary modifications.
            {
                return;
            }

            Type type = dictionaryObj.GetType();

            if (!type.ImplementsOpenGenericInterface(typeof(IDictionary<,>)))
                // A value change has changed the target modified value to
                // not be a dictionary - that also supersedes this modification.
            {
                return;
            }

            Type[] typeArgs = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IDictionary<,>));

            Type iType = typeof(IDictionary<,>).MakeGenericType(typeArgs);

            //
            // First, remove keys
            //

            if (DictionaryKeysRemoved != null && DictionaryKeysRemoved.Length > 0)
            {
                MethodInfo method = iType.GetMethod("Remove", new[] { typeArgs[0] });
                var parameters = new object[1];

                for (var i = 0; i < DictionaryKeysRemoved.Length; i++)
                {
                    parameters[0] = DictionaryKeysRemoved[i];

                    // Ensure the key value is safe to add
                    if (ReferenceEquals(parameters[0], null) || !typeArgs[0].IsAssignableFrom(parameters[0].GetType()))
                    {
                        continue;
                    }

                    method.Invoke(dictionaryObj, parameters);
                }
            }

            //
            // Then, add keys
            //

            if (DictionaryKeysAdded != null && DictionaryKeysAdded.Length > 0)
            {
                MethodInfo method = iType.GetMethod("set_Item", typeArgs);
                var parameters = new object[2];

                // Get default value to set key to
                parameters[1] = typeArgs[1].IsValueType ? Activator.CreateInstance(typeArgs[1]) : null;

                for (var i = 0; i < DictionaryKeysAdded.Length; i++)
                {
                    parameters[0] = DictionaryKeysAdded[i];

                    // Ensure the key value is safe to add
                    if (ReferenceEquals(parameters[0], null) || !typeArgs[0].IsAssignableFrom(parameters[0].GetType()))
                    {
                        continue;
                    }

                    method.Invoke(dictionaryObj, parameters);
                }
            }
        }

        private static void ReplaceAllReferencesInGraph(object graph, object oldReference, object newReference,
            HashSet<object> processedReferences = null)
        {
            if (processedReferences == null)
            {
                processedReferences = new HashSet<object>(ReferenceEqualityComparer<object>.Default);
            }

            processedReferences.Add(graph);

            if (graph.GetType().IsArray)
            {
                var array = (Array)graph;

                for (var i = 0; i < array.Length; i++)
                {
                    var value = array.GetValue(i);

                    if (ReferenceEquals(value, null))
                    {
                        continue;
                    }

                    if (ReferenceEquals(value, oldReference))
                    {
                        array.SetValue(newReference, i);
                        value = newReference;
                    }

                    if (!processedReferences.Contains(value))
                    {
                        ReplaceAllReferencesInGraph(value, oldReference, newReference, processedReferences);
                    }
                }
            }
            else
            {
                MemberInfo[] members =
                    FormatterUtilities.GetSerializableMembers(graph.GetType(), SerializationPolicies.Everything);

                for (var i = 0; i < members.Length; i++)
                {
                    var field = (FieldInfo)members[i];

                    if (field.FieldType.IsPrimitive || field.FieldType == typeof(SerializationData) ||
                        field.FieldType == typeof(string))
                    {
                        continue;
                    }

                    var value = field.GetValue(graph);

                    if (ReferenceEquals(value, null))
                    {
                        continue;
                    }

                    Type valueType = value.GetType();

                    if (valueType.IsPrimitive || valueType == typeof(SerializationData) || valueType == typeof(string))
                    {
                        continue;
                    }

                    if (ReferenceEquals(value, oldReference))
                    {
                        field.SetValue(graph, newReference);
                        value = newReference;
                    }

                    if (!processedReferences.Contains(value))
                    {
                        ReplaceAllReferencesInGraph(value, oldReference, newReference, processedReferences);
                    }
                }
            }
        }

        private static object GetInstanceFromPath(string path, object instance)
        {
            var steps = path.Split('.');

            var currentInstance = instance;

            for (var i = 0; i < steps.Length; i++)
            {
                currentInstance = GetInstanceOfStep(steps[i], currentInstance);

                if (ReferenceEquals(currentInstance, null))
                    //Debug.LogWarning("Failed to resolve modification path '" + path + "' at step '" + steps[i] + "'.");
                {
                    return null;
                }
            }

            return currentInstance;
        }

        private static object GetInstanceOfStep(string step, object instance)
        {
            Type type = instance.GetType();

            if (step.StartsWith("[", StringComparison.InvariantCulture) &&
                step.EndsWith("]", StringComparison.InvariantCulture))
            {
                int index;
                var indexStr = step.Substring(1, step.Length - 2);

                if (!int.TryParse(indexStr, out index))
                {
                    throw new ArgumentException("Couldn't parse an index from the path step '" + step + "'.");
                }

                // We need to check the current type to see if we can treat it as a list

                if (type.IsArray)
                {
                    var array = (Array)instance;

                    if (index < 0 || index >= array.Length)
                    {
                        return null;
                    }

                    return array.GetValue(index);
                }

                if (typeof(IList).IsAssignableFrom(type))
                {
                    var list = (IList)instance;

                    if (index < 0 || index >= list.Count)
                    {
                        return null;
                    }

                    return list[index];
                }

                if (type.ImplementsOpenGenericInterface(typeof(IList<>)))
                {
                    Type elementType = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0];
                    Type listType = typeof(IList<>).MakeGenericType(elementType);
                    MethodInfo getItemMethod = listType.GetMethod("get_Item",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    try
                    {
                        return getItemMethod.Invoke(instance, new object[] { index });
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
            else if (step.StartsWith("{", StringComparison.InvariantCultureIgnoreCase) &&
                     step.EndsWith("}", StringComparison.InvariantCultureIgnoreCase))
            {
                if (type.ImplementsOpenGenericInterface(typeof(IDictionary<,>)))
                {
                    Type[] dictArgs = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IDictionary<,>));

                    var key = DictionaryKeyUtility.GetDictionaryKeyValue(step, dictArgs[0]);

                    Type dictType = typeof(IDictionary<,>).MakeGenericType(dictArgs);
                    MethodInfo getItemMethod = dictType.GetMethod("get_Item",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    try
                    {
                        return getItemMethod.Invoke(instance, new[] { key });
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
            else
            {
                string privateTypeName = null;
                var plusIndex = step.IndexOf('+');

                if (plusIndex >= 0)
                {
                    privateTypeName = step.Substring(0, plusIndex);
                    step = step.Substring(plusIndex + 1);
                }

                IEnumerable<MemberInfo> possibleMembers =
                    type.GetAllMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(n => n is FieldInfo || n is PropertyInfo);

                foreach (MemberInfo member in possibleMembers)
                {
                    if (member.Name == step)
                    {
                        if (privateTypeName != null && member.DeclaringType.Name != privateTypeName)
                        {
                            continue;
                        }

                        return member.GetMemberValue(instance);
                    }
                }
            }

            return null;
        }

        private static void SetInstanceToPath(string path, object instance, object value)
        {
            bool setParentInstance;
            var steps = path.Split('.');
            SetInstanceToPath(path, steps, 0, instance, value, out setParentInstance);
        }

        private static void SetInstanceToPath(string path, string[] steps, int index, object instance, object value,
            out bool setParentInstance)
        {
            setParentInstance = false;

            if (index < steps.Length - 1)
            {
                var currentInstance = GetInstanceOfStep(steps[index], instance);

                if (ReferenceEquals(currentInstance, null))
                    //Debug.LogWarning("Failed to resolve prefab modification path '" + path + "' at step '" + steps[index] + "'.");
                {
                    return;
                }

                SetInstanceToPath(path, steps, index + 1, currentInstance, value, out setParentInstance);

                if (setParentInstance)
                    // We need to set the current instance to the parent instance member,
                    // because the current instance is a value type, and thus it may have
                    // been boxed. If we don't do this, the value set might be lost.
                {
                    TrySetInstanceOfStep(steps[index], instance, currentInstance, out setParentInstance);
                }
            }
            else
            {
                TrySetInstanceOfStep(steps[index], instance, value, out setParentInstance);
            }
        }

        private static bool TrySetInstanceOfStep(string step, object instance, object value, out bool setParentInstance)
        {
            setParentInstance = false;

            try
            {
                Type type = instance.GetType();

                if (step.StartsWith("[", StringComparison.InvariantCulture) &&
                    step.EndsWith("]", StringComparison.InvariantCulture))
                {
                    int index;
                    var indexStr = step.Substring(1, step.Length - 2);

                    if (!int.TryParse(indexStr, out index))
                    {
                        throw new ArgumentException("Couldn't parse an index from the path step '" + step + "'.");
                    }

                    // We need to check the current type to see if we can treat it as a list

                    if (type.IsArray)
                    {
                        var array = (Array)instance;

                        if (index < 0 || index >= array.Length)
                        {
                            return false;
                        }

                        array.SetValue(value, index);
                        return true;
                    }

                    if (typeof(IList).IsAssignableFrom(type))
                    {
                        var list = (IList)instance;

                        if (index < 0 || index >= list.Count)
                        {
                            return false;
                        }

                        list[index] = value;
                        return true;
                    }

                    if (type.ImplementsOpenGenericInterface(typeof(IList<>)))
                    {
                        Type elementType = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0];
                        Type listType = typeof(IList<>).MakeGenericType(elementType);
                        MethodInfo setItemMethod = listType.GetMethod("set_Item",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        setItemMethod.Invoke(instance, new[] { index, value });
                        return true;
                    }
                }
                else if (step.StartsWith("{", StringComparison.InvariantCulture) &&
                         step.EndsWith("}", StringComparison.InvariantCulture))
                {
                    if (type.ImplementsOpenGenericInterface(typeof(IDictionary<,>)))
                    {
                        Type[] dictArgs = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IDictionary<,>));

                        var key = DictionaryKeyUtility.GetDictionaryKeyValue(step, dictArgs[0]);

                        Type dictType = typeof(IDictionary<,>).MakeGenericType(dictArgs);

                        MethodInfo containsKeyMethod = dictType.GetMethod("ContainsKey",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        MethodInfo setItemMethod = dictType.GetMethod("set_Item",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        var containsKey = (bool)containsKeyMethod.Invoke(instance, new[] { key });

                        if (!containsKey)
                            // We are *not* allowed to add new keys during this step
                        {
                            return false;
                        }

                        setItemMethod.Invoke(instance, new[] { key, value });
                    }
                }
                else
                {
                    string privateTypeName = null;
                    var plusIndex = step.IndexOf('+');

                    if (plusIndex >= 0)
                    {
                        privateTypeName = step.Substring(0, plusIndex);
                        step = step.Substring(plusIndex + 1);
                    }

                    IEnumerable<MemberInfo> possibleMembers =
                        type.GetAllMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Where(n => n is FieldInfo || n is PropertyInfo);

                    foreach (MemberInfo member in possibleMembers)
                    {
                        if (member.Name == step)
                        {
                            if (privateTypeName != null && member.DeclaringType.Name != privateTypeName)
                            {
                                continue;
                            }

                            member.SetMemberValue(instance, value);

                            if (instance.GetType().IsValueType)
                            {
                                setParentInstance = true;
                            }

                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
