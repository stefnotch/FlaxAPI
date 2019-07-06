// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.

using System;
using System.Xml;
using FlaxEditor.Content;
using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.GUI;
using FlaxEditor.GUI;
using FlaxEditor.GUI.ContextMenu;
using FlaxEditor.GUI.Drag;
using FlaxEditor.Surface;
using FlaxEditor.Viewport.Cameras;
using FlaxEditor.Viewport.Previews;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Rendering;

// ReSharper disable MemberCanBePrivate.Local

namespace FlaxEditor.Windows.Assets
{
    /// <summary>
    /// Animation Graph window allows to view and edit <see cref="AnimationGraph"/> asset.
    /// Note: it uses ClonedAssetEditorWindowBase which is creating cloned asset to edit/preview.
    /// </summary>
    /// <seealso cref="AnimationGraph" />
    /// <seealso cref="FlaxEditor.Windows.Assets.AssetEditorWindow" />
    /// <seealso cref="FlaxEditor.Surface.IVisjectSurfaceOwner" />
    public sealed class AnimationGraphWindow : VisjectWindowBase<AnimationGraph, AnimGraphSurface>, IVisjectSurfaceOwner
    {
        internal static Guid BaseModelId = new Guid(1000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        private sealed class Preview : AnimatedModelPreview
        {
            private readonly AnimationGraphWindow _window;
            private ContextMenuButton _showFloorButton;
            private StaticModel _floorModel;

            public Preview(AnimationGraphWindow window)
            : base(true)
            {
                _window = window;

                // Show floor widget
                _showFloorButton = ViewWidgetShowMenu.AddButton("Floor", OnShowFloorModelClicked);
                _showFloorButton.Icon = Style.Current.CheckBoxTick;
                _showFloorButton.IndexInParent = 1;

                // Floor model
                _floorModel = StaticModel.New();
                _floorModel.Position = new Vector3(0, -25, 0);
                _floorModel.Scale = new Vector3(5, 0.5f, 5);
                _floorModel.Model = FlaxEngine.Content.LoadAsync<Model>(StringUtils.CombinePaths(Globals.EditorFolder, "Primitives/Cube.flax"));
                Task.CustomActors.Add(_floorModel);

                // Enable shadows
                PreviewLight.ShadowsMode = ShadowsCastingMode.All;
                PreviewLight.CascadeCount = 2;
                PreviewLight.ShadowsDistance = 1000.0f;
                Task.Flags |= ViewFlags.Shadows;
            }

            private void OnShowFloorModelClicked(ContextMenuButton obj)
            {
                _floorModel.IsActive = !_floorModel.IsActive;
                _showFloorButton.Icon = _floorModel.IsActive ? Style.Current.CheckBoxTick : Sprite.Invalid;
            }

            /// <inheritdoc />
            public override void Draw()
            {
                base.Draw();

                var style = Style.Current;
                if (_window.Asset == null || !_window.Asset.IsLoaded)
                {
                    Render2D.DrawText(style.FontLarge, "Loading...", new Rectangle(Vector2.Zero, Size), Color.White, TextAlignment.Center, TextAlignment.Center);
                }
                if (_window._properties.BaseModel == null)
                {
                    Render2D.DrawText(style.FontLarge, "Missing Base Model", new Rectangle(Vector2.Zero, Size), Color.Red, TextAlignment.Center, TextAlignment.Center, TextWrapping.WrapWords);
                }
            }

            /// <inheritdoc />
            public override void Dispose()
            {
                FlaxEngine.Object.Destroy(ref _floorModel);
                _showFloorButton = null;

                base.Dispose();
            }
        }

        /// <summary>
        /// The graph properties proxy object.
        /// </summary>
        private sealed class PropertiesProxy
        {
            private SkinnedModel _baseModel;

            [EditorOrder(10), EditorDisplay("General"), Tooltip("The base model used to preview the animation and prepare the graph (skeleton bones sstructure must match in instanced AnimationModel actors)")]
            public SkinnedModel BaseModel
            {
                get => _baseModel;
                set
                {
                    if (_baseModel != value)
                    {
                        _baseModel = value;
                        if (WindowReference != null)
                            WindowReference.PreviewActor.SkinnedModel = _baseModel;
                    }
                }
            }

            [EditorOrder(1000), EditorDisplay("Parameters"), CustomEditor(typeof(ParametersEditor))]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public AnimationGraphWindow WindowReference { get; set; }

            /// <summary>
            /// Custom editor for editing graph parameters collection.
            /// </summary>
            /// <seealso cref="FlaxEditor.CustomEditors.CustomEditor" />
            public class ParametersEditor : CustomEditor
            {
                private static readonly object[] DefaultAttributes = { new LimitAttribute(float.MinValue, float.MaxValue, 0.1f) };
                private int _parametersHash;

                private enum NewParameterType
                {
                    Bool = (int)ParameterType.Bool,
                    Integer = (int)ParameterType.Integer,
                    Float = (int)ParameterType.Float,
                    Vector2 = (int)ParameterType.Vector2,
                    Vector3 = (int)ParameterType.Vector3,
                    Vector4 = (int)ParameterType.Vector4,
                    Color = (int)ParameterType.Color,
                    Rotation = (int)ParameterType.Rotation,
                    Transform = (int)ParameterType.Transform,
                }

                /// <inheritdoc />
                public override DisplayStyle Style => DisplayStyle.InlineIntoParent;

                /// <inheritdoc />
                public override void Initialize(LayoutElementsContainer layout)
                {
                    var window = Values[0] as AnimationGraphWindow;
                    var asset = window?.Asset;
                    if (asset == null)
                    {
                        _parametersHash = -1;
                        layout.Label("No parameters");
                        return;
                    }
                    if (!asset.IsLoaded)
                    {
                        _parametersHash = -2;
                        layout.Label("Loading...");
                        return;
                    }
                    var parameters = window.PreviewActor.Parameters;
                    if (parameters == null || parameters.Length == 0)
                    {
                        _parametersHash = -1;
                        layout.Label("No parameters");
                        return;
                    }
                    _parametersHash = window.PreviewActor._parametersHash;

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var p = parameters[i];
                        if (!p.IsPublic)
                            continue;

                        var pIndex = i;
                        var pValue = p.Value;
                        var pGuidType = false;
                        Type pType;
                        object[] attributes = null;
                        switch (p.Type)
                        {
                        case AnimationGraphParameterType.Asset:
                            pType = typeof(Asset);
                            pGuidType = true;
                            break;
                        default:
                            pType = p.Value.GetType();
                            // TODO: support custom attributes with defined value range for parameter (min, max)
                            attributes = DefaultAttributes;
                            break;
                        }

                        var propertyValue = new CustomValueContainer(
                            pType,
                            pValue,
                            (instance, index) =>
                            {
                                // Get parameter
                                var win = (AnimationGraphWindow)instance;
                                return win.PreviewActor.Parameters[pIndex].Value;
                            },
                            (instance, index, value) =>
                            {
                                // Set parameter and surface parameter
                                var win = (AnimationGraphWindow)instance;

                                // Visject surface parameters are only value type objects so convert value if need to (eg. instead of texture ref write texture id)
                                var surfaceParam = value;
                                if (pGuidType)
                                    surfaceParam = (value as FlaxEngine.Object)?.ID ?? Guid.Empty;

                                win.PreviewActor.Parameters[pIndex].Value = value;
                                win.Surface.Parameters[pIndex].Value = surfaceParam;
                                win._paramValueChange = true;
                            },
                            attributes
                        );

                        var propertyLabel = new DragablePropertyNameLabel(p.Name);
                        propertyLabel.Tag = pIndex;
                        propertyLabel.MouseLeftDoubleClick += (label, location) => StartParameterRenaming(label);
                        propertyLabel.MouseRightClick += (label, location) => ShowParameterMenu(label, ref location);
                        propertyLabel.Drag = DragParameter;
                        var property = layout.AddPropertyItem(propertyLabel);
                        property.Object(propertyValue);
                    }

                    if (parameters.Length > 0)
                        layout.Space(10);

                    // Parameters creating
                    var paramType = layout.Enum(typeof(NewParameterType));
                    paramType.Value = (int)NewParameterType.Float;
                    var newParam = layout.Button("Add parameter");
                    newParam.Button.Clicked += () => AddParameter((ParameterType)paramType.Value);
                }

                private DragData DragParameter(DragablePropertyNameLabel label)
                {
                    var win = (AnimationGraphWindow)Values[0];
                    var animatedModel = win.PreviewActor;
                    var parameter = animatedModel.Parameters[(int)label.Tag];
                    return DragNames.GetDragData(SurfaceParameter.DragPrefix, parameter.Name);
                }

                /// <summary>
                /// Shows the parameter context menu.
                /// </summary>
                /// <param name="label">The label control.</param>
                /// <param name="targetLocation">The target location.</param>
                private void ShowParameterMenu(ClickablePropertyNameLabel label, ref Vector2 targetLocation)
                {
                    var contextMenu = new ContextMenu();
                    contextMenu.AddButton("Rename", () => StartParameterRenaming(label));
                    contextMenu.AddButton("Delete", () => DeleteParameter((int)label.Tag));
                    contextMenu.Show(label, targetLocation);
                }

                /// <summary>
                /// Adds the parameter.
                /// </summary>
                /// <param name="type">The type.</param>
                private void AddParameter(ParameterType type)
                {
                    var window = Values[0] as AnimationGraphWindow;
                    var asset = window?.Asset;
                    if (asset == null || !asset.IsLoaded)
                        return;

                    var param = SurfaceParameter.Create(type);
                    window.Surface.Parameters.Add(param);
                    window.Surface.OnParamCreated(param);
                }

                /// <summary>
                /// Starts renaming parameter.
                /// </summary>
                /// <param name="label">The label control.</param>
                private void StartParameterRenaming(ClickablePropertyNameLabel label)
                {
                    var win = (AnimationGraphWindow)Values[0];
                    var animatedModel = win.PreviewActor;
                    var parameter = animatedModel.Parameters[(int)label.Tag];
                    var dialog = RenamePopup.Show(label, new Rectangle(0, 0, label.Width - 2, label.Height), parameter.Name, false);
                    dialog.Tag = label;
                    dialog.Renamed += OnParameterRenamed;
                }

                private void OnParameterRenamed(RenamePopup renamePopup)
                {
                    var label = (ClickablePropertyNameLabel)renamePopup.Tag;
                    var index = (int)label.Tag;
                    var newName = renamePopup.Text;

                    var win = (AnimationGraphWindow)Values[0];
                    var param = win.Surface.Parameters[index];
                    param.Name = newName;
                    label.Text = newName;
                    win.Surface.OnParamRenamed(param);
                }

                /// <summary>
                /// Removes the parameter.
                /// </summary>
                /// <param name="index">The index.</param>
                private void DeleteParameter(int index)
                {
                    var win = (AnimationGraphWindow)Values[0];
                    var param = win.Surface.Parameters[index];
                    win.Surface.Parameters.RemoveAt(index);
                    win.Surface.OnParamDeleted(param);
                }

                /// <inheritdoc />
                public override void Refresh()
                {
                    var window = Values[0] as AnimationGraphWindow;
                    var asset = window?.Asset;
                    int parametersHash = -1;
                    if (asset)
                    {
                        if (asset.IsLoaded)
                        {
                            var parameters = window.PreviewActor.Parameters; // need to ask for params here to sync valid hash   
                            parametersHash = window.PreviewActor._parametersHash;
                        }
                        else
                        {
                            parametersHash = -2;
                        }
                    }

                    if (parametersHash != _parametersHash)
                    {
                        // Parameters has been modified (loaded/unloaded/edited)
                        RebuildLayout();
                    }
                }
            }

            /// <summary>
            /// Gathers parameters from the graph.
            /// </summary>
            /// <param name="window">The graph window.</param>
            public void OnLoad(AnimationGraphWindow window)
            {
                WindowReference = window;

                var model = window.PreviewActor;
                var param = model.GetParam(BaseModelId);
                BaseModel = param?.Value as SkinnedModel;
            }

            /// <summary>
            /// Saves the properties to the graph.
            /// </summary>
            /// <param name="window">The graph window.</param>
            public void OnSave(AnimationGraphWindow window)
            {
                var model = window.PreviewActor;
                var param = model.GetParam(BaseModelId);
                if (param != null)
                    param.Value = BaseModel;
                var surfaceParam = window.Surface.GetParameter(BaseModelId);
                if (surfaceParam != null)
                    surfaceParam.Value = BaseModel?.ID ?? Guid.Empty;
            }

            /// <summary>
            /// Clears temporary data.
            /// </summary>
            public void OnClean()
            {
                // Unlink
                WindowReference = null;
            }
        }

