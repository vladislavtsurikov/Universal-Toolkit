﻿//-----------------------------------------------------------------------
// <copyright file="BinaryDataReader.cs" company="Sirenix IVS">
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
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OdinSerializer.Utilities.Unsafe;

namespace OdinSerializer
{
    /// <summary>
    ///     Reads data from a stream that has been written by a <see cref="BinaryDataWriter" />.
    /// </summary>
    /// <seealso cref="BaseDataReader" />
    public unsafe class BinaryDataReader : BaseDataReader
    {
        private static readonly Dictionary<Type, Delegate> PrimitiveFromByteMethods = new()
        {
            { typeof(char), (Func<byte[], int, char>)((b, i) => (char)ProperBitConverter.ToUInt16(b, i)) },
            { typeof(byte), (Func<byte[], int, byte>)((b, i) => b[i]) },
            { typeof(sbyte), (Func<byte[], int, sbyte>)((b, i) => (sbyte)b[i]) },
            { typeof(bool), (Func<byte[], int, bool>)((b, i) => b[i] == 0 ? false : true) },
            { typeof(short), (Func<byte[], int, short>)ProperBitConverter.ToInt16 },
            { typeof(int), (Func<byte[], int, int>)ProperBitConverter.ToInt32 },
            { typeof(long), (Func<byte[], int, long>)ProperBitConverter.ToInt64 },
            { typeof(ushort), (Func<byte[], int, ushort>)ProperBitConverter.ToUInt16 },
            { typeof(uint), (Func<byte[], int, uint>)ProperBitConverter.ToUInt32 },
            { typeof(ulong), (Func<byte[], int, ulong>)ProperBitConverter.ToUInt64 },
            { typeof(decimal), (Func<byte[], int, decimal>)ProperBitConverter.ToDecimal },
            { typeof(float), (Func<byte[], int, float>)ProperBitConverter.ToSingle },
            { typeof(double), (Func<byte[], int, double>)ProperBitConverter.ToDouble },
            { typeof(Guid), (Func<byte[], int, Guid>)ProperBitConverter.ToGuid }
        };

        private readonly Dictionary<int, Type> types = new(16);

        private byte[] buffer = new byte[1024 * 100];
        private int bufferEnd;

        private int bufferIndex;

        private byte[] internalBufferBackup;
        private BinaryEntryType peekedBinaryEntryType;
        private string peekedEntryName;

        private EntryType? peekedEntryType;

        public BinaryDataReader() : base(null, null) => internalBufferBackup = buffer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryDataReader" /> class.
        /// </summary>
        /// <param name="stream">The base stream of the reader.</param>
        /// <param name="context">The deserialization context to use.</param>
        public BinaryDataReader(Stream stream, DeserializationContext context) : base(stream, context) =>
            internalBufferBackup = buffer;

        /// <summary>
        ///     Disposes all resources kept by the data reader, except the stream, which can be reused later.
        /// </summary>
        public override void Dispose()
        {
            //this.Stream.Dispose();
        }

        /// <summary>
        ///     Peeks ahead and returns the type of the next entry in the stream.
        /// </summary>
        /// <param name="name">The name of the next entry, if it has one.</param>
        /// <returns>
        ///     The type of the next entry.
        /// </returns>
        public override EntryType PeekEntry(out string name)
        {
            if (peekedEntryType != null)
            {
                name = peekedEntryName;
                return (EntryType)peekedEntryType;
            }

            peekedBinaryEntryType =
                HasBufferData(1) ? (BinaryEntryType)buffer[bufferIndex++] : BinaryEntryType.EndOfStream;

            // Switch on entry type
            switch (peekedBinaryEntryType)
            {
                case BinaryEntryType.EndOfStream:
                    name = null;
                    peekedEntryName = null;
                    peekedEntryType = EntryType.EndOfStream;
                    break;

                case BinaryEntryType.NamedStartOfReferenceNode:
                case BinaryEntryType.NamedStartOfStructNode:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.StartOfNode;
                    break;

                case BinaryEntryType.UnnamedStartOfReferenceNode:
                case BinaryEntryType.UnnamedStartOfStructNode:
                    name = null;
                    peekedEntryType = EntryType.StartOfNode;
                    break;

                case BinaryEntryType.EndOfNode:
                    name = null;
                    peekedEntryType = EntryType.EndOfNode;
                    break;

                case BinaryEntryType.StartOfArray:
                    name = null;
                    peekedEntryType = EntryType.StartOfArray;
                    break;

                case BinaryEntryType.EndOfArray:
                    name = null;
                    peekedEntryType = EntryType.EndOfArray;
                    break;

                case BinaryEntryType.PrimitiveArray:
                    name = null;
                    peekedEntryType = EntryType.PrimitiveArray;
                    break;

                case BinaryEntryType.NamedInternalReference:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.InternalReference;
                    break;

                case BinaryEntryType.UnnamedInternalReference:
                    name = null;
                    peekedEntryType = EntryType.InternalReference;
                    break;

                case BinaryEntryType.NamedExternalReferenceByIndex:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.ExternalReferenceByIndex;
                    break;

                case BinaryEntryType.UnnamedExternalReferenceByIndex:
                    name = null;
                    peekedEntryType = EntryType.ExternalReferenceByIndex;
                    break;

                case BinaryEntryType.NamedExternalReferenceByGuid:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.ExternalReferenceByGuid;
                    break;

                case BinaryEntryType.UnnamedExternalReferenceByGuid:
                    name = null;
                    peekedEntryType = EntryType.ExternalReferenceByGuid;
                    break;

                case BinaryEntryType.NamedExternalReferenceByString:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.ExternalReferenceByString;
                    break;

                case BinaryEntryType.UnnamedExternalReferenceByString:
                    name = null;
                    peekedEntryType = EntryType.ExternalReferenceByString;
                    break;

                case BinaryEntryType.NamedSByte:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.UnnamedSByte:
                    name = null;
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.NamedByte:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.UnnamedByte:
                    name = null;
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.NamedShort:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.UnnamedShort:
                    name = null;
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.NamedUShort:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.UnnamedUShort:
                    name = null;
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.NamedInt:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.UnnamedInt:
                    name = null;
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.NamedUInt:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.UnnamedUInt:
                    name = null;
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.NamedLong:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.UnnamedLong:
                    name = null;
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.NamedULong:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.UnnamedULong:
                    name = null;
                    peekedEntryType = EntryType.Integer;
                    break;

                case BinaryEntryType.NamedFloat:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.FloatingPoint;
                    break;

                case BinaryEntryType.UnnamedFloat:
                    name = null;
                    peekedEntryType = EntryType.FloatingPoint;
                    break;

                case BinaryEntryType.NamedDouble:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.FloatingPoint;
                    break;

                case BinaryEntryType.UnnamedDouble:
                    name = null;
                    peekedEntryType = EntryType.FloatingPoint;
                    break;

                case BinaryEntryType.NamedDecimal:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.FloatingPoint;
                    break;

                case BinaryEntryType.UnnamedDecimal:
                    name = null;
                    peekedEntryType = EntryType.FloatingPoint;
                    break;

                case BinaryEntryType.NamedChar:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.String;
                    break;

                case BinaryEntryType.UnnamedChar:
                    name = null;
                    peekedEntryType = EntryType.String;
                    break;

                case BinaryEntryType.NamedString:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.String;
                    break;

                case BinaryEntryType.UnnamedString:
                    name = null;
                    peekedEntryType = EntryType.String;
                    break;

                case BinaryEntryType.NamedGuid:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Guid;
                    break;

                case BinaryEntryType.UnnamedGuid:
                    name = null;
                    peekedEntryType = EntryType.Guid;
                    break;

                case BinaryEntryType.NamedBoolean:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Boolean;
                    break;

                case BinaryEntryType.UnnamedBoolean:
                    name = null;
                    peekedEntryType = EntryType.Boolean;
                    break;

                case BinaryEntryType.NamedNull:
                    name = ReadStringValue();
                    peekedEntryType = EntryType.Null;
                    break;

                case BinaryEntryType.UnnamedNull:
                    name = null;
                    peekedEntryType = EntryType.Null;
                    break;

                case BinaryEntryType.TypeName:
                case BinaryEntryType.TypeID:
                    peekedBinaryEntryType = BinaryEntryType.Invalid;
                    peekedEntryType = EntryType.Invalid;
                    throw new InvalidOperationException(
                        "Invalid binary data stream: BinaryEntryType.TypeName and BinaryEntryType.TypeID must never be peeked by the binary reader.");

                case BinaryEntryType.Invalid:
                default:
                    name = null;
                    peekedBinaryEntryType = BinaryEntryType.Invalid;
                    peekedEntryType = EntryType.Invalid;
                    throw new InvalidOperationException(
                        "Invalid binary data stream: could not parse peeked BinaryEntryType byte '" +
                        (byte)peekedBinaryEntryType + "' into a known entry type.");
            }

            peekedEntryName = name;
            return peekedEntryType.Value;
        }

