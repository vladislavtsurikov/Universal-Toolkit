﻿//-----------------------------------------------------------------------
// <copyright file="ImmutableList.cs" company="Sirenix IVS">
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

namespace OdinSerializer.Utilities
{
    /// <summary>
    ///     Interface for immutable list.
    /// </summary>
    public interface IImmutableList : IList
    {
    }

    /// <summary>
    ///     Interface for generic immutable list.
    /// </summary>
    public interface IImmutableList<T> : IImmutableList, IList<T>
    {
        /// <summary>
        ///     Index accessor.
        /// </summary>
        new T this[int index] { get; }
    }

    /// <summary>
    ///     Immutable list wraps another list, and allows for reading the inner list, without the ability to change it.
    /// </summary>
    [Serializable]
    public sealed class ImmutableList : IImmutableList<object>
    {
        private readonly IList innerList;

        /// <summary>
        ///     Creates an immutable list around another list.
        /// </summary>
        public ImmutableList(IList innerList)
        {
            if (innerList == null)
            {
                throw new ArgumentNullException("innerList");
            }

            this.innerList = innerList;
        }

        /// <summary>
        ///     Number of items in the list.
        /// </summary>
        public int Count => innerList.Count;

        /// <summary>
        ///     Immutable list cannot be changed directly, so it's size is always fixed.
        /// </summary>
        public bool IsFixedSize => true;

        /// <summary>
        ///     Immutable list are always readonly.
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        ///     Returns <c>true</c> if the inner list is synchronized.
        /// </summary>
        public bool IsSynchronized => innerList.IsSynchronized;

        /// <summary>
        ///     Gets the sync root object.
        /// </summary>
        public object SyncRoot => innerList.SyncRoot;

        object IList.this[int index]
        {
            get => innerList[index];

            set => throw new NotSupportedException("Immutable Lists cannot be edited.");
        }

        object IList<object>.this[int index]
        {
            get => innerList[index];

            set => throw new NotSupportedException("Immutable Lists cannot be edited.");
        }

        /// <summary>
        ///     Index accessor.
        /// </summary>
        /// <param name="index">Index.</param>
        public object this[int index] => innerList[index];

        /// <summary>
        ///     Returns <c>true</c> if the item is contained in the list.
        /// </summary>
        /// <param name="value">The item's value.</param>
        public bool Contains(object value) => innerList.Contains(value);

        /// <summary>
        ///     Copy the list to an array,
        /// </summary>
        /// <param name="array">Target array.</param>
        /// <param name="arrayIndex">Index.</param>
        public void CopyTo(object[] array, int arrayIndex) => innerList.CopyTo(array, arrayIndex);

        /// <summary>
        ///     Copy the list to an array,
        /// </summary>
        /// <param name="array">Target array.</param>
        /// <param name="index">Index.</param>
        public void CopyTo(Array array, int index) => innerList.CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            foreach (var obj in innerList)
            {
                yield return obj;
            }
        }

        int IList.Add(object value) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.Clear() => throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.Insert(int index, object value) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.Remove(object value) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.RemoveAt(int index) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        /// <summary>
        ///     Get the index of a value.
        /// </summary>
        /// <param name="value">The item's value.</param>
        public int IndexOf(object value) => innerList.IndexOf(value);

        /// <summary>
        ///     Immutable list cannot be edited.
        /// </summary>
        /// <param name="index">Index.</param>
        void IList<object>.RemoveAt(int index) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        /// <summary>
        ///     Immutable list cannot be edited.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="item">Item.</param>
        void IList<object>.Insert(int index, object item) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");

        /// <summary>
        ///     Immutable list cannot be edited.
        /// </summary>
        /// <param name="item">Item.</param>
        void ICollection<object>.Add(object item) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");

        /// <summary>
        ///     Immutable list cannot be edited.
        /// </summary>
        void ICollection<object>.Clear() => throw new NotSupportedException("Immutable Lists cannot be edited.");

        /// <summary>
        ///     Immutable list cannot be edited.
        /// </summary>
        /// <param name="item">Item.</param>
        bool ICollection<object>.Remove(object item) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");

