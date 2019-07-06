using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace FlaxEditor.Surface
{
    public class NumberGraphSurface : VisjectSurface
    {
        /// </summary>
        public static readonly List<GroupArchetype> NumberGraphGroup = new List<GroupArchetype>(16)
        {
            new GroupArchetype
            {
                GroupID = 1,
                Name = "NumberGraph",
                Color = new Color(231, 231, 60),
                Archetypes = Archetypes.Material.Nodes
            }
        };

        /// <inheritdoc />
        public NumberGraphSurface(IVisjectSurfaceOwner owner, Action onSave)
        : base(owner, onSave)
        {
        }
    }
}