        private readonly Preview _preview;
        private readonly NavigationBar _navigationBar;

        private readonly PropertiesProxy _properties;
        private bool _isWaitingForSurfaceLoad;
        internal bool _paramValueChange;

        /// <summary>
        /// Gets the animated model actor used for the animation preview.
        /// </summary>
        public AnimatedModel PreviewActor => _preview.PreviewActor;

        /// <inheritdoc />
        public AnimationGraphWindow(Editor editor, AssetItem item)
        : base(editor, item)
        {

            // Animation preview
            _preview = new Preview(this)
            {
                ViewportCamera = new FPSCamera(),
                ScaleToFit = false,
                PlayAnimation = true,
                Parent = _split2.Panel1
            };

            // Graph properties editor
            var propertiesEditor = new CustomEditorPresenter(null);
            propertiesEditor.Panel.Parent = _split2.Panel2;
            _properties = new PropertiesProxy();
            propertiesEditor.Select(_properties);
            propertiesEditor.Modified += OnGraphPropertyEdited;

            // Surface
            _surface = new AnimGraphSurface(this, Save)
            {
                Parent = _split1.Panel1,
                Enabled = false
            };
            Surface.ContextChanged += OnSurfaceContextChanged;

            // Toolstrip
            _toolstrip.AddSeparator();
            _toolstrip.AddButton(editor.Icons.PageScale32, Surface.ShowWholeGraph).LinkTooltip("Show the whole graph");
            _toolstrip.AddButton(editor.Icons.Bone32, () => _preview.ShowBones = !_preview.ShowBones).SetAutoCheck(true).LinkTooltip("Show animated model bones debug view");
            _toolstrip.AddSeparator();
            _toolstrip.AddButton(editor.Icons.Docs32, () => Application.StartProcess(Utilities.Constants.DocsUrl + "manual/animation/anim-graph/index.html")).LinkTooltip("See documentation to learn more");

            // Navigation bar
            _navigationBar = new NavigationBar
            {
                Parent = this
            };
        }

