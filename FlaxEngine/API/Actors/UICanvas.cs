// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System.Globalization;
using System.IO;
using System.Text;
using FlaxEngine.GUI;
using FlaxEngine.Rendering;
using Newtonsoft.Json;

namespace FlaxEngine
{
    /// <summary>
    /// The canvas rendering modes.
    /// </summary>
    public enum CanvasRenderMode
    {
        /// <summary>
        /// The screen space rendering mode that places UI elements on the screen rendered on top of the scene. If the screen is resized or changes resolution, the Canvas will automatically change size to match this.
        /// </summary>
        ScreenSpace = 0,

        /// <summary>
        /// The camera space rendering mode that places Canvas in a given distance in front of a specified Camera. The UI elements are rendered by this camera, which means that the Camera settings affect the appearance of the UI. If the Camera is set to Perspective, the UI elements will be rendered with perspective, and the amount of perspective distortion can be controlled by the Camera Field of View. If the screen is resized, changes resolution, or the camera frustum changes, the Canvas will automatically change size to match as well.
        /// </summary>
        CameraSpace = 1,

        /// <summary>
        /// The world space rendering mode that places Canvas as any other object in the scene. The size of the Canvas can be set manually using its Rect Transform, and UI elements will render in front of or behind other objects in the scene based on 3D placement. This is useful for UIs that are meant to be a part of the world. This is also known as a "diegetic interface".
        /// </summary>
        WorldSpace = 2,
    }

    /// <summary>
    /// PostFx used to render the <see cref="UICanvas"/>. Used when render mode is <see cref="CanvasRenderMode.CameraSpace"/> or <see cref="CanvasRenderMode.WorldSpace"/>.
    /// </summary>
    /// <seealso cref="FlaxEngine.Rendering.PostProcessEffect" />
    public sealed class CanvasRenderer : PostProcessEffect
    {
        /// <summary>
        /// The canvas to render.
        /// </summary>
        public UICanvas Canvas;

        /// <inheritdoc />
        public override void Render(GPUContext context, SceneRenderTask task, RenderTarget input, RenderTarget output)
        {
            // TODO: apply frustum culling to skip rendering if canvas is not in a viewport

            // TODO: support additive postFx to prevent frame copy
            context.Draw(output, input);
            
            // Calculate rendering matrix (world*view*projection)
            Matrix viewProjection;
            Matrix world;
            Canvas.GetWorldMatrix(out world);
            Matrix view;
            Matrix.Multiply(ref world, ref task.View.View, out view);
            Matrix.Multiply(ref view, ref task.View.Projection, out viewProjection);

            // Pick a depth buffer
            RenderTarget depthBuffer = Canvas.IgnoreDepth ? null : task.Buffers.DepthBuffer;

            // Render GUI in 3D
            Render2D.CallDrawing(Canvas.GUI, context, output, depthBuffer, ref viewProjection);
        }
    }

    public sealed partial class UICanvas
    {
        private CanvasRenderMode _renderMode;
        private readonly CanvasRootControl _guiRoot;
        private CanvasRenderer _renderer;
        private bool _isLoading;

