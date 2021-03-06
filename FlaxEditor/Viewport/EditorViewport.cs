// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using FlaxEditor.Viewport.Cameras;
using FlaxEditor.Viewport.Widgets;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Rendering;
using FlaxEngine.Utilities;

namespace FlaxEditor.Viewport
{
    /// <summary>
    /// Editor viewports base class.
    /// </summary>
    /// <seealso cref="FlaxEngine.GUI.RenderOutputControl" />
    public class EditorViewport : RenderOutputControl
    {
        // TODO: maybe cache view/projection matricies to reuse them

        /// <summary>
        /// Gathered input data.
        /// </summary>
        public struct Input
        {
            /// <summary>
            /// The is panning state.
            /// </summary>
            public bool IsPanning;

            /// <summary>
            /// The is rotating state.
            /// </summary>
            public bool IsRotating;

            /// <summary>
            /// The is moving state.
            /// </summary>
            public bool IsMoving;

            /// <summary>
            /// The is zooming state.
            /// </summary>
            public bool IsZooming;

            /// <summary>
            /// The is orbiting state.
            /// </summary>
            public bool IsOrbiting;

            /// <summary>
            /// The is control down flag.
            /// </summary>
            public bool IsControlDown;

            /// <summary>
            /// The is shift down flag.
            /// </summary>
            public bool IsShiftDown;

            /// <summary>
            /// The is alt down flag.
            /// </summary>
            public bool IsAltDown;

            /// <summary>
            /// The is mouse right down flag.
            /// </summary>
            public bool IsMouseRightDown;

            /// <summary>
            /// The is mouse middle down flag.
            /// </summary>
            public bool IsMouseMiddleDown;

            /// <summary>
            /// The is mouse left down flag.
            /// </summary>
            public bool IsMouseLeftDown;

            /// <summary>
            /// The mouse wheel delta.
            /// </summary>
            public float MouseWheelDelta;

            /// <summary>
            /// Gets a value indicating whether use is controlling mouse.
            /// </summary>
            public bool IsControllingMouse => IsMouseMiddleDown || IsMouseRightDown || (IsAltDown && IsMouseLeftDown);

            /// <summary>
            /// Gathers input from the specified window.
            /// </summary>
            /// <param name="window">The window.</param>
            public void Gather(FlaxEngine.Window window)
            {
                IsControlDown = window.GetKey(Keys.Control);
                IsShiftDown = window.GetKey(Keys.Shift);
                IsAltDown = window.GetKey(Keys.Alt);

                IsMouseRightDown = window.GetMouseButton(MouseButton.Right);
                IsMouseMiddleDown = window.GetMouseButton(MouseButton.Middle);
                IsMouseLeftDown = window.GetMouseButton(MouseButton.Left);
            }

            /// <summary>
            /// Clears the data.
            /// </summary>
            public void Clear()
            {
                IsControlDown = false;
                IsShiftDown = false;
                IsAltDown = false;

                IsMouseRightDown = false;
                IsMouseMiddleDown = false;
                IsMouseLeftDown = false;
            }
        }

        /// <summary>
        /// The FPS camera filtering frames count (how much frames we want to keep in the buffer to calculate the avg. delta currently hardcoded).
        /// </summary>
        public const int FpsCameraFilteringFrames = 3;

        /// <summary>
        /// The speed widget button.
        /// </summary>
        protected ViewportWidgetButton _speedWidget;


        private float _movementSpeed;
        private float _mouseAccelerationScale;
        private bool _useMouseFiltering;
        private bool _useMouseAcceleration;

        // Input

        private bool _isControllingMouse;
        protected Input _prevInput;
        protected Input _input;
        protected int _deltaFilteringStep;
        protected Vector2 _viewMousePos;
        protected Vector2 _mouseDeltaRight;
        protected Vector2 _mouseDeltaLeft;
        protected Vector2 _startPosRight;
        protected Vector2 _startPosLeft;
        protected Vector2 _mouseDeltaRightLast;
        protected Vector2[] _deltaFilteringBuffer = new Vector2[FpsCameraFilteringFrames];

