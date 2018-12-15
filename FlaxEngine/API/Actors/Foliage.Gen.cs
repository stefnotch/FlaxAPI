// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
    /// <summary>
    /// Represents a foliage actor that contains a set of instanced meshes.
    /// </summary>
    [Serializable]
    public sealed partial class Foliage : Actor
    {
        /// <summary>
        /// Creates new <see cref="Foliage"/> object.
        /// </summary>
        private Foliage() : base()
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="Foliage"/> object.
        /// </summary>
        /// <returns>Created object.</returns>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static Foliage New()
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            return Internal_Create(typeof(Foliage)) as Foliage;
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
#endif

        #endregion
    }
}
