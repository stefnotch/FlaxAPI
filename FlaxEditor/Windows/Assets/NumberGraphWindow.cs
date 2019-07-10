using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor.Content;
using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.GUI;
using FlaxEditor.GUI;
using FlaxEditor.GUI.ContextMenu;
using FlaxEditor.GUI.Drag;
using FlaxEditor.Surface;
using FlaxEditor.Viewport.Previews;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.Windows.Assets
{
    public class NumberGraphWindow : VisjectWindowBase<JsonAsset, NumberGraphSurface>, IVisjectSurfaceOwner
    {
        /// <summary>
        /// The properties proxy object.
        /// </summary>
        private sealed class PropertiesProxy
        {
            [EditorOrder(1000), EditorDisplay("Parameters"), CustomEditor(typeof(ParametersEditor))]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public NumberGraphWindow WinRef { get; set; }

            [EditorOrder(20), EditorDisplay("General"), Tooltip("It's for demo purposes")]
            public int DemoInteger { get; set; }

            /// <summary>
            /// Custom editor for editing number graph parameters collection.
            /// </summary>
            /// <seealso cref="FlaxEditor.CustomEditors.CustomEditor" />
            public class ParametersEditor : CustomEditor
            {
                private static readonly object[] DefaultAttributes = { new LimitAttribute(float.MinValue, float.MaxValue, 0.1f) };

                private enum NewParameterType
                {
                    Bool = (int)ParameterType.Bool,
                    Integer = (int)ParameterType.Integer,
                    Float = (int)ParameterType.Float,
                    Vector2 = (int)ParameterType.Vector2,
                    Vector3 = (int)ParameterType.Vector3,
                    Vector4 = (int)ParameterType.Vector4
                }

                /// <inheritdoc />
                public override DisplayStyle Style => DisplayStyle.InlineIntoParent;

                private NumberGraphWindow Window => Values[0] as NumberGraphWindow;

                /// <inheritdoc />
                public override void Initialize(LayoutElementsContainer layout)
                {
                    var numberGraph = Window?.Asset;
                    if (numberGraph == null || !numberGraph.IsLoaded)
                    {
                        layout.Label("Loading...");
                        return;
                    }
                    var parameters = Window.Surface.Parameters;

                    for (int i = 0; i < parameters.Count; i++)
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
                                var win = (NumberGraphWindow)instance;
                                return win.Surface.Parameters[pIndex].Value;
                            },
                            (instance, index, value) =>
                            {
                                var win = (NumberGraphWindow)instance;

                                // Visject surface parameters are only value type objects so convert value if need to (eg. instead of texture ref write texture id)
                                var surfaceParam = value;
                                if (pGuidType)
                                    surfaceParam = (value as FlaxEngine.Object)?.ID ?? Guid.Empty;

                                win.Surface.Parameters[pIndex].Value = surfaceParam;
                            },
                            attributes
                        );

                        var propertyLabel = new DragablePropertyNameLabel(p.Name);
                        propertyLabel.Tag = pIndex;
                        propertyLabel.MouseLeftDoubleClick += (label, location) => StartParameterRenaming(pIndex, label);
                        propertyLabel.MouseRightClick += (label, location) => ShowParameterMenu(pIndex, label, ref location);
                        propertyLabel.Drag = DragParameter;
                        var property = layout.AddPropertyItem(propertyLabel);
                        property.Object(propertyValue);
                    }

                    if (parameters.Count > 0)
                        layout.Space(10);
                    else
                        layout.Label("No parameters");

                    // Parameters creating
                    var paramType = layout.Enum(typeof(NewParameterType));
                    paramType.Value = (int)NewParameterType.Float;
                    var newParam = layout.Button("Add parameter");
                    newParam.Button.Clicked += () => AddParameter((ParameterType)paramType.Value);
                }

                private DragData DragParameter(DragablePropertyNameLabel label)
                {
                    var parameter = Window.Surface.Parameters[(int)label.Tag];
                    return DragNames.GetDragData(SurfaceParameter.DragPrefix, parameter.Name);
                }

                /// <summary>
                /// Shows the parameter context menu.
                /// </summary>
                /// <param name="index">The index.</param>
                /// <param name="label">The label control.</param>
                /// <param name="targetLocation">The target location.</param>
                private void ShowParameterMenu(int index, Control label, ref Vector2 targetLocation)
                {
                    var contextMenu = new ContextMenu();
                    contextMenu.AddButton("Rename", () => StartParameterRenaming(index, label));
                    contextMenu.AddButton("Delete", () => DeleteParameter(index));
                    contextMenu.Show(label, targetLocation);
                }

                /// <summary>
                /// Adds the parameter.
                /// </summary>
                /// <param name="type">The type.</param>
                private void AddParameter(ParameterType type)
                {
                    var particleEmitter = Window?.Asset;
                    if (particleEmitter == null || !particleEmitter.IsLoaded)
                        return;

                    var param = SurfaceParameter.Create(type);
                    Window.Surface.Parameters.Add(param);
                    Window.Surface.OnParamCreated(param);
                    RebuildLayout();
                }

                /// <summary>
                /// Starts renaming parameter.
                /// </summary>
                /// <param name="index">The index.</param>
                /// <param name="label">The label control.</param>
                private void StartParameterRenaming(int index, Control label)
                {
                    var parameter = Window.Surface.Parameters[index];
                    var dialog = RenamePopup.Show(label, new Rectangle(0, 0, label.Width - 2, label.Height), parameter.Name, false);
                    dialog.Tag = index;
                    dialog.Renamed += OnParameterRenamed;
                }

                private void OnParameterRenamed(RenamePopup renamePopup)
                {
                    var index = (int)renamePopup.Tag;
                    var newName = renamePopup.Text;

                    var param = Window.Surface.Parameters[index];
                    param.Name = newName;
                    Window.Surface.OnParamRenamed(param);
                    RebuildLayout();
                }

                /// <summary>
                /// Removes the parameter.
                /// </summary>
                /// <param name="index">The index.</param>
                private void DeleteParameter(int index)
                {
                    var param = Window.Surface.Parameters[index];
                    Window.Surface.Parameters.RemoveAt(index);
                    Window.Surface.OnParamDeleted(param);
                    RebuildLayout();
                }
            }

            /// <summary>
            /// Gathers parameters from the specified window.
            /// </summary>
            public void OnLoad(NumberGraphWindow window)
            {
                // Link
                WinRef = window;
            }

            /// <summary>
            /// Clears temporary data.
            /// </summary>
            public void OnClean()
            {
                // Unlink
                WinRef = null;
            }
        }

        private readonly NumberGraphPreview _preview;
        private readonly CustomEditorPresenter _propertiesEditor;

        private readonly PropertiesProxy _properties;
        private bool _isWaitingForSurfaceLoad;

        /// <inheritdoc />
        public NumberGraphWindow(Editor editor, AssetItem item)
        : base(editor, item)
        {

            // preview
            _preview = new NumberGraphPreview(true)
            {
                Parent = _split2.Panel1
            };

            // properties editor
            var propertiesEditor = new CustomEditorPresenter(null);
            propertiesEditor.Panel.Parent = _split2.Panel2;
            _properties = new PropertiesProxy();
            propertiesEditor.Select(_properties);
            propertiesEditor.Modified += OnPropertyEdited;
            _propertiesEditor = propertiesEditor;

            // Surface
            _surface = new NumberGraphSurface(this, Save)
            {
                Parent = _split1.Panel1,
                Enabled = false
            };

            // Toolstrip
            _toolstrip.AddSeparator();
            _toolstrip.AddButton(editor.Icons.PageScale32, _surface.ShowWholeGraph).LinkTooltip("Show whole graph");
            _toolstrip.AddSeparator();
            _toolstrip.AddButton(editor.Icons.BracketsSlash32, () => ShowSourceCode(_asset)).LinkTooltip("Show generated shader source code");
            //_toolstrip.AddButton(editor.Icons.Docs32, () => Application.StartProcess(Utilities.Constants.DocsUrl + "manual/particles/index.html")).LinkTooltip("See documentation to learn more");
        }

        private void OnPropertyEdited()
        {
            _surface.MarkAsEdited();
        }

        /// <summary>
        /// Shows the source code window.
        /// </summary>
        /// <param name="asset">The json asset.</param>
        public static void ShowSourceCode(JsonAsset asset)
        {
            var source = asset.Data;
            if (string.IsNullOrEmpty(source))
            {
                MessageBox.Show("No JSON data.", "No source.");
                return;
            }

            CreateWindowSettings settings = CreateWindowSettings.Default;
            settings.ActivateWhenFirstShown = true;
            settings.AllowMaximize = false;
            settings.AllowMinimize = false;
            settings.HasSizingFrame = false;
            settings.StartPosition = WindowStartPosition.CenterScreen;
            settings.Size = new Vector2(500, 600);
            settings.Title = "JSON Asset Source";
            var dialog = Window.Create(settings);

            var copyButton = new Button(4, 4, 100);
            copyButton.Text = "Copy";
            copyButton.Clicked += () => Application.ClipboardText = source;
            copyButton.Parent = dialog.GUI;

            var sourceTextBox = new TextBox(true, 2, copyButton.Bottom + 4, settings.Size.X - 4);
            sourceTextBox.Height = settings.Size.Y - sourceTextBox.Top - 2;
            sourceTextBox.Text = source;
            sourceTextBox.Parent = dialog.GUI;

            dialog.Show();
            dialog.Focus();
        }

        /// <summary>
        /// Refreshes temporary asset to see changes live when editing the surface.
        /// </summary>
        /// <returns>True if cannot refresh it, otherwise false.</returns>
        public override bool RefreshTempAsset()
        {
            // Early check
            if (_asset == null || _isWaitingForSurfaceLoad)
                return true;

            // Check if surface has been edited
            if (_surface.IsEdited)
            {
                _surface.Save();
            }

            _preview.RefreshAsset();

            return false;
        }

        /// <inheritdoc />
        public override void OnSave()
        {
            // Save the internal JsonSurface
            InternalSaveToOriginal();
        }

        private bool InternalSaveToOriginal()
        {
            // Wait until temporary asset file be fully loaded
            if (_asset.WaitForLoaded())
            {
                // Error
                Editor.LogError(string.Format("Cannot save asset {0}. Wait for temporary asset loaded failed.", _item.Path));
                return true;
            }

            // Cache data
            var id = _item.ID;

            // Check if original asset is loaded
            var originalAsset = FlaxEngine.Content.GetAsset<JsonAsset>(id) ?? FlaxEngine.Content.LoadAsync<JsonAsset>(id);
            if (originalAsset)
            {
                // Wait for loaded to prevent any issues
                if (originalAsset.WaitForLoaded())
                {
                    // Error
                    Editor.LogError(string.Format("Cannot save asset {0}. Wait for original asset loaded failed.", _item.Path));
                    return true;
                }

                // Copy temporary material to the final destination
                JsonSurface.SaveSurface(originalAsset, SurfaceData);
            }
            else
            {
                Editor.LogError("Cannot find original asset");
            }


            // Refresh thumbnail
            _item.RefreshThumbnail();

            return false;
        }

        protected override JsonAsset LoadAsset()
        {
            var asset = base.LoadAsset();

            // Clone the surface as well
            string sourcePath = JsonSurface.GetSurfacePath(_item.Path);
            if (Editor.ContentDatabase.Find(sourcePath) != null)
            {
                string destinationPath = JsonSurface.GetSurfacePath(asset);

                Editor.ContentEditing.CloneAssetFile(destinationPath, sourcePath, Guid.NewGuid());
            }
            return asset;
        }

        /// <inheritdoc />
        protected override void UnlinkItem()
        {
            _properties.OnClean();
            _preview.Asset = null;
            _isWaitingForSurfaceLoad = false;

            base.UnlinkItem();
        }

        /// <inheritdoc />
        protected override void OnAssetLinked()
        {
            _preview.Asset = _asset;
            _isWaitingForSurfaceLoad = true;

            base.OnAssetLinked();
        }

        /// <inheritdoc />
        public string SurfaceName => "Number Graph";

        /// <inheritdoc />
        public byte[] SurfaceData
        {
            get => JsonSurface.LoadSurface(_asset, true);
            set
            {
                // Compile the surface
                if (_surface != null)
                {
                    var graphInstance = _asset.CreateInstance<NumberGraph>();
                    graphInstance.NumberGraphDefinition = _surface.CompileToGraphDefinition();
                    Editor.SaveJsonAsset(_asset.Path, graphInstance);
                }

                // Save data to the temporary asset
                if (JsonSurface.SaveSurface(_asset, value))
                {
                    // Error
                    _surface.MarkAsEdited();
                    Editor.LogError("Failed to save surface data");
                }

                //_preview.PreviewActor.ResetSimulation();
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

                // Init asset properties and parameters proxy
                _properties.OnLoad(this);

                // Load surface data from the asset
                byte[] data = JsonSurface.LoadSurface(_asset, true);

                if (data == null)
                {
                    // Error
                    Editor.LogError("Failed to load surface data.");
                    Close();
                    return;
                }

                // Load surface graph
                if (_surface.Load(data))
                {
                    // Error
                    Editor.LogError("Failed to load surface.");
                    Close();
                    return;
                }

                // Setup
                _surface.Enabled = true;
                _propertiesEditor.BuildLayout();
                ClearEditedFlag();
            }
        }
    }
}
