// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Runtime.CompilerServices;

namespace FlaxEditor
{
    /// <summary>
    /// Game building service. Processes project files and outputs builded game for a target platform.
    /// </summary>
    public static partial class GameCooker
    {
        /// <summary>
        /// Determines whether game building is running.
        /// </summary>
        [UnmanagedCall]
        public static bool IsRunning
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_IsRunning(); }
#endif
        }

        /// <summary>
        /// Determines whether building cancel has been requested.
        /// </summary>
        [UnmanagedCall]
        public static bool IsCancelRequested
        {
#if UNIT_TEST_COMPILANT
            get; set;
#else
            get { return Internal_IsCancelRequested(); }
#endif
        }

        /// <summary>
        /// Sends a cancel event to the game building service.
        /// </summary>
        /// <param name="waitForEnd">If set to <c>true</c> wait for the stopped building end.</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void Cancel(bool waitForEnd = false)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_Cancel(waitForEnd);
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Internal_IsRunning();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Internal_IsCancelRequested();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_Cancel(bool waitForEnd);
#endif

        #endregion
    }
}
