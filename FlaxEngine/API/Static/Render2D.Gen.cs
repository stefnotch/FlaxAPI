// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.
// This code was generated by a tool. Changes to this file may cause
// incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Runtime.CompilerServices;

namespace FlaxEngine
{
    /// <summary>
    /// The interface to render fast two dimensional graphics.
    /// </summary>
    public static partial class Render2D
    {
        /// <summary>
        /// Pushes 2D transformation matrix on the stack.
        /// </summary>
        /// <param name="transform">The transformation.</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void PushTransform(Matrix3x3 transform)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_PushTransform(ref transform);
#endif
        }

        /// <summary>
        /// Pops transformation matrix from the stack.
        /// </summary>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void PopTransform()
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_PopTransform();
#endif
        }

        /// <summary>
        /// Push clipping rectangle mask
        /// </summary>
        /// <param name="clipRect">Axis aligned clipping mask rectangle</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void PushClip(Rectangle clipRect)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_PushClip(ref clipRect);
#endif
        }

        /// <summary>
        /// Pop clipping rectangle mask
        /// </summary>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void PopClip()
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_PopClip();
#endif
        }

        /// <summary>
        /// Draw text
        /// </summary>
        /// <param name="font">Font to use</param>
        /// <param name="text">Text to render</param>
        /// <param name="layoutRect">The size and position of the area in which the text is drawn</param>
        /// <param name="color">Text color</param>
        /// <param name="horizontalAlignment">Horizontal alignment of the text in a layout rectangle</param>
        /// <param name="verticalAlignment">Vertical alignment of the text in a layout rectangle</param>
        /// <param name="textWrapping">Describes how wrap text inside a layout rectangle</param>
        /// <param name="baseLinesGapScale">Scale for distance one baseline from another. Default is 1.</param>
        /// <param name="scale">Text drawing scale. Default is 1.</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawText(Font font, string text, Rectangle layoutRect, Color color, TextAlignment horizontalAlignment = TextAlignment.Near, TextAlignment verticalAlignment = TextAlignment.Near, TextWrapping textWrapping = TextWrapping.NoWrap, float baseLinesGapScale = 1.0f, float scale = 1.0f)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawText1(Object.GetUnmanagedPtr(font), text, ref layoutRect, ref color, horizontalAlignment, verticalAlignment, textWrapping, baseLinesGapScale, scale);
#endif
        }

        /// <summary>
        /// Draws text using a custom material shader. Given material must have GUI domain and a public parameter named Font (texture parameter used for a font atlas sampling).
        /// </summary>
        /// <param name="font">Font to use</param>
        /// <param name="customMaterial">Custom material for font characters rendering. It must contain texture parameter named Font used to sample font texture.</param>
        /// <param name="text">Text to render</param>
        /// <param name="layoutRect">The size and position of the area in which the text is drawn</param>
        /// <param name="color">Text color</param>
        /// <param name="horizontalAlignment">Horizontal alignment of the text in a layout rectangle</param>
        /// <param name="verticalAlignment">Vertical alignment of the text in a layout rectangle</param>
        /// <param name="textWrapping">Describes how wrap text inside a layout rectangle</param>
        /// <param name="baseLinesGapScale">Scale for distance one baseline from another. Default is 1.</param>
        /// <param name="scale">Text drawing scale. Default is 1.</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawText(Font font, MaterialBase customMaterial, string text, Rectangle layoutRect, Color color, TextAlignment horizontalAlignment = TextAlignment.Near, TextAlignment verticalAlignment = TextAlignment.Near, TextWrapping textWrapping = TextWrapping.NoWrap, float baseLinesGapScale = 1.0f, float scale = 1.0f)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawText2(Object.GetUnmanagedPtr(font), Object.GetUnmanagedPtr(customMaterial), text, ref layoutRect, ref color, horizontalAlignment, verticalAlignment, textWrapping, baseLinesGapScale, scale);
#endif
        }

        /// <summary>
        /// Fill rectangle area
        /// </summary>
        /// <param name="rect">Rectangle to fill</param>
        /// <param name="color">Color to use</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void FillRectangle(Rectangle rect, Color color)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_FillRectangle1(ref rect, ref color);
#endif
        }

        /// <summary>
        /// Fill rectangle area
        /// </summary>
        /// <param name="rect">Rectangle to fill</param>
        /// <param name="color0">Color to use for upper left vertex</param>
        /// <param name="color1">Color to use for upper right vertex</param>
        /// <param name="color2">Color to use for bottom right vertex</param>
        /// <param name="color3">Color to use for bottom left vertex</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void FillRectangle(Rectangle rect, Color color0, Color color1, Color color2, Color color3)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_FillRectangle2(ref rect, ref color0, ref color1, ref color2, ref color3);
#endif
        }

        /// <summary>
        /// Draw rectangle borders
        /// </summary>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="color">Color to use</param>
        /// <param name="thickness">Lines thickness (in pixels)</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawRectangle(Rectangle rect, Color color, float thickness = 1.0f)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawRectangle(ref rect, ref color, thickness);