        private void OnGraphPropertyEdited()
        {
            Surface.MarkAsEdited(!_paramValueChange);
            _paramValueChange = false;
        }

        private void OnSurfaceContextChanged(VisjectSurfaceContext context)
        {
            Surface.UpdateNavigationBar(_navigationBar, _toolstrip);
        }

        /// <summary>
        /// Refreshes temporary asset to see changes live when editing the surface.
        /// </summary>
        /// <returns>True if cannot refresh it, otherwise false.</returns>
        public override bool RefreshTempAsset()
        {
            // Ensure that asset is loaded
            if (_asset == null || !_asset.IsLoaded)
            {
                // Error
                return true;
            }
            if (_isWaitingForSurfaceLoad)
                return true;

            // Check if surface has been edited
            if (Surface.IsEdited)
            {
                // Sync edited parameters
                _properties.OnSave(this);

                // Save surface (will call SurfaceData setter)
                Surface.Save();
            }

            return false;
        }

        /// <summary>
        /// Gets or sets the main graph node.
        /// </summary>
        private Surface.Archetypes.Animation.Output MainNode
        {
            get
            {
                var mainNode = Surface.FindNode(8, 1) as Surface.Archetypes.Animation.Output;
                if (mainNode == null)
                {
                    // Error
                    Editor.LogError("Failed to find main graph node.");
                }
                return mainNode;
            }
        }

