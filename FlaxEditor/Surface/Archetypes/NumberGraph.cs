using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor.Surface.Elements;
using FlaxEditor.Windows.Assets;
using FlaxEngine;

namespace FlaxEditor.Surface.Archetypes
{
    /// <summary>
    /// Contains archetypes for nodes from the number graph group.
    /// </summary>
    public static class NumberGraph
    {
        /// <summary>
        /// Customized <see cref="SurfaceNode"/> for main number graph node.
        /// </summary>
        /// <seealso cref="FlaxEditor.Surface.SurfaceNode" />
        public class SurfaceNodeNumberGraph : SurfaceNode
        {
            /// <summary>
            /// Number graph node input boxes (each enum item value maps to box ID).
            /// </summary>
            public enum NumberGraphNodeBoxes
            {
                /// <summary>
                /// The float input.
                /// </summary>
                Float = 0,

                /// <summary>
                /// The Vector2 input.
                /// </summary>
                Vector2 = 1,

                /// <summary>
                /// The Vector3 input.
                /// </summary>
                Vector3 = 2,
            };

            /// <inheritdoc />
            public SurfaceNodeNumberGraph(uint id, VisjectSurfaceContext context, NodeArchetype nodeArch, GroupArchetype groupArch)
            : base(id, context, nodeArch, groupArch)
            {
            }

            /// <summary>
            /// Gets the material box.
            /// </summary>
            /// <param name="box">The input type.</param>
            /// <returns>The box</returns>
            public Box GetBox(NumberGraphNodeBoxes box)
            {
                return GetBox((int)box);
            }

            /// <summary>
            /// Update material node boxes
            /// </summary>
            public void UpdateBoxes()
            {
                // Try get parent material window
                // Maybe too hacky :D
                if (!(Surface.Owner is NumberGraphWindow window) || window.Item == null)
                    return;

                // TODO: Read NumberGraphWindow._properties and then do stuff here
                // Get material info
                /*MaterialInfo info;
                materialWindow.FillMaterialInfo(out info);

                // Update boxes
                switch (info.Domain)
                {
                case MaterialDomain.Surface:
                case MaterialDomain.Terrain:
                case MaterialDomain.Particle:
                {
                    bool isNotUnlit = info.ShadingModel != MaterialShadingModel.Unlit;
                    bool isTransparent = info.BlendMode == MaterialBlendMode.Transparent;
                    bool withTess = info.TessellationMode != TessellationMethod.None;
                    bool withSubsurface = info.ShadingModel == MaterialShadingModel.Subsurface;

                    GetBox(MaterialNodeBoxes.Color).Enabled = isNotUnlit;
                    GetBox(MaterialNodeBoxes.Mask).Enabled = true;
                    GetBox(MaterialNodeBoxes.Emissive).Enabled = true;
                    GetBox(MaterialNodeBoxes.Metalness).Enabled = isNotUnlit;
                    GetBox(MaterialNodeBoxes.Specular).Enabled = isNotUnlit;
                    GetBox(MaterialNodeBoxes.Roughness).Enabled = isNotUnlit;
                    break;
                }
                case MaterialDomain.PostProcess:
                {
                    GetBox(MaterialNodeBoxes.Color).Enabled = false;
                    GetBox(MaterialNodeBoxes.Mask).Enabled = false;
                    break;
                }
                
                default: throw new ArgumentOutOfRangeException();
                }*/
            }

            /// <inheritdoc />
            public override void ConnectionTick(Box box)
            {
                base.ConnectionTick(box);

                UpdateBoxes();
            }
        }

        public static NodeArchetype[] Nodes =
        {
            new NodeArchetype
            {
                TypeID = 1,
                Create = (id, context, arch, groupArch) => new SurfaceNodeNumberGraph(id, context, arch, groupArch),
                Title = "NumberGraph",
                Description = "Main number graph node",
                Flags = NodeFlags.AllGraphs | NodeFlags.NoRemove | NodeFlags.NoSpawnViaGUI | NodeFlags.NoCloseButton,
                Size = new Vector2(150, 300),
                Elements = new[]
                {
                    NodeElementArchetype.Factory.Input(0, "Float", true, ConnectionType.Float, 0),
                    NodeElementArchetype.Factory.Input(1, "Vector2", true, ConnectionType.Vector2, 1),
                    NodeElementArchetype.Factory.Input(2, "Vector3", true, ConnectionType.Vector3, 2)
                }
            },
            new NodeArchetype
            {
                TypeID = 2,
                Title = "Random Vector3",
                Description = "A totally random 3 component vector",
                Flags = NodeFlags.AllGraphs,
                Size = new Vector2(150, 30),
                Elements = new[]
                {
                    NodeElementArchetype.Factory.Output(0, "XYZ", ConnectionType.Vector3, 0),
                }
            },
        };
    }
}