        /// <summary>
        ///     Gets an enumerator.
        /// </summary>
        public IEnumerator GetEnumerator() => innerList.GetEnumerator();
    }

    /// <summary>
    ///     Not yet documented.
    /// </summary>
    [Serializable]
    public sealed class ImmutableList<T> : IImmutableList<T>
    {
        private readonly IList<T> innerList;

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public ImmutableList(IList<T> innerList)
        {
            if (innerList == null)
            {
                throw new ArgumentNullException("innerList");
            }

            this.innerList = innerList;
        }

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public int Count => innerList.Count;

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => null;
        bool IList.IsFixedSize => true;
        bool IList.IsReadOnly => true;

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public bool IsReadOnly => true;

        object IList.this[int index]
        {
            get => this[index];

            set => throw new NotSupportedException("Immutable Lists cannot be edited.");
        }

        T IList<T>.this[int index]
        {
            get => innerList[index];

            set => throw new NotSupportedException("Immutable Lists cannot be edited.");
        }

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public T this[int index] => innerList[index];

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public bool Contains(T item) => innerList.Contains(item);

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex) => innerList.CopyTo(array, arrayIndex);

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => innerList.GetEnumerator();

        void ICollection.CopyTo(Array array, int index) => innerList.CopyTo((T[])array, index);

        void ICollection<T>.Add(T item) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        void ICollection<T>.Clear() => throw new NotSupportedException("Immutable Lists cannot be edited.");

        bool ICollection<T>.Remove(T item) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        int IList.Add(object value) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.Clear() => throw new NotSupportedException("Immutable Lists cannot be edited.");

        bool IList.Contains(object value) => innerList.Contains((T)value);

        int IList.IndexOf(object value) => innerList.IndexOf((T)value);

        void IList.Insert(int index, object value) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.Remove(object value) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList<T>.Insert(int index, T item) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.RemoveAt(int index) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public int IndexOf(T item) => innerList.IndexOf(item);

        void IList<T>.RemoveAt(int index) => throw new NotSupportedException("Immutable Lists cannot be edited.");
    }

    /// <summary>
    ///     Immutable list wraps another list, and allows for reading the inner list, without the ability to change it.
    /// </summary>
    [Serializable]
    public sealed class ImmutableList<TList, TElement> : IImmutableList<TElement> where TList : IList<TElement>
    {
        private TList innerList;

        /// <summary>
        ///     Creates an immutable list around another list.
        /// </summary>
        public ImmutableList(TList innerList)
        {
            if (innerList == null)
            {
                throw new ArgumentNullException("innerList");
            }

            this.innerList = innerList;
        }

        /// <summary>
        ///     Number of items in the list.
        /// </summary>
        public int Count => innerList.Count;

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => null;
        bool IList.IsFixedSize => true;
        bool IList.IsReadOnly => true;

        /// <summary>
        ///     Immutable list are always readonly.
        /// </summary>
        public bool IsReadOnly => true;

        object IList.this[int index]
        {
            get => this[index];

            set => throw new NotSupportedException("Immutable Lists cannot be edited.");
        }

        TElement IList<TElement>.this[int index]
        {
            get => innerList[index];

            set => throw new NotSupportedException("Immutable Lists cannot be edited.");
        }

        /// <summary>
        ///     Index accessor.
        /// </summary>
        /// <param name="index">Index.</param>
        public TElement this[int index] => innerList[index];

        /// <summary>
        ///     Returns <c>true</c> if the item is contained in the list.
        /// </summary>
        public bool Contains(TElement item) => innerList.Contains(item);

        /// <summary>
        ///     Copies the list to an array.
        /// </summary>
        public void CopyTo(TElement[] array, int arrayIndex) => innerList.CopyTo(array, arrayIndex);

        /// <summary>
        ///     Gets an enumerator.
        /// </summary>
        public IEnumerator<TElement> GetEnumerator() => innerList.GetEnumerator();

        void ICollection.CopyTo(Array array, int index) => innerList.CopyTo((TElement[])array, index);

        void ICollection<TElement>.Add(TElement item) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");

        void ICollection<TElement>.Clear() => throw new NotSupportedException("Immutable Lists cannot be edited.");

        bool ICollection<TElement>.Remove(TElement item) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        int IList.Add(object value) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.Clear() => throw new NotSupportedException("Immutable Lists cannot be edited.");

        bool IList.Contains(object value) => innerList.Contains((TElement)value);

        int IList.IndexOf(object value) => innerList.IndexOf((TElement)value);

        void IList.Insert(int index, object value) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.Remove(object value) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList<TElement>.Insert(int index, TElement item) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");

        void IList.RemoveAt(int index) => throw new NotSupportedException("Immutable Lists cannot be edited.");

        /// <summary>
        ///     Gets the index of an item.
        /// </summary>
        public int IndexOf(TElement item) => innerList.IndexOf(item);

        void IList<TElement>.RemoveAt(int index) =>
            throw new NotSupportedException("Immutable Lists cannot be edited.");
    }
}