        // Camera

        private ViewportCamera _camera;
        protected float _yaw;
        protected float _pitch;
        protected float _fieldOfView = 60.0f;
        protected float _nearPlane = 2.0f;
        protected float _farPlane = 10000.0f;

        /// <summary>
        /// Speed of the mouse.
        /// </summary>
        public float MouseSpeed = 1;

        /// <summary>
        /// Speed of the mouse wheel zooming.
        /// </summary>
        public float MouseWheelZoomSpeedFactor = 1;

        /// <summary>
        /// Gets or sets the camera movement speed.
        /// </summary>
        public float MovementSpeed
        {
            get => _movementSpeed;
            set
            {
                for (int i = 0; i < EditorViewportCameraSpeedValues.Length; i++)
                {
                    if (Math.Abs(value - EditorViewportCameraSpeedValues[i]) < 0.001f)
                    {
                        _movementSpeed = EditorViewportCameraSpeedValues[i];
                        if (_speedWidget != null)
                            _speedWidget.Text = _movementSpeed.ToString();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the mouse movement delta for the right button (user press and move).
        /// </summary>
        public Vector2 MouseDeltaRight => _mouseDeltaRight;

        /// <summary>
        /// Gets the mouse movement delta for the left button (user press and move).
        /// </summary>
        public Vector2 MouseDeltaLeft => _mouseDeltaLeft;

        /// <summary>
        /// Camera's pitch angle clamp range (in degrees).
        /// </summary>
        public Vector2 CamPitchAngles = new Vector2(-88, 88);

        /// <summary>
        /// Gets the view transform.
        /// </summary>
        public Transform ViewTransform => new Transform(ViewPosition, ViewOrientation);

        /// <summary>
        /// Gets or sets the view position.
        /// </summary>
        public Vector3 ViewPosition { get; set; }

        /// <summary>
        /// Gets or sets the view orientation.
        /// </summary>
        public Quaternion ViewOrientation
        {
            get => Quaternion.RotationYawPitchRoll(_yaw * Mathf.DegreesToRadians, _pitch * Mathf.DegreesToRadians, 0);
            set => EulerAngles = value.EulerAngles;
        }

        /// <summary>
        /// Gets or sets the view direction vector.
        /// </summary>
        public Vector3 ViewDirection
        {
            get => Vector3.Forward * ViewOrientation;
            set
            {
                Vector3 right = Vector3.Cross(value, Vector3.Up);
                Vector3 up = Vector3.Cross(right, value);
                ViewOrientation = Quaternion.LookRotation(value, up);
            }
        }

        /// <summary>
        /// Gets or sets the view ray (position and direction).
        /// </summary>
        public Ray ViewRay
        {
            get => new Ray(ViewPosition, ViewDirection);
            set
            {
                ViewPosition = value.Position;
                ViewDirection = value.Direction;
            }
        }

        /// <summary>
        /// Gets or sets the yaw angle (in degrees).
        /// </summary>
        public float Yaw
        {
            get => _yaw;
            set => _yaw = value;
        }

        /// <summary>
        /// Gets or sets the pitch angle (in degrees).
        /// </summary>
        public float Pitch
        {
            get => _pitch;
            set => _pitch = Mathf.Clamp(value, CamPitchAngles.X, CamPitchAngles.Y);
        }

        /// <summary>
        /// Gets or sets the absolute mouse position (normalized, not in pixels). Yaw is X, Pitch is Y.
        /// </summary>
        public Vector2 YawPitch
        {
            get => new Vector2(_yaw, _pitch);
            set
            {
                Yaw = value.X;
                Pitch = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the euler angles (pitch, yaw, roll).
        /// </summary>
        public Vector3 EulerAngles
        {
            get => new Vector3(_pitch, _yaw, 0);
            set
            {
                Pitch = value.X;
                Yaw = value.Y;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this viewport has loaded dependant assets.
        /// </summary>
        public virtual bool HasLoadedAssets => true;

        /// <summary>
        /// The 'View' widget button context menu.
        /// </summary>
        public ContextMenu ViewWidgetButtonMenu;

        /// <summary>
        /// Gets or sets the viewport camera controller.
        /// </summary>
        public ViewportCamera ViewportCamera
        {
            get => _camera;
            set
            {
                if (_camera != null)
                    _camera.Viewport = null;

                _camera = value;

                if (_camera != null)
                    _camera.Viewport = this;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorViewport"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="camera">The camera controller.</param>
        /// <param name="useWidgets">Enable/disable viewport widgets.</param>
        public EditorViewport(SceneRenderTask task, ViewportCamera camera, bool useWidgets)
        : base(task)
        {
            _movementSpeed = 1;
            _mouseAccelerationScale = 0.1f;
            _useMouseFiltering = false;
            _useMouseAcceleration = false;
            _camera = camera;
            if (_camera != null)
                _camera.Viewport = this;

            DockStyle = DockStyle.Fill;

            if (useWidgets)
            {
                // Camera speed widget
                var camSpeed = new ViewportWidgetsContainer(ViewportWidgetLocation.UpperRight);
                var camSpeedCM = new ContextMenu();
                var camSpeedButton = new ViewportWidgetButton("1", Editor.Instance.Icons.ArrowRightBorder16, camSpeedCM);
                camSpeedButton.Tag = this;
                camSpeedButton.TooltipText = "Camera speed scale";
                _speedWidget = camSpeedButton;
                for (int i = 0; i < EditorViewportCameraSpeedValues.Length; i++)
                {
                    var v = EditorViewportCameraSpeedValues[i];
                    var button = camSpeedCM.AddButton(v.ToString());
                    button.Tag = v;
                }
                camSpeedCM.ButtonClicked += (button) => MovementSpeed = (float)button.Tag;
                camSpeedCM.VisibleChanged += widgetCamSpeedShowHide;
                camSpeedButton.Parent = camSpeed;
                camSpeed.Parent = this;

                // View mode widget
                var viewMode = new ViewportWidgetsContainer(ViewportWidgetLocation.UpperLeft);
                ViewWidgetButtonMenu = new ContextMenu();
                var viewModeButton = new ViewportWidgetButton("View", Sprite.Invalid, ViewWidgetButtonMenu);
                viewModeButton.TooltipText = "View properties";
                viewModeButton.Parent = viewMode;
                viewMode.Parent = this;

                // Show FPS
                {
                    InitFpsCounter();
                    _showFpsButon = ViewWidgetButtonMenu.AddButton("Show FPS", () => ShowFpsCounter = !ShowFpsCounter);
                }

                // View Flags
                {
                    var viewFlags = ViewWidgetButtonMenu.AddChildMenu("View Flags").ContextMenu;
                    for (int i = 0; i < EditorViewportViewFlagsValues.Length; i++)
                    {
                        var v = EditorViewportViewFlagsValues[i];
                        var button = viewFlags.AddButton(v.Name);
                        button.Tag = v.Mode;
                    }
                    viewFlags.ButtonClicked += (button) => Task.Flags ^= (ViewFlags)button.Tag;
                    viewFlags.VisibleChanged += widgetViewFlagsShowHide;
                }

                // Debug View
                {
                    var debugView = ViewWidgetButtonMenu.AddChildMenu("Debug View").ContextMenu;
                    for (int i = 0; i < EditorViewportViewModeValues.Length; i++)
                    {
                        var v = EditorViewportViewModeValues[i];
                        var button = debugView.AddButton(v.Name);
                        button.Tag = v.Mode;
                    }
                    debugView.ButtonClicked += (button) => Task.Mode = (ViewMode)button.Tag;
                    debugView.VisibleChanged += widgetViewModeShowHide;
                }

                ViewWidgetButtonMenu.AddSeparator();

                // Field of View
                {
                    var fov = ViewWidgetButtonMenu.AddButton("Field Of View");
                    var fovValue = new FloatValueBox(1, 75, 2, 50.0f, 35.0f, 160.0f);
                    fovValue.Parent = fov;
                    fovValue.ValueChanged += () => _fieldOfView = fovValue.Value;
                    ViewWidgetButtonMenu.VisibleChanged += control => fovValue.Value = _fieldOfView;
                }

                // Far Plane
                {
                    var farPlane = ViewWidgetButtonMenu.AddButton("Far Plane");
                    var farPlaneValue = new FloatValueBox(1000, 75, 2, 50.0f, 10.0f, 100000.0f);
                    farPlaneValue.Parent = farPlane;
                    farPlaneValue.ValueChanged += () => _farPlane = farPlaneValue.Value;
                    ViewWidgetButtonMenu.VisibleChanged += control => farPlaneValue.Value = _farPlane;
                }

                // Brightness
                {
                    var brightness = ViewWidgetButtonMenu.AddButton("Brightness");
                    var brightnessValue = new FloatValueBox(1.0f, 75, 2, 50.0f, 0.001f, 10.0f, 0.001f);
                    brightnessValue.Parent = brightness;
                    brightnessValue.ValueChanged += () => Brightness = brightnessValue.Value;
                    ViewWidgetButtonMenu.VisibleChanged += control => brightnessValue.Value = Brightness;
                }
            }

            // Link for task event
            task.Begin += OnRenderBegin;
        }

        private void OnRenderBegin(SceneRenderTask task, GPUContext context)
        {
            CopyViewData(ref task.View);
        }

        #region FPS Counter

        private class FpsCounter : Control
        {
            public FpsCounter(float x, float y)
            : base(x, y, 64, 32)
            {
            }

            public override void Draw()
            {
                base.Draw();

                int fps = Time.FramesPerSecond;
                Color color = Color.Green;
                if (fps < 13)
                    color = Color.Red;
                else if (fps < 22)
                    color = Color.Yellow;
                string text = string.Format("FPS: {0}", fps);
                Render2D.DrawText(Style.Current.FontMedium, text, new Rectangle(Vector2.Zero, Size), color);
            }
        }

        private FpsCounter _fpsCounter;
        private ContextMenuButton _showFpsButon;

        /// <summary>
        /// Gets or sets a value indicating whether show or hide FPS counter.
        /// </summary>
        public bool ShowFpsCounter
        {
            get => _fpsCounter.Visible;
            set
            {
                _fpsCounter.Visible = value;
                _showFpsButon.Icon = value ? Style.Current.CheckBoxTick : Sprite.Invalid;
            }
        }

        private void InitFpsCounter()
        {
            _fpsCounter = new FpsCounter(10, ViewportWidgetsContainer.WidgetsHeight + 14);
            _fpsCounter.Visible = false;
            _fpsCounter.Parent = this;
        }

        #endregion

        /// <summary>
        /// Takes the screenshot of the current viewport.
        /// </summary>
        /// <param name="path">The output file path. Set null to use default value.</param>
        public void TakeScreenshot(string path = null)
        {
            Screenshot.Capture(Task, path);
        }

        /// <summary>
        /// Copies the render view data to <see cref="RenderView"/> structure.
        /// </summary>
        /// <param name="view">The view.</param>
        public void CopyViewData(ref RenderView view)
        {
            // Ceate matricies
            CreateProjectionMatrix(out view.Projection);
            CreateViewMatrix(out view.View);

            // Copy data
            view.Position = ViewPosition;
            view.Direction = ViewDirection;
            view.Near = _nearPlane;
            view.Far = _farPlane;
        }

        /// <summary>
        /// Gets the input state data.
        /// </summary>
        /// <param name="input">The input.</param>
        public void GetInput(out Input input)
        {
            input = _input;
        }

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="result">The result.</param>
        protected virtual void CreateProjectionMatrix(out Matrix result)
        {
            // Create projection matrix
            float aspect = Width / Height;
            Matrix.PerspectiveFov(_fieldOfView * Mathf.DegreesToRadians, aspect, _nearPlane, _farPlane, out result);
        }

        /// <summary>
        /// Creates the view matrix.
        /// </summary>
        /// <param name="result">The result.</param>
        protected virtual void CreateViewMatrix(out Matrix result)
        {
            // Create view matrix
            Vector3 position = ViewPosition;
            Vector3 direction = ViewDirection;
            Vector3 target = position + direction;
            Vector3 right = Vector3.Normalize(Vector3.Cross(Vector3.Up, direction));
            Vector3 up = Vector3.Normalize(Vector3.Cross(direction, right));
            Matrix.LookAt(ref position, ref target, ref up, out result);
        }

        /// <summary>
        /// Gets the mouse ray.
        /// </summary>
        public Ray MouseRay
        {
            get
            {
                if (IsMouseOver)
                    return ConvertMouseToRay(ref _viewMousePos);
                return new Ray(Vector3.Maximum, Vector3.Up);
            }
        }

        /// <summary>
        /// Converts the mouse position to the ray (in world space of the viewport).
        /// </summary>
        /// <param name="mousePosition">The mouse position.</param>
        /// <returns>The result ray.</returns>
        public Ray ConvertMouseToRay(ref Vector2 mousePosition)
        {
            // Prepare
            var viewport = new FlaxEngine.Viewport(0, 0, Width, Height);
            Matrix v, p, ivp;
            CreateProjectionMatrix(out p);
            CreateViewMatrix(out v);
            Matrix.Multiply(ref v, ref p, out ivp);
            ivp.Invert();

            // Create near and far points
            Vector3 nearPoint = new Vector3(mousePosition, 0.0f);
            Vector3 farPoint = new Vector3(mousePosition, 1.0f);
            viewport.Unproject(ref nearPoint, ref ivp, out nearPoint);
            viewport.Unproject(ref farPoint, ref ivp, out farPoint);

            // Create direction vector
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        /// <summary>
        /// Called when mouse control begins.
        /// </summary>
        /// <param name="win">The parent window.</param>
        protected virtual void OnControlMouseBegin(FlaxEngine.Window win)
        {
            // Hide cursor and start tracking mouse movement
            win.StartTrackingMouse(false);
            win.Cursor = CursorType.Hidden;

            // Center mouse position
            //_viewMousePos = Center;
            //win.MousePosition = PointToWindow(_viewMousePos);
        }

        /// <summary>
        /// Called when mouse control ends.
        /// </summary>
        /// <param name="win">The parent window.</param>
        protected virtual void OnControlMouseEnd(FlaxEngine.Window win)
        {
            // Restore cursor and stop tracking mouse movement
            win.Cursor = CursorType.Default;
            win.EndTrackingMouse();
        }

        /// <summary>
        /// Called when left mouse button goes down (on press).
        /// </summary>
        protected virtual void OnLeftMouseButtonDown()
        {
            _startPosLeft = _viewMousePos;
        }

        /// <summary>
        /// Called when left mouse button goes up (on release).
        /// </summary>
        protected virtual void OnLeftMouseButtonUp()
        {
        }

        /// <summary>
        /// Called when right mouse button goes down (on press).
        /// </summary>
        protected virtual void OnRightMouseButtonDown()
        {
            _startPosRight = _viewMousePos;
        }

        /// <summary>
        /// Called when right mouse button goes up (on release).
        /// </summary>
        protected virtual void OnRightMouseButtonUp()
        {
        }

        /// <summary>
        /// Updates the view.
        /// </summary>
        /// <param name="dt">The delta time (in seconds).</param>
        /// <param name="moveDelta">The move delta (scaled).</param>
        /// <param name="mouseDelta">The mouse delta (scaled).</param>
        /// <param name="centerMouse">True if center mouse after the update.</param>
        protected virtual void UpdateView(float dt, ref Vector3 moveDelta, ref Vector2 mouseDelta, out bool centerMouse)
        {
            centerMouse = true;
            _camera?.UpdateView(dt, ref moveDelta, ref mouseDelta, out centerMouse);
        }

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            _camera?.Update(deltaTime);

            // Get parent window
            var win = (WindowRootControl)Root;
            
            // Get current mouse position in the view
            _viewMousePos = PointFromWindow(win.MousePosition);

            // Update input
            {
                // Get input buttons and keys (skip if viewort has no focus or mouse is over a child control)
                _prevInput = _input;
                if (ContainsFocus && GetChildAt(_viewMousePos) == null)
                    _input.Gather(win.Window);
                else
                    _input.Clear();

                // Track controlling mouse state change
                bool wasControllingMouse = _prevInput.IsControllingMouse;
                _isControllingMouse = _input.IsControllingMouse;
                if (wasControllingMouse != _isControllingMouse)
                {
                    if (_isControllingMouse)
                        OnControlMouseBegin(win.Window);
                    else
                        OnControlMouseEnd(win.Window);
                }

                // Track mouse buttons state change
                if (!_prevInput.IsMouseLeftDown && _input.IsMouseLeftDown)
                    OnLeftMouseButtonDown();
                else if (_prevInput.IsMouseLeftDown && !_input.IsMouseLeftDown)
                    OnLeftMouseButtonUp();
                //
                if (!_prevInput.IsMouseRightDown && _input.IsMouseRightDown)
                    OnRightMouseButtonDown();
                else if (_prevInput.IsMouseRightDown && !_input.IsMouseRightDown)
                    OnRightMouseButtonUp();
            }

            // Check if update mouse
            Vector2 size = Size;
            if (_isControllingMouse)
            {
                // Gather input
                {
                    bool isAltDown = _input.IsAltDown;
                    bool lbDown = _input.IsMouseLeftDown;
                    bool mbDown = _input.IsMouseMiddleDown;
                    bool rbDown = _input.IsMouseRightDown;
                    bool wheelInUse = Math.Abs(_input.MouseWheelDelta) > Mathf.Epsilon;

                    _input.IsPanning = !isAltDown && mbDown && !rbDown;
                    _input.IsRotating = !isAltDown && !mbDown && rbDown;
                    _input.IsMoving = !isAltDown && mbDown && rbDown;
                    //_input.IsZooming = (isAltDown && !lbDown && !mbDown && rbDown) || wheelInUse;
                    _input.IsZooming = wheelInUse;
                    _input.IsOrbiting = isAltDown && lbDown && !mbDown && !rbDown;
                }

                // Get input movement
                Vector3 moveDelta = Vector3.Zero;
                if (win.GetKey(Keys.W))
                {
                    moveDelta += Vector3.Forward;
                }
                if (win.GetKey(Keys.S))
                {
                    moveDelta += Vector3.Backward;
                }
                if (win.GetKey(Keys.D))
                {
                    moveDelta += Vector3.Right;
                }
                if (win.GetKey(Keys.A))
                {
                    moveDelta += Vector3.Left;
                }
                if (win.GetKey(Keys.E))
                {
                    moveDelta += Vector3.Up;
                }
                if (win.GetKey(Keys.Q))
                {
                    moveDelta += Vector3.Down;
                }
                moveDelta.Normalize(); // normalize direction
                moveDelta *= _movementSpeed;

                // Speed up or speed down
                if (_input.IsShiftDown)
                    moveDelta *= 4.0f;
                if (_input.IsControlDown)
                    moveDelta *= 0.3f;

                // Calculate smooth mouse delta not dependant on viewport size
                Vector2 offset = _viewMousePos - _startPosRight;
                offset.X = offset.X > 0 ? Mathf.Floor(offset.X) : Mathf.Ceil(offset.X);
                offset.Y = offset.Y > 0 ? Mathf.Floor(offset.Y) : Mathf.Ceil(offset.Y);
                _mouseDeltaRight = offset / size;
                _mouseDeltaRight.Y *= size.Y / size.X;

                Vector2 mouseDelta = Vector2.Zero;
                if (_useMouseFiltering) // mouse filtering
                {
                    // update delta filtering buffer
                    _deltaFilteringBuffer[_deltaFilteringStep] = _mouseDeltaRight;
                    _deltaFilteringStep++;

                    // if the step is too far, zeroe
                    if (_deltaFilteringStep == FpsCameraFilteringFrames)
                        _deltaFilteringStep = 0;

                    // calculate filtered delta(avg)
                    for (int i = 0; i < FpsCameraFilteringFrames; i++)
                        mouseDelta += _deltaFilteringBuffer[i];

                    mouseDelta /= FpsCameraFilteringFrames;
                }
                else
                    mouseDelta = _mouseDeltaRight;

                if (_useMouseAcceleration) // mouse acceleration
                {
                    // accelerate the delta
                    var currentDelta = mouseDelta;
                    mouseDelta = mouseDelta + _mouseDeltaRightLast * _mouseAccelerationScale;
                    _mouseDeltaRightLast = currentDelta;
                }

                // Get clamped delta time (more stable during lags)
                var dt = Math.Min(Time.UnscaledDeltaTime, 1.0f);

                // Update
                moveDelta *= dt * (60.0f * 4.0f);
                mouseDelta *= 200.0f * MouseSpeed;
                bool centerMouse;
                UpdateView(dt, ref moveDelta, ref mouseDelta, out centerMouse);

                // Move mouse back to the root position
                if (centerMouse)
                {
                    Vector2 center = PointToWindow(_startPosRight);
                    win.MousePosition = center;
                }
            }
            else
            {
                _mouseDeltaRight = _mouseDeltaRightLast = Vector2.Zero;
            }
            if (_input.IsMouseLeftDown)
            {
                // Calculate smooth mouse delta not dependant on viewport size
                Vector2 offset = _viewMousePos - _startPosLeft;
                offset.X = offset.X > 0 ? Mathf.Floor(offset.X) : Mathf.Ceil(offset.X);
                offset.Y = offset.Y > 0 ? Mathf.Floor(offset.Y) : Mathf.Ceil(offset.Y);
                _mouseDeltaLeft = offset / size;
                _startPosLeft = _viewMousePos;
            }
            else
            {
                _mouseDeltaLeft = Vector2.Zero;
            }

            _input.MouseWheelDelta = 0;
        }

        /// <inheritdoc />
        public override bool OnMouseDown(Vector2 location, MouseButton buttons)
        {
            Focus();

            base.OnMouseDown(location, buttons);
            return true;
        }

        /// <inheritdoc />
        public override bool OnMouseWheel(Vector2 location, float delta)
        {
            _input.MouseWheelDelta += delta;

            return base.OnMouseWheel(location, delta);
        }

        /// <inheritdoc />
        public override void OnChildResized(Control control)
        {
            base.OnChildResized(control);

            PerformLayout();
        }

        /// <inheritdoc />
        protected override void PerformLayoutSelf()
        {
            base.PerformLayoutSelf();
            ViewportWidgetsContainer.ArrangeWidgets(this);
        }

        private float[] EditorViewportCameraSpeedValues =
        {
            0.1f,
            0.25f,
            0.5f,
            1.0f,
            2.0f,
            4.0f,
            6.0f,
            8.0f,
        };

        private struct ViewModeOptions
        {
            public ViewMode Mode;
            public string Name;

            public ViewModeOptions(ViewMode mode, string name)
            {
                Mode = mode;
                Name = name;
            }
        }

        private ViewModeOptions[] EditorViewportViewModeValues =
        {
            new ViewModeOptions(ViewMode.Default, "Default"),
            new ViewModeOptions(ViewMode.NoPostFx, "No PostFx"),
            new ViewModeOptions(ViewMode.LightBuffer, "Light Buffer"),
            new ViewModeOptions(ViewMode.Reflections, "Reflections Buffer"),
            new ViewModeOptions(ViewMode.Depth, "Depth Buffer"),
            new ViewModeOptions(ViewMode.Diffuse, "Diffuse"),
            new ViewModeOptions(ViewMode.Metalness, "Metalness"),
            new ViewModeOptions(ViewMode.Roughness, "Roughness"),
            new ViewModeOptions(ViewMode.Specular, "Specular"),
            new ViewModeOptions(ViewMode.SpecularColor, "Specular Color"),
            new ViewModeOptions(ViewMode.ShadingModel, "Shading Model"),
            new ViewModeOptions(ViewMode.Emissive, "Emissive Light"),
            new ViewModeOptions(ViewMode.Normals, "Normals"),
            new ViewModeOptions(ViewMode.AmbientOcclusion, "Ambient Occlusion"),
        };

        private void widgetCamSpeedShowHide(Control cm)
        {
            if (cm.Visible == false)
                return;

            var ccm = (ContextMenu)cm;
            foreach (var e in ccm.Items)
            {
                if (e is ContextMenuButton b)
                {
                    var v = (float)b.Tag;
                    b.Icon = Mathf.Abs(MovementSpeed - v) < 0.001f
                             ? Style.Current.CheckBoxTick
                             : Sprite.Invalid;
                }
            }
        }

        private void widgetViewModeShowHide(Control cm)
        {
            if (cm.Visible == false)
                return;

            var ccm = (ContextMenu)cm;
            foreach (var e in ccm.Items)
            {
                if (e is ContextMenuButton b)
                {
                    var v = (ViewMode)b.Tag;
                    b.Icon = Task.Mode == v
                             ? Style.Current.CheckBoxTick
                             : Sprite.Invalid;
                }
            }
        }

        private struct ViewFlagOptions
        {
            public ViewFlags Mode;
            public string Name;

            public ViewFlagOptions(ViewFlags mode, string name)
            {
                Mode = mode;
                Name = name;
            }
        }

        private ViewFlagOptions[] EditorViewportViewFlagsValues =
        {
            new ViewFlagOptions(ViewFlags.AntiAliasing, "Anti Aliasing"),
            new ViewFlagOptions(ViewFlags.Shadows, "Shadows"),
            new ViewFlagOptions(ViewFlags.DynamicActors, "Dynamic Actors"),
            new ViewFlagOptions(ViewFlags.EditorSprites, "Editor Sprites"),
            new ViewFlagOptions(ViewFlags.Reflections, "Reflectons"),
            new ViewFlagOptions(ViewFlags.SSR, "Screen Space Reflections"),
            new ViewFlagOptions(ViewFlags.AO, "Ambient Occlusion"),
            new ViewFlagOptions(ViewFlags.GI, "Global Illumination"),
            new ViewFlagOptions(ViewFlags.DirectionalLights, "Directional Lights"),
            new ViewFlagOptions(ViewFlags.PointLights, "Point Lights"),
            new ViewFlagOptions(ViewFlags.SpotLights, "Spot Lights"),
            new ViewFlagOptions(ViewFlags.SkyLights, "Sky Lights"),
            new ViewFlagOptions(ViewFlags.Fog, "Fog"),
            new ViewFlagOptions(ViewFlags.SpecularLight, "Specular Light"),
            new ViewFlagOptions(ViewFlags.Decals, "Decals"),
            new ViewFlagOptions(ViewFlags.CustomPostProcess, "Custom Post Process"),
            new ViewFlagOptions(ViewFlags.Bloom, "Bloom"),
            new ViewFlagOptions(ViewFlags.ToneMapping, "Tone Mapping"),
            new ViewFlagOptions(ViewFlags.EyeAdaptation, "Eye Adaptaion"),
            new ViewFlagOptions(ViewFlags.CameraArtifacts, "Camera Artifacts"),
            new ViewFlagOptions(ViewFlags.LensFlares, "Lens Flares"),
            new ViewFlagOptions(ViewFlags.DepthOfField, "Depth of Field"),
            new ViewFlagOptions(ViewFlags.PhysicsDebug, "Physics Debug"),
        };

        private void widgetViewFlagsShowHide(Control cm)
        {
            if (cm.Visible == false)
                return;

            var ccm = (ContextMenu)cm;
            foreach (var e in ccm.Items)
            {
                if (e is ContextMenuButton b)
                {
                    var v = (ViewFlags)b.Tag;
                    b.Icon = (Task.Flags & v) != 0
                             ? Style.Current.CheckBoxTick
                             : Sprite.Invalid;
                }
            }
        }
    }
}
