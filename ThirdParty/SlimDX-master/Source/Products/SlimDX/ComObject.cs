﻿// Copyright (c) 2007-2011 SlimDX Group
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SlimDX
{
    /// <summary>
    /// A COM object.
    /// </summary>
    public abstract class ComObject
    {
        #region Interface

		protected ComObject(IntPtr nativePointer)
        {
            NativePointer = nativePointer;

            if (ReferenceTracker.TrackReferences)
                ReferenceTracker.Constructed(NativePointer, GetType());
        }

        public IntPtr NativePointer
        {
            get;
            private set;
        }

        /// <summary>
        /// Attempts to retrieve a reference to an interface of an object.
        /// </summary>
        /// <typeparam name="T">The interface to retrieve.</typeparam>
        /// <returns>
        /// A reference to an interface, if the requested interface is supported by the object. Otherwise null.
        /// </returns>
        public T QueryInterface<T>() where T : ComObject
        {
            IntPtr resultingNativeInterface;
            var guid = typeof(T).GUID;

            var resultCode = Marshal.QueryInterface(NativePointer, ref guid, out resultingNativeInterface);
            if (resultCode != 0)
                return null;

            var constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new[] { typeof(IntPtr) }, null);
            if (constructor == null)
            {
                return null;
            }

            return (T)constructor.Invoke(new object[] { resultingNativeInterface });
        }

        /// <summary>
        /// Increments the reference count for an object. 
        /// </summary>
        public int AddReference()
        {
            if (ReferenceTracker.TrackReferences)
                ReferenceTracker.ReferenceIncremented(NativePointer, GetType());

            return Marshal.AddRef(NativePointer);
        }

        /// <summary>
        /// Decrements the reference count for an object.
        /// </summary>
        public int ReleaseReference()
        {
            if (ReferenceTracker.TrackReferences)
                ReferenceTracker.ReferenceDecremented(NativePointer, GetType());

            return Marshal.Release(NativePointer);
        }

        #endregion
    }
}