        /// <summary>
        /// Gets or sets the canvas rendering mode.
        /// </summary>
        [EditorOrder(10), EditorDisplay("Canvas"), Tooltip("Canvas rendering mode.")]
        public CanvasRenderMode RenderMode
        {
            get => _renderMode;
            set
            {
                if (_renderMode != value)
                {
                    _renderMode = value;

                    Setup();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether canvas can receive the input events.
        /// </summary>
        [EditorOrder(15), EditorDisplay("Canvas"), Tooltip("If checked, canvas can receive the input events.")]
        public bool ReceivesEvents { get; set; } = true;

        private bool Editor_Is3D => _renderMode != CanvasRenderMode.ScreenSpace;

        private bool Editor_IsCameraSpace => _renderMode == CanvasRenderMode.CameraSpace;

        /// <summary>
        /// Gets or sets the size of the canvas. Used only in <see cref="CanvasRenderMode.CameraSpace"/> or <see cref="CanvasRenderMode.WorldSpace"/>.
        /// </summary>
        [EditorOrder(20), EditorDisplay("Canvas"), VisibleIf(nameof(Editor_Is3D)), Tooltip("Canvas size.")]
        public Vector2 Size
        {
            get => _guiRoot.Size;
            set
            {
                if (_renderMode != CanvasRenderMode.ScreenSpace || _isLoading)
                {
                    _guiRoot.Size = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ignore scene depth when rendering the GUI (scene objects won't cover the interface).
        /// </summary>
        [EditorOrder(30), EditorDisplay("Canvas"), VisibleIf(nameof(Editor_Is3D)), Tooltip("If checked, scene depth will be ignored when rendering the GUI (scene objects won't cover the interface).")]
        public bool IgnoreDepth { get; set; } = false;

        /// <summary>
        /// Gets or sets the camera used as a input events source. Canvas will use handle mouse raycasting for this camera to provide GUI system with events.
        /// </summary>
        [EditorOrder(40), EditorDisplay("Canvas"), VisibleIf(nameof(Editor_Is3D)), Tooltip("Camera used as a input events source. Canvas will use handle mouse raycasting for this camera to provide GUI system with events.")]
        public Camera EventCamera { get; set; }

        /// <summary>
        /// Gets or sets the camera used to place the GUI when render mode is set to <see cref="CanvasRenderMode.CameraSpace"/>.
        /// </summary>
        [EditorOrder(50), EditorDisplay("Canvas"), VisibleIf(nameof(Editor_IsCameraSpace)), Tooltip("Camera used to place the GUI when RenderMode is set to CameraSpace")]
        public Camera RenderCamera { get; set; }

        /// <summary>
        /// Gets or sets the distance from the <see cref="RenderCamera"/> to place the plane with GUI. Use negative value to move UI behind the camera, positive to move it in front of it.
        /// </summary>
        [EditorOrder(60), EditorDisplay("Canvas"), VisibleIf(nameof(Editor_IsCameraSpace)), Tooltip("Distance from the RenderCamera to place the plane with GUI. Use negative value to move UI behind the camera, positive to move it in front of it.")]
        public float Distance { get; set; } = 500;

        /// <summary>
        /// Gets the canvas GUI root control.
        /// </summary>
        public CanvasRootControl GUI => _guiRoot;

        private UICanvas()
        {
            _guiRoot = new CanvasRootControl(this);
            _guiRoot.IsLayoutLocked = false;
        }

        /// <summary>
        /// Gets the world matrix used to transform the GUI from the local space to the world space. Handles canvas rendering mode
        /// </summary>
        /// <param name="world">The world.</param>
        public void GetWorldMatrix(out Matrix world)
        {
            if (_renderMode == CanvasRenderMode.WorldSpace)
            {
                // In 3D world
                GetLocalToWorldMatrix(out world);
            }
            else if (_renderMode == CanvasRenderMode.CameraSpace && RenderCamera)
            {
                // Center viewport (and flip)
                Matrix tmp1, tmp2;
                var viewport = RenderCamera.Viewport;
                Matrix.Translation(viewport.Width / -2.0f, viewport.Height / -2.0f, 0, out world);
                Matrix.RotationX(Mathf.Pi, out tmp2);
                Matrix.Multiply(ref world, ref tmp2, out tmp1);
                tmp1 = world;

                // In front of the camera
                var viewPos = RenderCamera.Position;
                var viewRot = RenderCamera.Orientation;
                var viewUp = Vector3.Up * viewRot;
                var viewForward = Vector3.Forward * viewRot;
                var pos = viewPos + viewForward * Distance;
                Matrix.BillboardLH(ref pos, ref viewPos, ref viewUp, ref viewForward, out tmp2);

                Matrix.Multiply(ref tmp1, ref tmp2, out world);
            }
            else
            {
                // Direct projection
                world = Matrix.Identity;
            }
        }

        private void Setup()
        {
            if (_isLoading)
                return;

            switch (_renderMode)
            {
            case CanvasRenderMode.ScreenSpace:
            {
                // Link to the game UI and fill the area
                _guiRoot.DockStyle = DockStyle.Fill;
                _guiRoot.Parent = RootControl.GameRoot;
                if (_renderer)
                {
                    SceneRenderTask.GlobalCustomPostFx.Remove(_renderer);
                    Destroy(_renderer);
                    _renderer = null;
                }
                break;
            }
            case CanvasRenderMode.CameraSpace:
            case CanvasRenderMode.WorldSpace:
            {
                // Render canvas manually
                _guiRoot.DockStyle = DockStyle.None;
                _guiRoot.Parent = null;
                _guiRoot.PerformLayout();
                if (_renderer == null)
                {
                    _renderer = New<CanvasRenderer>();
                    _renderer.Canvas = this;
                    SceneRenderTask.GlobalCustomPostFx.Add(_renderer);
                }
                break;
            }
            }
        }

        private void Cleanup()
        {
            _guiRoot.Parent = null;
            if (_renderer)
            {
                SceneRenderTask.GlobalCustomPostFx.Remove(_renderer);
                Destroy(_renderer);
                _renderer = null;
            }
        }

        internal string Serialize()
        {
            StringBuilder sb = new StringBuilder(256);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.IndentChar = '\t';
                jsonWriter.Indentation = 1;
                jsonWriter.Formatting = Formatting.Indented;

                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName("RenderMode");
                jsonWriter.WriteValue(_renderMode);

                jsonWriter.WritePropertyName("ReceivesEvents");
                jsonWriter.WriteValue(ReceivesEvents);

                jsonWriter.WritePropertyName("IgnoreDepth");
                jsonWriter.WriteValue(IgnoreDepth);

                jsonWriter.WritePropertyName("EventCamera");
                jsonWriter.WriteValue(Json.JsonSerializer.GetStringID(EventCamera));

                jsonWriter.WritePropertyName("RenderCamera");
                jsonWriter.WriteValue(Json.JsonSerializer.GetStringID(RenderCamera));

                jsonWriter.WritePropertyName("Distance");
                jsonWriter.WriteValue(Distance);
                
                jsonWriter.WritePropertyName("Size");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("X");
                jsonWriter.WriteValue(Size.X);
                jsonWriter.WritePropertyName("Y");
                jsonWriter.WriteValue(Size.Y);
                jsonWriter.WriteEndObject();

                jsonWriter.WriteEndObject();
            }

            return sw.ToString();
        }

        internal void Deserialize(string json)
        {
            _isLoading = true;
            Json.JsonSerializer.Deserialize(this, json);
            _isLoading = false;
        }

        internal void PostDeserialize()
        {
            Setup();
        }

        internal void ActiveInTreeChanged()
        {
            bool isActiveInHierarchy = IsActiveInHierarchy;
            _guiRoot.Enabled = isActiveInHierarchy;
            _guiRoot.Visible = isActiveInHierarchy;
            if (_renderer)
                _renderer.Enabled = isActiveInHierarchy;
        }

        internal void EndPlay()
        {
            Cleanup();
        }
    }
}