        /// <inheritdoc />
        protected override void UnlinkItem()
        {
            _properties.OnClean();
            if (PreviewActor != null)
                PreviewActor.AnimationGraph = null;
            _isWaitingForSurfaceLoad = false;

            base.UnlinkItem();
        }

        /// <inheritdoc />
        protected override void OnAssetLinked()
        {
            PreviewActor.AnimationGraph = _asset;
            _isWaitingForSurfaceLoad = true;

            base.OnAssetLinked();
        }

        /// <inheritdoc />
        public string SurfaceName => "Anim Graph";

        /// <inheritdoc />
        public byte[] SurfaceData
        {
            get => _asset.LoadSurface();
            set
            {
                if (value == null)
                {
                    // Error
                    Editor.LogError("Failed to save animation graph surface");
                    return;
                }

                // Save data to the temporary asset
                if (_asset.SaveSurface(value))
                {
                    // Error
                    Surface.MarkAsEdited();
                    Editor.LogError("Failed to save animation graph surface data");
                    return;
                }

                // Reset any root motion
                _preview.PreviewActor.ResetLocalTransform();
            }
        }

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Check if need to load surface
            if (_isWaitingForSurfaceLoad && _asset.IsLoaded)
            {
                // Clear flag
                _isWaitingForSurfaceLoad = false;

                // Load surface data from the asset
                byte[] data = _asset.LoadSurface();
                if (data == null)
                {
                    // Error
                    Editor.LogError("Failed to load animation graph surface data.");
                    Close();
                    return;
                }

                // Load surface graph
                if (Surface.Load(data))
                {
                    // Error
                    Editor.LogError("Failed to load animation graph surface.");
                    Close();
                    return;
                }

                // Init properties and parameters proxy
                _properties.OnLoad(this);

                // Setup
                Surface.UpdateNavigationBar(_navigationBar, _toolstrip);
                Surface.Enabled = true;
                ClearEditedFlag();
            }
        }

        /// <inheritdoc />
        protected override void PerformLayoutSelf()
        {
            base.PerformLayoutSelf();

            _navigationBar?.UpdateBounds(_toolstrip);
        }
    }
}
