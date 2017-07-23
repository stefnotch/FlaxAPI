////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
	/// <summary>
	/// Texture asset contains an image that is usually stored on a GPU and is used during rendering graphics.
	/// </summary>
	public partial class Texture : BinaryAsset
	{
		/// <summary>
		/// Creates new <see cref="Texture"/> object.
		/// </summary>
		private Texture() : base()
		{
		}

		/// <summary>
		/// Gets the total width of the texture. Actual resident size may be different due to dynamic content streaming. Returns 0 if texture is not loaded.
		/// </summary>
		[UnmanagedCall]
		public int Width
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetWidth(unmanagedPtr); }
#endif
		}

		/// <summary>
		/// Gets the total height of the texture. Actual resident size may be different due to dynamic content streaming. Returns 0 if texture is not loaded.
		/// </summary>
		[UnmanagedCall]
		public int Height
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetHeight(unmanagedPtr); }
#endif
		}

		/// <summary>
		/// Gets the total size of the texture. Actual resident size may be different due to dynamic content streaming.
		/// </summary>
		[UnmanagedCall]
		public Vector2 Size
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { Vector2 resultAsRef; Internal_GetSize(unmanagedPtr, out resultAsRef); return resultAsRef; }
#endif
		}

		/// <summary>
		/// Gets the total array size of the texture.
		/// </summary>
		[UnmanagedCall]
		public int ArraySize
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetArraySize(unmanagedPtr); }
#endif
		}

		/// <summary>
		/// Gets the total mip levels count of the texture. Actual resident mipmaps count may be different due to dynamic content streaming.
		/// </summary>
		[UnmanagedCall]
		public int MipLevels
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetMipLevels(unmanagedPtr); }
#endif
		}

		/// <summary>
		/// Gets the current mip levels count of the texture that are on GPU ready to use.
		/// </summary>
		[UnmanagedCall]
		public int ResidentMipLevels
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetResidentMipLevels(unmanagedPtr); }
#endif
		}

		/// <summary>
		/// Returns true if texture is a normal map, otherwise false.
		/// </summary>
		[UnmanagedCall]
		public bool IsNormalMap
		{
#if UNIT_TEST_COMPILANT
			get; set;
#else
			get { return Internal_GetIsNormalMap(unmanagedPtr); }
#endif
		}

#region Internal Calls
#if !UNIT_TEST_COMPILANT
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetWidth(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetHeight(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_GetSize(IntPtr obj, out Vector2 resultAsRef);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetArraySize(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetMipLevels(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int Internal_GetResidentMipLevels(IntPtr obj);
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_GetIsNormalMap(IntPtr obj);
#endif
#endregion
	}
}