        /// <summary>
        ///     Tries to enters an array node. This will succeed if the next entry is an <see cref="EntryType.StartOfArray" />.
        ///     <para />
        ///     This call MUST (eventually) be followed by a corresponding call to <see cref="IDataReader.ExitArray()" />
        ///     <para />
        ///     This call will change the values of the <see cref="IDataReader.IsInArrayNode" />,
        ///     <see cref="IDataReader.CurrentNodeName" />, <see cref="IDataReader.CurrentNodeId" /> and
        ///     <see cref="IDataReader.CurrentNodeDepth" /> properties to the correct values for the current array node.
        /// </summary>
        /// <param name="length">The length of the array that was entered.</param>
        /// <returns>
        ///     <c>true</c> if an array was entered, otherwise <c>false</c>
        /// </returns>
        public override bool EnterArray(out long length)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedEntryType == EntryType.StartOfArray)
            {
                PushArray();
                MarkEntryContentConsumed();

                if (UNSAFE_Read_8_Int64(out length))
                {
                    if (length < 0)
                    {
                        length = 0;
                        Context.Config.DebugContext.LogError("Invalid array length: " + length + ".");
                        return false;
                    }

                    return true;
                }

                return false;
            }

            SkipEntry();
            length = 0;
            return false;
        }

        /// <summary>
        ///     Tries to enter a node. This will succeed if the next entry is an <see cref="EntryType.StartOfNode" />.
        ///     <para />
        ///     This call MUST (eventually) be followed by a corresponding call to <see cref="IDataReader.ExitNode()" />
        ///     <para />
        ///     This call will change the values of the <see cref="IDataReader.IsInArrayNode" />,
        ///     <see cref="IDataReader.CurrentNodeName" />, <see cref="IDataReader.CurrentNodeId" /> and
        ///     <see cref="IDataReader.CurrentNodeDepth" /> properties to the correct values for the current node.
        /// </summary>
        /// <param name="type">
        ///     The type of the node. This value will be null if there was no metadata, or if the reader's
        ///     serialization binder failed to resolve the type name.
        /// </param>
        /// <returns>
        ///     <c>true</c> if entering a node succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool EnterNode(out Type type)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedStartOfReferenceNode ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedStartOfReferenceNode)
            {
                MarkEntryContentConsumed();
                type = ReadTypeEntry();
                int id;

                if (!UNSAFE_Read_4_Int32(out id))
                {
                    type = null;
                    return false;
                }

                PushNode(peekedEntryName, id, type);
                return true;
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedStartOfStructNode ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedStartOfStructNode)
            {
                type = ReadTypeEntry();
                PushNode(peekedEntryName, -1, type);
                MarkEntryContentConsumed();
                return true;
            }

            SkipEntry();
            type = null;
            return false;
        }

        /// <summary>
        ///     Exits the closest array. This method will keep skipping entries using <see cref="IDataReader.SkipEntry()" /> until
        ///     an <see cref="EntryType.EndOfArray" /> is reached, or the end of the stream is reached.
        ///     <para />
        ///     This call MUST have been preceded by a corresponding call to <see cref="IDataReader.EnterArray(out long)" />.
        ///     <para />
        ///     This call will change the values of the <see cref="IDataReader.IsInArrayNode" />,
        ///     <see cref="IDataReader.CurrentNodeName" />, <see cref="IDataReader.CurrentNodeId" /> and
        ///     <see cref="IDataReader.CurrentNodeDepth" /> to the correct values for the node that was prior to the exited array
        ///     node.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the method exited an array, <c>false</c> if it reached the end of the stream.
        /// </returns>
        public override bool ExitArray()
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            while (peekedBinaryEntryType != BinaryEntryType.EndOfArray &&
                   peekedBinaryEntryType != BinaryEntryType.EndOfStream)
            {
                if (peekedEntryType == EntryType.EndOfNode)
                {
                    Context.Config.DebugContext.LogError(
                        "Data layout mismatch; skipping past node boundary when exiting array.");
                    MarkEntryContentConsumed();
                }

                SkipEntry();
            }

            if (peekedBinaryEntryType == BinaryEntryType.EndOfArray)
            {
                MarkEntryContentConsumed();
                PopArray();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Exits the current node. This method will keep skipping entries using <see cref="IDataReader.SkipEntry()" /> until
        ///     an <see cref="EntryType.EndOfNode" /> is reached, or the end of the stream is reached.
        ///     <para />
        ///     This call MUST have been preceded by a corresponding call to <see cref="IDataReader.EnterNode(out Type)" />.
        ///     <para />
        ///     This call will change the values of the <see cref="IDataReader.IsInArrayNode" />,
        ///     <see cref="IDataReader.CurrentNodeName" />, <see cref="IDataReader.CurrentNodeId" /> and
        ///     <see cref="IDataReader.CurrentNodeDepth" /> to the correct values for the node that was prior to the current node.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the method exited a node, <c>false</c> if it reached the end of the stream.
        /// </returns>
        public override bool ExitNode()
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            while (peekedBinaryEntryType != BinaryEntryType.EndOfNode &&
                   peekedBinaryEntryType != BinaryEntryType.EndOfStream)
            {
                if (peekedEntryType == EntryType.EndOfArray)
                {
                    Context.Config.DebugContext.LogError(
                        "Data layout mismatch; skipping past array boundary when exiting node.");
                    MarkEntryContentConsumed();
                }

                SkipEntry();
            }

            if (peekedBinaryEntryType == BinaryEntryType.EndOfNode)
            {
                MarkEntryContentConsumed();
                PopNode(CurrentNodeName);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Reads a primitive array value. This call will succeed if the next entry is an
        ///     <see cref="EntryType.PrimitiveArray" />.
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The element type of the primitive array. Valid element types can be determined using
        ///     <see cref="FormatterUtilities.IsPrimitiveArrayType(Type)" />.
        /// </typeparam>
        /// <param name="array">The resulting primitive array.</param>
        /// <returns>
        ///     <c>true</c> if reading a primitive array succeeded, otherwise <c>false</c>
        /// </returns>
        /// <exception cref="System.ArgumentException">Type  + typeof(T).Name +  is not a valid primitive array type.</exception>
        public override bool ReadPrimitiveArray<T>(out T[] array)
        {
            if (FormatterUtilities.IsPrimitiveArrayType(typeof(T)) == false)
            {
                throw new ArgumentException("Type " + typeof(T).Name + " is not a valid primitive array type.");
            }

            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedEntryType == EntryType.PrimitiveArray)
            {
                MarkEntryContentConsumed();

                int elementCount;
                int bytesPerElement;

                if (!UNSAFE_Read_4_Int32(out elementCount) || !UNSAFE_Read_4_Int32(out bytesPerElement))
                {
                    array = null;
                    return false;
                }

                var byteCount = elementCount * bytesPerElement;

                if (!HasBufferData(byteCount))
                {
                    bufferIndex = bufferEnd; // We're done!
                    array = null;
                    return false;
                }

                // Read the actual array content
                if (typeof(T) == typeof(byte))
                {
                    // We can include a special case for byte arrays, as there's no need to copy that to a buffer
                    var byteArray = new byte[byteCount];

                    Buffer.BlockCopy(buffer, bufferIndex, byteArray, 0, byteCount);

                    array = (T[])(object)byteArray;

                    bufferIndex += byteCount;

                    return true;
                }

                array = new T[elementCount];

                // We always store in little endian, so we can do a direct memory mapping, which is a lot faster
                if (BitConverter.IsLittleEndian)
                {
                    var toHandle = GCHandle.Alloc(array, GCHandleType.Pinned);

                    try
                    {
                        fixed (byte* fromBase = buffer)
                        {
                            void* from = fromBase + bufferIndex;
                            void* to = toHandle.AddrOfPinnedObject().ToPointer();
                            UnsafeUtilities.MemoryCopy(from, to, byteCount);
                        }
                    }
                    finally
                    {
                        toHandle.Free();
                    }
                }
                else
                {
                    // We have to convert each individual element from bytes, since the byte order has to be reversed
                    var fromBytes = (Func<byte[], int, T>)PrimitiveFromByteMethods[typeof(T)];

                    for (var i = 0; i < elementCount; i++)
                    {
                        array[i] = fromBytes(buffer, bufferIndex + i * bytesPerElement);
                    }
                }

                bufferIndex += byteCount;
                return true;
            }

            SkipEntry();
            array = null;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="bool" /> value. This call will succeed if the next entry is an <see cref="EntryType.Boolean" />.
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadBoolean(out bool value)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedEntryType == EntryType.Boolean)
            {
                MarkEntryContentConsumed();

                if (HasBufferData(1))
                {
                    value = buffer[bufferIndex++] == 1;
                    return true;
                }

                value = false;
                return false;
            }

            SkipEntry();
            value = default;
            return false;
        }

        /// <summary>
        ///     Reads an <see cref="sbyte" /> value. This call will succeed if the next entry is an
        ///     <see cref="EntryType.Integer" />.
        ///     <para />
        ///     If the value of the stored integer is smaller than <see cref="sbyte.MinValue" /> or larger than
        ///     <see cref="sbyte.MaxValue" />, the result will be default(<see cref="sbyte" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadSByte(out sbyte value)
        {
            long longValue;
            if (ReadInt64(out longValue))
            {
                checked
                {
                    try
                    {
                        value = (sbyte)longValue;
                    }
                    catch (OverflowException)
                    {
                        value = default;
                    }
                }

                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="byte" /> value. This call will succeed if the next entry is an <see cref="EntryType.Integer" />.
        ///     <para />
        ///     If the value of the stored integer is smaller than <see cref="byte.MinValue" /> or larger than
        ///     <see cref="byte.MaxValue" />, the result will be default(<see cref="byte" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadByte(out byte value)
        {
            ulong ulongValue;
            if (ReadUInt64(out ulongValue))
            {
                checked
                {
                    try
                    {
                        value = (byte)ulongValue;
                    }
                    catch (OverflowException)
                    {
                        value = default;
                    }
                }

                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="short" /> value. This call will succeed if the next entry is an <see cref="EntryType.Integer" />
        ///     .
        ///     <para />
        ///     If the value of the stored integer is smaller than <see cref="short.MinValue" /> or larger than
        ///     <see cref="short.MaxValue" />, the result will be default(<see cref="short" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadInt16(out short value)
        {
            long longValue;
            if (ReadInt64(out longValue))
            {
                checked
                {
                    try
                    {
                        value = (short)longValue;
                    }
                    catch (OverflowException)
                    {
                        value = default;
                    }
                }

                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Reads an <see cref="ushort" /> value. This call will succeed if the next entry is an
        ///     <see cref="EntryType.Integer" />.
        ///     <para />
        ///     If the value of the stored integer is smaller than <see cref="ushort.MinValue" /> or larger than
        ///     <see cref="ushort.MaxValue" />, the result will be default(<see cref="ushort" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadUInt16(out ushort value)
        {
            ulong ulongValue;
            if (ReadUInt64(out ulongValue))
            {
                checked
                {
                    try
                    {
                        value = (ushort)ulongValue;
                    }
                    catch (OverflowException)
                    {
                        value = default;
                    }
                }

                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Reads an <see cref="int" /> value. This call will succeed if the next entry is an <see cref="EntryType.Integer" />.
        ///     <para />
        ///     If the value of the stored integer is smaller than <see cref="int.MinValue" /> or larger than
        ///     <see cref="int.MaxValue" />, the result will be default(<see cref="int" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadInt32(out int value)
        {
            long longValue;
            if (ReadInt64(out longValue))
            {
                checked
                {
                    try
                    {
                        value = (int)longValue;
                    }
                    catch (OverflowException)
                    {
                        value = default;
                    }
                }

                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Reads an <see cref="uint" /> value. This call will succeed if the next entry is an <see cref="EntryType.Integer" />
        ///     .
        ///     <para />
        ///     If the value of the stored integer is smaller than <see cref="uint.MinValue" /> or larger than
        ///     <see cref="uint.MaxValue" />, the result will be default(<see cref="uint" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadUInt32(out uint value)
        {
            ulong ulongValue;
            if (ReadUInt64(out ulongValue))
            {
                checked
                {
                    try
                    {
                        value = (uint)ulongValue;
                    }
                    catch (OverflowException)
                    {
                        value = default;
                    }
                }

                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="long" /> value. This call will succeed if the next entry is an <see cref="EntryType.Integer" />.
        ///     <para />
        ///     If the value of the stored integer is smaller than <see cref="long.MinValue" /> or larger than
        ///     <see cref="long.MaxValue" />, the result will be default(<see cref="long" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadInt64(out long value)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedEntryType == EntryType.Integer)
            {
                try
                {
                    switch (peekedBinaryEntryType)
                    {
                        case BinaryEntryType.NamedSByte:
                        case BinaryEntryType.UnnamedSByte:
                            sbyte i8;
                            if (UNSAFE_Read_1_SByte(out i8))
                            {
                                value = i8;
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;
                        case BinaryEntryType.NamedByte:
                        case BinaryEntryType.UnnamedByte:
                            byte ui8;
                            if (UNSAFE_Read_1_Byte(out ui8))
                            {
                                value = ui8;
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedShort:
                        case BinaryEntryType.UnnamedShort:
                            short i16;
                            if (UNSAFE_Read_2_Int16(out i16))
                            {
                                value = i16;
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedUShort:
                        case BinaryEntryType.UnnamedUShort:
                            ushort ui16;
                            if (UNSAFE_Read_2_UInt16(out ui16))
                            {
                                value = ui16;
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedInt:
                        case BinaryEntryType.UnnamedInt:
                            int i32;
                            if (UNSAFE_Read_4_Int32(out i32))
                            {
                                value = i32;
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedUInt:
                        case BinaryEntryType.UnnamedUInt:
                            uint ui32;
                            if (UNSAFE_Read_4_UInt32(out ui32))
                            {
                                value = ui32;
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedLong:
                        case BinaryEntryType.UnnamedLong:
                            if (!UNSAFE_Read_8_Int64(out value))
                            {
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedULong:
                        case BinaryEntryType.UnnamedULong:
                            ulong uint64;
                            if (UNSAFE_Read_8_UInt64(out uint64))
                            {
                                if (uint64 > long.MaxValue)
                                {
                                    value = 0;
                                    return false;
                                }
                                else
                                {
                                    value = (long)uint64;
                                }
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    return true;
                }
                finally
                {
                    MarkEntryContentConsumed();
                }
            }

            SkipEntry();
            value = default;
            return false;
        }

        /// <summary>
        ///     Reads an <see cref="ulong" /> value. This call will succeed if the next entry is an
        ///     <see cref="EntryType.Integer" />.
        ///     <para />
        ///     If the value of the stored integer is smaller than <see cref="ulong.MinValue" /> or larger than
        ///     <see cref="ulong.MaxValue" />, the result will be default(<see cref="ulong" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadUInt64(out ulong value)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedEntryType == EntryType.Integer)
            {
                try
                {
                    switch (peekedBinaryEntryType)
                    {
                        case BinaryEntryType.NamedSByte:
                        case BinaryEntryType.UnnamedSByte:
                        case BinaryEntryType.NamedByte:
                        case BinaryEntryType.UnnamedByte:
                            byte i8;
                            if (UNSAFE_Read_1_Byte(out i8))
                            {
                                value = i8;
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedShort:
                        case BinaryEntryType.UnnamedShort:
                            short i16;
                            if (UNSAFE_Read_2_Int16(out i16))
                            {
                                if (i16 >= 0)
                                {
                                    value = (ulong)i16;
                                }
                                else
                                {
                                    value = 0;
                                    return false;
                                }
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedUShort:
                        case BinaryEntryType.UnnamedUShort:
                            ushort ui16;
                            if (UNSAFE_Read_2_UInt16(out ui16))
                            {
                                value = ui16;
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedInt:
                        case BinaryEntryType.UnnamedInt:
                            int i32;
                            if (UNSAFE_Read_4_Int32(out i32))
                            {
                                if (i32 >= 0)
                                {
                                    value = (ulong)i32;
                                }
                                else
                                {
                                    value = 0;
                                    return false;
                                }
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedUInt:
                        case BinaryEntryType.UnnamedUInt:
                            uint ui32;
                            if (UNSAFE_Read_4_UInt32(out ui32))
                            {
                                value = ui32;
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedLong:
                        case BinaryEntryType.UnnamedLong:
                            long i64;
                            if (UNSAFE_Read_8_Int64(out i64))
                            {
                                if (i64 >= 0)
                                {
                                    value = (ulong)i64;
                                }
                                else
                                {
                                    value = 0;
                                    return false;
                                }
                            }
                            else
                            {
                                value = 0;
                                return false;
                            }

                            break;

                        case BinaryEntryType.NamedULong:
                        case BinaryEntryType.UnnamedULong:
                            if (!UNSAFE_Read_8_UInt64(out value))
                            {
                                return false;
                            }

                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    return true;
                }
                finally
                {
                    MarkEntryContentConsumed();
                }
            }

            SkipEntry();
            value = default;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="char" /> value. This call will succeed if the next entry is an <see cref="EntryType.String" />.
        ///     <para />
        ///     If the string of the entry is longer than 1 character, the first character of the string will be taken as the
        ///     result.
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadChar(out char value)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedChar ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedChar)
            {
                MarkEntryContentConsumed();
                return UNSAFE_Read_2_Char(out value);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedString ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedString)
            {
                MarkEntryContentConsumed();
                var str = ReadStringValue();

                if (str == null || str.Length == 0)
                {
                    value = default;
                    return false;
                }

                value = str[0];
                return true;
            }

            SkipEntry();
            value = default;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="float" /> value. This call will succeed if the next entry is an
        ///     <see cref="EntryType.FloatingPoint" /> or an <see cref="EntryType.Integer" />.
        ///     <para />
        ///     If the stored integer or floating point value is smaller than <see cref="float.MinValue" /> or larger than
        ///     <see cref="float.MaxValue" />, the result will be default(<see cref="float" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadSingle(out float value)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedFloat ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedFloat)
            {
                MarkEntryContentConsumed();
                return UNSAFE_Read_4_Float32(out value);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedDouble ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedDouble)
            {
                MarkEntryContentConsumed();

                double d;
                if (!UNSAFE_Read_8_Float64(out d))
                {
                    value = 0;
                    return false;
                }

                try
                {
                    checked
                    {
                        value = (float)d;
                    }
                }
                catch (OverflowException)
                {
                    value = default;
                }

                return true;
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedDecimal ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedDecimal)
            {
                MarkEntryContentConsumed();

                decimal d;
                if (!UNSAFE_Read_16_Decimal(out d))
                {
                    value = 0;
                    return false;
                }

                try
                {
                    checked
                    {
                        value = (float)d;
                    }
                }
                catch (OverflowException)
                {
                    value = default;
                }

                return true;
            }

            if (peekedEntryType == EntryType.Integer)
            {
                long val;
                if (!ReadInt64(out val))
                {
                    value = 0;
                    return false;
                }

                try
                {
                    checked
                    {
                        value = val;
                    }
                }
                catch (OverflowException)
                {
                    value = default;
                }

                return true;
            }

            SkipEntry();
            value = default;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="double" /> value. This call will succeed if the next entry is an
        ///     <see cref="EntryType.FloatingPoint" /> or an <see cref="EntryType.Integer" />.
        ///     <para />
        ///     If the stored integer or floating point value is smaller than <see cref="double.MinValue" /> or larger than
        ///     <see cref="double.MaxValue" />, the result will be default(<see cref="double" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadDouble(out double value)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedDouble ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedDouble)
            {
                MarkEntryContentConsumed();
                return UNSAFE_Read_8_Float64(out value);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedFloat ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedFloat)
            {
                MarkEntryContentConsumed();

                float s;
                if (!UNSAFE_Read_4_Float32(out s))
                {
                    value = 0;
                    return false;
                }

                value = s;
                return true;
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedDecimal ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedDecimal)
            {
                MarkEntryContentConsumed();

                decimal d;
                if (!UNSAFE_Read_16_Decimal(out d))
                {
                    value = 0;
                    return false;
                }

                try
                {
                    checked
                    {
                        value = (double)d;
                    }
                }
                catch (OverflowException)
                {
                    value = 0;
                }

                return true;
            }

            if (peekedEntryType == EntryType.Integer)
            {
                long val;
                if (!ReadInt64(out val))
                {
                    value = 0;
                    return false;
                }

                try
                {
                    checked
                    {
                        value = val;
                    }
                }
                catch (OverflowException)
                {
                    value = 0;
                }

                return true;
            }

            SkipEntry();
            value = default;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="decimal" /> value. This call will succeed if the next entry is an
        ///     <see cref="EntryType.FloatingPoint" /> or an <see cref="EntryType.Integer" />.
        ///     <para />
        ///     If the stored integer or floating point value is smaller than <see cref="decimal.MinValue" /> or larger than
        ///     <see cref="decimal.MaxValue" />, the result will be default(<see cref="decimal" />).
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadDecimal(out decimal value)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedDecimal ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedDecimal)
            {
                MarkEntryContentConsumed();
                return UNSAFE_Read_16_Decimal(out value);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedDouble ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedDouble)
            {
                MarkEntryContentConsumed();

                double d;
                if (!UNSAFE_Read_8_Float64(out d))
                {
                    value = 0;
                    return false;
                }

                try
                {
                    checked
                    {
                        value = (decimal)d;
                    }
                }
                catch (OverflowException)
                {
                    value = default;
                }

                return true;
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedFloat ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedFloat)
            {
                MarkEntryContentConsumed();

                float f;
                if (!UNSAFE_Read_4_Float32(out f))
                {
                    value = 0;
                    return false;
                }

                try
                {
                    checked
                    {
                        value = (decimal)f;
                    }
                }
                catch (OverflowException)
                {
                    value = default;
                }

                return true;
            }

            if (peekedEntryType == EntryType.Integer)
            {
                long val;
                if (!ReadInt64(out val))
                {
                    value = 0;
                    return false;
                }

                try
                {
                    checked
                    {
                        value = val;
                    }
                }
                catch (OverflowException)
                {
                    value = default;
                }

                return true;
            }

            SkipEntry();
            value = default;
            return false;
        }

        /// <summary>
        ///     Reads an external reference guid. This call will succeed if the next entry is an
        ///     <see cref="EntryType.ExternalReferenceByGuid" />.
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="guid">The external reference guid.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadExternalReference(out Guid guid)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedExternalReferenceByGuid ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedExternalReferenceByGuid)
            {
                MarkEntryContentConsumed();
                return UNSAFE_Read_16_Guid(out guid);
            }

            SkipEntry();
            guid = default;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="Guid" /> value. This call will succeed if the next entry is an <see cref="EntryType.Guid" />.
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadGuid(out Guid value)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedGuid ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedGuid)
            {
                MarkEntryContentConsumed();
                return UNSAFE_Read_16_Guid(out value);
            }

            SkipEntry();
            value = default;
            return false;
        }

        /// <summary>
        ///     Reads an external reference index. This call will succeed if the next entry is an
        ///     <see cref="EntryType.ExternalReferenceByIndex" />.
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="index">The external reference index.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadExternalReference(out int index)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedExternalReferenceByIndex ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedExternalReferenceByIndex)
            {
                MarkEntryContentConsumed();
                return UNSAFE_Read_4_Int32(out index);
            }

            SkipEntry();
            index = -1;
            return false;
        }

        /// <summary>
        ///     Reads an external reference string. This call will succeed if the next entry is an
        ///     <see cref="EntryType.ExternalReferenceByString" />.
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="id">The external reference string.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadExternalReference(out string id)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedExternalReferenceByString ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedExternalReferenceByString)
            {
                id = ReadStringValue();
                MarkEntryContentConsumed();
                return id != null;
            }

            SkipEntry();
            id = null;
            return false;
        }

        /// <summary>
        ///     Reads a <c>null</c> value. This call will succeed if the next entry is an <see cref="EntryType.Null" />.
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadNull()
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedNull ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedNull)
            {
                MarkEntryContentConsumed();
                return true;
            }

            SkipEntry();
            return false;
        }

        /// <summary>
        ///     Reads an internal reference id. This call will succeed if the next entry is an
        ///     <see cref="EntryType.InternalReference" />.
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="id">The internal reference id.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadInternalReference(out int id)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedInternalReference ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedInternalReference)
            {
                MarkEntryContentConsumed();
                return UNSAFE_Read_4_Int32(out id);
            }

            SkipEntry();
            id = -1;
            return false;
        }

        /// <summary>
        ///     Reads a <see cref="string" /> value. This call will succeed if the next entry is an <see cref="EntryType.String" />
        ///     .
        ///     <para />
        ///     If the call fails (and returns <c>false</c>), it will skip the current entry value, unless that entry is an
        ///     <see cref="EntryType.EndOfNode" /> or an <see cref="EntryType.EndOfArray" />.
        /// </summary>
        /// <param name="value">The value that has been read.</param>
        /// <returns>
        ///     <c>true</c> if reading the value succeeded, otherwise <c>false</c>
        /// </returns>
        public override bool ReadString(out string value)
        {
            if (!peekedEntryType.HasValue)
            {
                string name;
                PeekEntry(out name);
            }

            if (peekedBinaryEntryType == BinaryEntryType.NamedString ||
                peekedBinaryEntryType == BinaryEntryType.UnnamedString)
            {
                value = ReadStringValue();
                MarkEntryContentConsumed();
                return value != null;
            }

            SkipEntry();
            value = null;
            return false;
        }

        /// <summary>
        ///     Tells the reader that a new serialization session is about to begin, and that it should clear all cached values
        ///     left over from any prior serialization sessions.
        ///     This method is only relevant when the same reader is used to deserialize several different, unrelated values.
        /// </summary>
        public override void PrepareNewSerializationSession()
        {
            base.PrepareNewSerializationSession();
            peekedEntryType = null;
            peekedEntryName = null;
            peekedBinaryEntryType = BinaryEntryType.Invalid;
            types.Clear();
            bufferIndex = 0;
            bufferEnd = 0;
            buffer = internalBufferBackup;
        }

        public override string GetDataDump()
        {
            byte[] bytes;

            if (bufferEnd == buffer.Length)
            {
                bytes = buffer;
            }
            else
            {
                bytes = new byte[bufferEnd];

                fixed (void* from = buffer)
                fixed (void* to = bytes)
                {
                    UnsafeUtilities.MemoryCopy(from, to, bytes.Length);
                }
            }

            return "Binary hex dump: " + ProperBitConverter.BytesToHexString(bytes);
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private string ReadStringValue()
        {
            byte charSizeFlag;

            if (!UNSAFE_Read_1_Byte(out charSizeFlag))
            {
                return null;
            }

            int length;

            if (!UNSAFE_Read_4_Int32(out length))
            {
                return null;
            }

            var str = new string(' ', length);

            if (charSizeFlag == 0)
            {
                // 8 bit

                fixed (byte* baseFromPtr = buffer)
                fixed (char* baseToPtr = str)
                {
                    var fromPtr = baseFromPtr + bufferIndex;
                    var toPtr = (byte*)baseToPtr;

                    if (BitConverter.IsLittleEndian)
                    {
                        for (var i = 0; i < length; i++)
                        {
                            *toPtr++ = *fromPtr++;
                            toPtr++; // Skip every other string byte
                        }
                    }
                    else
                    {
                        for (var i = 0; i < length; i++)
                        {
                            toPtr++; // Skip every other string byte
                            *toPtr++ = *fromPtr++;
                        }
                    }
                }

                bufferIndex += length;
                return str;
            }

            // 16 bit
            var bytes = length * 2;

            fixed (byte* baseFromPtr = buffer)
            fixed (char* baseToPtr = str)
            {
                if (BitConverter.IsLittleEndian)
                {
                    var fromLargePtr = (Struct256Bit*)(baseFromPtr + bufferIndex);
                    var toLargePtr = (Struct256Bit*)baseToPtr;

                    var end = (byte*)baseToPtr + bytes;

                    while (toLargePtr + 1 < end)
                    {
                        *toLargePtr++ = *fromLargePtr++;
                    }

                    var fromSmallPtr = (byte*)fromLargePtr;
                    var toSmallPtr = (byte*)toLargePtr;

                    while (toSmallPtr < end)
                    {
                        *toSmallPtr++ = *fromSmallPtr++;
                    }
                }
                else
                {
                    var fromPtr = baseFromPtr + bufferIndex;
                    var toPtr = (byte*)baseToPtr;

                    for (var i = 0; i < length; i++)
                    {
                        *toPtr = *(fromPtr + 1);
                        *(toPtr + 1) = *fromPtr;

                        fromPtr += 2;
                        toPtr += 2;
                    }
                }
            }

            bufferIndex += bytes;
            return str;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private void SkipStringValue()
        {
            byte charSizeFlag;

            if (!UNSAFE_Read_1_Byte(out charSizeFlag))
            {
                return;
            }

            int skipBytes;

            if (!UNSAFE_Read_4_Int32(out skipBytes))
            {
                return;
            }

            if (charSizeFlag != 0)
            {
                skipBytes *= 2;
            }

            if (HasBufferData(skipBytes))
            {
                bufferIndex += skipBytes;
            }
            else
            {
                bufferIndex = bufferEnd;
            }
        }

        private void SkipPeekedEntryContent()
        {
            if (peekedEntryType != null)
            {
                try
                {
                    switch (peekedBinaryEntryType)
                    {
                        case BinaryEntryType.NamedStartOfReferenceNode:
                        case BinaryEntryType.UnnamedStartOfReferenceNode:
                            ReadTypeEntry(); // Never actually skip type entries; they might contain type ids that we'll need later
                            if (!SkipBuffer(4))
                            {
                                return; // Skip reference id int
                            }

                            break;

                        case BinaryEntryType.NamedStartOfStructNode:
                        case BinaryEntryType.UnnamedStartOfStructNode:
                            ReadTypeEntry(); // Never actually skip type entries; they might contain type ids that we'll need later
                            break;

                        case BinaryEntryType.StartOfArray:
                            // Skip length long
                            SkipBuffer(8);

                            break;

                        case BinaryEntryType.PrimitiveArray:
                            // We must skip the whole entry array content
                            int elements;
                            int bytesPerElement;

                            if (!UNSAFE_Read_4_Int32(out elements) || !UNSAFE_Read_4_Int32(out bytesPerElement))
                            {
                                return;
                            }

                            SkipBuffer(elements * bytesPerElement);
                            break;

                        case BinaryEntryType.NamedSByte:
                        case BinaryEntryType.UnnamedSByte:
                        case BinaryEntryType.NamedByte:
                        case BinaryEntryType.UnnamedByte:
                        case BinaryEntryType.NamedBoolean:
                        case BinaryEntryType.UnnamedBoolean:
                            SkipBuffer(1);
                            break;

                        case BinaryEntryType.NamedChar:
                        case BinaryEntryType.UnnamedChar:
                        case BinaryEntryType.NamedShort:
                        case BinaryEntryType.UnnamedShort:
                        case BinaryEntryType.NamedUShort:
                        case BinaryEntryType.UnnamedUShort:
                            SkipBuffer(2);
                            break;

                        case BinaryEntryType.NamedInternalReference:
                        case BinaryEntryType.UnnamedInternalReference:
                        case BinaryEntryType.NamedInt:
                        case BinaryEntryType.UnnamedInt:
                        case BinaryEntryType.NamedUInt:
                        case BinaryEntryType.UnnamedUInt:
                        case BinaryEntryType.NamedExternalReferenceByIndex:
                        case BinaryEntryType.UnnamedExternalReferenceByIndex:
                        case BinaryEntryType.NamedFloat:
                        case BinaryEntryType.UnnamedFloat:
                            SkipBuffer(4);
                            break;

                        case BinaryEntryType.NamedLong:
                        case BinaryEntryType.UnnamedLong:
                        case BinaryEntryType.NamedULong:
                        case BinaryEntryType.UnnamedULong:
                        case BinaryEntryType.NamedDouble:
                        case BinaryEntryType.UnnamedDouble:
                            SkipBuffer(8);
                            break;

                        case BinaryEntryType.NamedGuid:
                        case BinaryEntryType.UnnamedGuid:
                        case BinaryEntryType.NamedExternalReferenceByGuid:
                        case BinaryEntryType.UnnamedExternalReferenceByGuid:
                        case BinaryEntryType.NamedDecimal:
                        case BinaryEntryType.UnnamedDecimal:
                            SkipBuffer(8);
                            break;

                        case BinaryEntryType.NamedString:
                        case BinaryEntryType.UnnamedString:
                        case BinaryEntryType.NamedExternalReferenceByString:
                        case BinaryEntryType.UnnamedExternalReferenceByString:
                            SkipStringValue();
                            break;

                        case BinaryEntryType.TypeName:
                            Context.Config.DebugContext.LogError(
                                "Parsing error in binary data reader: should not be able to peek a TypeName entry.");
                            SkipBuffer(4);
                            ReadStringValue();
                            break;

                        case BinaryEntryType.TypeID:
                            Context.Config.DebugContext.LogError(
                                "Parsing error in binary data reader: should not be able to peek a TypeID entry.");
                            SkipBuffer(4);
                            break;

                        case BinaryEntryType.EndOfArray:
                        case BinaryEntryType.EndOfNode:
                        case BinaryEntryType.NamedNull:
                        case BinaryEntryType.UnnamedNull:
                        case BinaryEntryType.EndOfStream:
                        case BinaryEntryType.Invalid:
                        default:
                            // Skip nothing - there is no content to skip
                            break;
                    }
                }
                finally
                {
                    MarkEntryContentConsumed();
                }
            }
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool SkipBuffer(int amount)
        {
            var newIndex = bufferIndex + amount;

            if (newIndex > bufferEnd)
            {
                bufferIndex = bufferEnd;
                return false;
            }

            bufferIndex = newIndex;
            return true;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private Type ReadTypeEntry()
        {
            if (!HasBufferData(1))
            {
                return null;
            }

            var entryType = (BinaryEntryType)buffer[bufferIndex++];

            Type type;
            int id;

            if (entryType == BinaryEntryType.TypeID)
            {
                if (!UNSAFE_Read_4_Int32(out id))
                {
                    return null;
                }

                if (types.TryGetValue(id, out type) == false)
                {
                    Context.Config.DebugContext.LogError("Missing type ID during deserialization: " + id + " at node " +
                                                         CurrentNodeName + " and depth " + CurrentNodeDepth +
                                                         " and id " + CurrentNodeId);
                }
            }
            else if (entryType == BinaryEntryType.TypeName)
            {
                if (!UNSAFE_Read_4_Int32(out id))
                {
                    return null;
                }

                var name = ReadStringValue();
                type = name == null ? null : Context.Binder.BindToType(name, Context.Config.DebugContext);
                types.Add(id, type);
            }
            else if (entryType == BinaryEntryType.UnnamedNull)
            {
                type = null;
            }
            else
            {
                type = null;
                Context.Config.DebugContext.LogError(
                    "Expected TypeName, TypeID or UnnamedNull entry flag for reading type data, but instead got the entry flag: " +
                    entryType + ".");
            }

            return type;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private void MarkEntryContentConsumed()
        {
            peekedEntryType = null;
            peekedEntryName = null;
            peekedBinaryEntryType = BinaryEntryType.Invalid;
        }

        /// <summary>
        ///     Peeks the current entry.
        /// </summary>
        /// <returns>The peeked entry.</returns>
        protected override EntryType PeekEntry()
        {
            string name;
            return PeekEntry(out name);
        }

        /// <summary>
        ///     Consumes the current entry, and reads to the next one.
        /// </summary>
        /// <returns>The next entry.</returns>
        protected override EntryType ReadToNextEntry()
        {
            string name;
            SkipPeekedEntryContent();
            return PeekEntry(out name);
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_1_Byte(out byte value)
        {
            if (HasBufferData(1))
            {
                value = buffer[bufferIndex++];
                return true;
            }

            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_1_SByte(out sbyte value)
        {
            if (HasBufferData(1))
            {
                unchecked
                {
                    value = (sbyte)buffer[bufferIndex++];
                }

                return true;
            }

            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_2_Int16(out short value)
        {
            if (HasBufferData(2))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        value = *(short*)(basePtr + bufferIndex);
                    }
                    else
                    {
                        short val = 0;
                        var toPtr = (byte*)&val + 1;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 2;
                return true;
            }

            bufferIndex = bufferEnd;
            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_2_UInt16(out ushort value)
        {
            if (HasBufferData(2))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        value = *(ushort*)(basePtr + bufferIndex);
                    }
                    else
                    {
                        ushort val = 0;
                        var toPtr = (byte*)&val + 1;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 2;
                return true;
            }

            bufferIndex = bufferEnd;
            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_2_Char(out char value)
        {
            if (HasBufferData(2))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        value = *(char*)(basePtr + bufferIndex);
                    }
                    else
                    {
                        var val = default(char);
                        var toPtr = (byte*)&val + 1;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 2;
                return true;
            }

            bufferIndex = bufferEnd;
            value = default;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_4_Int32(out int value)
        {
            if (HasBufferData(4))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        value = *(int*)(basePtr + bufferIndex);
                    }
                    else
                    {
                        var val = 0;
                        var toPtr = (byte*)&val + 3;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 4;
                return true;
            }

            bufferIndex = bufferEnd;
            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_4_UInt32(out uint value)
        {
            if (HasBufferData(4))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        value = *(uint*)(basePtr + bufferIndex);
                    }
                    else
                    {
                        uint val = 0;
                        var toPtr = (byte*)&val + 3;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 4;
                return true;
            }

            bufferIndex = bufferEnd;
            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_4_Float32(out float value)
        {
            if (HasBufferData(4))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        if (ArchitectureInfo.Architecture_Supports_Unaligned_Float32_Reads)
                        {
                            // We can read directly from the buffer, safe in the knowledge that any potential unaligned reads will work
                            value = *(float*)(basePtr + bufferIndex);
                        }
                        else
                        {
                            // We do a read through a 32-bit int and a locally addressed float instead, should be almost as fast as the real deal
                            float result = 0;
                            *(int*)&result = *(int*)(basePtr + bufferIndex);
                            value = result;
                        }
                    }
                    else
                    {
                        float val = 0;
                        var toPtr = (byte*)&val + 3;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 4;
                return true;
            }

            bufferIndex = bufferEnd;
            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_8_Int64(out long value)
        {
            if (HasBufferData(8))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        if (ArchitectureInfo.Architecture_Supports_All_Unaligned_ReadWrites)
                        {
                            // We can read directly from the buffer, safe in the knowledge that any potential unaligned reads will work
                            value = *(long*)(basePtr + bufferIndex);
                        }
                        else
                        {
                            // We do an int-by-int read instead, into an address that we know is aligned
                            long result = 0;
                            var toPtr = (int*)&result;
                            var fromPtr = (int*)(basePtr + bufferIndex);

                            *toPtr++ = *fromPtr++;
                            *toPtr = *fromPtr;

                            value = result;
                        }
                    }
                    else
                    {
                        long val = 0;
                        var toPtr = (byte*)&val + 7;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 8;
                return true;
            }

            bufferIndex = bufferEnd;
            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_8_UInt64(out ulong value)
        {
            if (HasBufferData(8))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        if (ArchitectureInfo.Architecture_Supports_All_Unaligned_ReadWrites)
                        {
                            // We can read directly from the buffer, safe in the knowledge that any potential unaligned reads will work
                            value = *(ulong*)(basePtr + bufferIndex);
                        }
                        else
                        {
                            // We do an int-by-int read instead, into an address that we know is aligned
                            ulong result = 0;

                            var toPtr = (int*)&result;
                            var fromPtr = (int*)(basePtr + bufferIndex);

                            *toPtr++ = *fromPtr++;
                            *toPtr = *fromPtr;

                            value = result;
                        }
                    }
                    else
                    {
                        ulong val = 0;
                        var toPtr = (byte*)&val + 7;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 8;
                return true;
            }

            bufferIndex = bufferEnd;
            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_8_Float64(out double value)
        {
            if (HasBufferData(8))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        if (ArchitectureInfo.Architecture_Supports_All_Unaligned_ReadWrites)
                        {
                            // We can read directly from the buffer, safe in the knowledge that any potential unaligned reads will work
                            value = *(double*)(basePtr + bufferIndex);
                        }
                        else
                        {
                            // We do an int-by-int read instead, into an address that we know is aligned
                            double result = 0;

                            var toPtr = (int*)&result;
                            var fromPtr = (int*)(basePtr + bufferIndex);

                            *toPtr++ = *fromPtr++;
                            *toPtr = *fromPtr;

                            value = result;
                        }
                    }
                    else
                    {
                        double val = 0;
                        var toPtr = (byte*)&val + 7;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 8;
                return true;
            }

            bufferIndex = bufferEnd;
            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_16_Decimal(out decimal value)
        {
            if (HasBufferData(16))
            {
                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        if (ArchitectureInfo.Architecture_Supports_All_Unaligned_ReadWrites)
                        {
                            // We can read directly from the buffer, safe in the knowledge that any potential unaligned reads will work
                            value = *(decimal*)(basePtr + bufferIndex);
                        }
                        else
                        {
                            // We do an int-by-int read instead, into an address that we know is aligned
                            decimal result = 0;

                            var toPtr = (int*)&result;
                            var fromPtr = (int*)(basePtr + bufferIndex);

                            *toPtr++ = *fromPtr++;
                            *toPtr++ = *fromPtr++;
                            *toPtr++ = *fromPtr++;
                            *toPtr = *fromPtr;

                            value = result;
                        }
                    }
                    else
                    {
                        decimal val = 0;
                        var toPtr = (byte*)&val + 15;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 16;
                return true;
            }

            bufferIndex = bufferEnd;
            value = 0;
            return false;
        }

        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool UNSAFE_Read_16_Guid(out Guid value)
        {
            if (HasBufferData(16))
            {
                // First 10 bytes of a guid are always little endian
                // Last 6 bytes depend on architecture endianness
                // See http://stackoverflow.com/questions/10190817/guid-byte-order-in-net

                // TODO: Test if this actually works on big-endian architecture. Where the hell do we find that?

                fixed (byte* basePtr = buffer)
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        if (ArchitectureInfo.Architecture_Supports_All_Unaligned_ReadWrites)
                        {
                            // We can read directly from the buffer, safe in the knowledge that any potential unaligned reads will work
                            value = *(Guid*)(basePtr + bufferIndex);
                        }
                        else
                        {
                            // We do an int-by-int read instead, into an address that we know is aligned
                            var result = default(Guid);

                            var toPtr = (int*)&result;
                            var fromPtr = (int*)(basePtr + bufferIndex);

                            *toPtr++ = *fromPtr++;
                            *toPtr++ = *fromPtr++;
                            *toPtr++ = *fromPtr++;
                            *toPtr = *fromPtr;

                            value = result;
                        }
                    }
                    else
                    {
                        var val = default(Guid);
                        var toPtr = (byte*)&val;
                        var fromPtr = basePtr + bufferIndex;

                        *toPtr++ = *fromPtr++;
                        *toPtr++ = *fromPtr++;
                        *toPtr++ = *fromPtr++;
                        *toPtr++ = *fromPtr++;
                        *toPtr++ = *fromPtr++;
                        *toPtr++ = *fromPtr++;
                        *toPtr++ = *fromPtr++;
                        *toPtr++ = *fromPtr++;
                        *toPtr++ = *fromPtr++;
                        *toPtr = *fromPtr++;

                        toPtr += 6;

                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr-- = *fromPtr++;
                        *toPtr = *fromPtr;

                        value = val;
                    }
                }

                bufferIndex += 16;
                return true;
            }

            bufferIndex = bufferEnd;
            value = default;
            return false;
        }


        [MethodImpl((MethodImplOptions)0x100)] // Set aggressive inlining flag, for the runtimes that understand that
        private bool HasBufferData(int amount)
        {
            if (bufferEnd == 0)
            {
                ReadEntireStreamToBuffer();
            }

            return bufferIndex + amount <= bufferEnd;
        }

        private void ReadEntireStreamToBuffer()
        {
            bufferIndex = 0;

            if (Stream is MemoryStream)
                // We can do a small trick and just steal the memory stream's internal buffer
                // and totally avoid copying from the stream's internal buffer that way.
                //
                // This is pretty great, since most of the time we will be deserializing from
                // a memory stream.
            {
                try
                {
                    buffer = (Stream as MemoryStream).GetBuffer();
                    bufferEnd = (int)Stream.Length;
                    bufferIndex = (int)Stream.Position;
                    return;
                }
                catch (UnauthorizedAccessException)
                {
                    // Sometimes we're not actually allowed to get the internal buffer
                    // in that case, we can just copy from the stream as we normally do.
                }
            }

            buffer = internalBufferBackup;

            var remainder = (int)(Stream.Length - Stream.Position);

            if (buffer.Length >= remainder)
            {
                Stream.Read(buffer, 0, remainder);
            }
            else
            {
                buffer = new byte[remainder];
                Stream.Read(buffer, 0, remainder);

                if (remainder <= 1024 * 1024 * 10)
                    // We've made a larger buffer - might as well keep that, so long as it's not too ridiculously big (>10 MB)
                    // We don't want to be too much of a memory hog - at least there will usually only be one reader instance
                    // instantiated, ever.
                {
                    internalBufferBackup = buffer;
                }
            }

            bufferIndex = 0;
            bufferEnd = remainder;
        }

        private struct Struct256Bit
        {
            public decimal d1;
            public decimal d2;
        }
    }
}