#endif
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="rt">Render target to draw</param>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="color">Color to use</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawRenderTarget(Rendering.RenderTarget rt, Rectangle rect, Color color)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawRenderTarget(Object.GetUnmanagedPtr(rt), ref rect, ref color);
#endif
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="t">Texture to draw</param>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="color">Color to use</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawTexture(Texture t, Rectangle rect, Color color)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawTexture1(Object.GetUnmanagedPtr(t), ref rect, ref color);
#endif
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="t">Texture to draw</param>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="color">Color to use</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawTexture(SpriteAtlas t, Rectangle rect, Color color)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawTexture2(Object.GetUnmanagedPtr(t), ref rect, ref color);
#endif
        }

        /// <summary>
        /// Draw texture (uses point sampler)
        /// </summary>
        /// <param name="t">Texture to draw</param>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="color">Color to use</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawTexturePoint(Texture t, Rectangle rect, Color color)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawTexture3(Object.GetUnmanagedPtr(t), ref rect, ref color);
#endif
        }

        /// <summary>
        /// Draw texture (uses point sampler)
        /// </summary>
        /// <param name="t">Texture to draw</param>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="color">Color to use</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawTexturePoint(SpriteAtlas t, Rectangle rect, Color color)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawTexture4(Object.GetUnmanagedPtr(t), ref rect, ref color);
#endif
        }

        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="p1">Start point</param>
        /// <param name="p2">End point</param>
        /// <param name="color">Color to use</param>
        /// <param name="thickness">Lines thickness (in pixels)</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawLine(Vector2 p1, Vector2 p2, Color color, float thickness = 1.0f)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawLine(ref p1, ref p2, ref color, thickness);
#endif
        }

        /// <summary>
        /// Draw Bezier curve
        /// </summary>
        /// <param name="p1">Start point</param>
        /// <param name="p2">First control point</param>
        /// <param name="p3">Second control point</param>
        /// <param name="p4">End point</param>
        /// <param name="color">Color to use</param>
        /// <param name="thickness">Lines thickness (in pixels)</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawBezier(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color, float thickness = 1.0f)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawBezier(ref p1, ref p2, ref p3, ref p4, ref color, thickness);
#endif
        }

        /// <summary>
        /// Draws the GUI material in the 2D.
        /// </summary>
        /// <param name="material">Material to render. Must be a GUI material type.</param>
        /// <param name="rect">The target rectangle to draw.</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawMaterial(MaterialBase material, Rectangle rect)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawMaterial1(Object.GetUnmanagedPtr(material), ref rect);
#endif
        }

        /// <summary>
        /// Draws the GUI material in the 2D.
        /// </summary>
        /// <param name="material">Material to render. Must be a GUI material type.</param>
        /// <param name="rect">The target rectangle to draw.</param>
        /// <param name="color">Color to use</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawMaterial(MaterialBase material, Rectangle rect, Color color)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawMaterial2(Object.GetUnmanagedPtr(material), ref rect, ref color);
#endif
        }

        /// <summary>
        /// Draws the Gaussian-blur rectangle in the 2D that blurs the background.
        /// </summary>
        /// <param name="rect">The target rectangle to draw (blurs its background).</param>
        /// <param name="blurStrength">The blur strength defines how blurry the background is. Larger numbers increase blur, resulting in a larger runtime cost on the GPU.</param>
#if UNIT_TEST_COMPILANT
        [Obsolete("Unit tests, don't support methods calls.")]
#endif
        [UnmanagedCall]
        public static void DrawBlur(Rectangle rect, float blurStrength)
        {
#if UNIT_TEST_COMPILANT
            throw new NotImplementedException("Unit tests, don't support methods calls. Only properties can be get or set.");
#else
            Internal_DrawBlur(ref rect, blurStrength);
#endif
        }

        #region Internal Calls

#if !UNIT_TEST_COMPILANT
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_PushTransform(ref Matrix3x3 transform);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_PopTransform();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_PushClip(ref Rectangle clipRect);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_PopClip();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawText1(IntPtr font, string text, ref Rectangle layoutRect, ref Color color, TextAlignment horizontalAlignment, TextAlignment verticalAlignment, TextWrapping textWrapping, float baseLinesGapScale, float scale);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawText2(IntPtr font, IntPtr customMaterial, string text, ref Rectangle layoutRect, ref Color color, TextAlignment horizontalAlignment, TextAlignment verticalAlignment, TextWrapping textWrapping, float baseLinesGapScale, float scale);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_FillRectangle1(ref Rectangle rect, ref Color color);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_FillRectangle2(ref Rectangle rect, ref Color color0, ref Color color1, ref Color color2, ref Color color3);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawRectangle(ref Rectangle rect, ref Color color, float thickness);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawRenderTarget(IntPtr rt, ref Rectangle rect, ref Color color);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawTexture1(IntPtr t, ref Rectangle rect, ref Color color);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawTexture2(IntPtr t, ref Rectangle rect, ref Color color);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawTexture3(IntPtr t, ref Rectangle rect, ref Color color);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawTexture4(IntPtr t, ref Rectangle rect, ref Color color);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawLine(ref Vector2 p1, ref Vector2 p2, ref Color color, float thickness);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawBezier(ref Vector2 p1, ref Vector2 p2, ref Vector2 p3, ref Vector2 p4, ref Color color, float thickness);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawMaterial1(IntPtr material, ref Rectangle rect);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawMaterial2(IntPtr material, ref Rectangle rect, ref Color color);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_DrawBlur(ref Rectangle rect, float blurStrength);
#endif

        #endregion
    }
}
