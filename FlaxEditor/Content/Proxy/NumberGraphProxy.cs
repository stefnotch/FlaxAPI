using System;
using FlaxEditor.Windows;
using FlaxEditor.Windows.Assets;
using FlaxEngine;

namespace FlaxEditor.Content
{
    /// <summary>
    ///  TODO: 
    /// A <see cref="NumberGraph"/> asset proxy object.
    /// </summary>
    /// <seealso cref="FlaxEditor.Content.JsonAssetProxy" />
    public class NumberGraphProxy : JsonAssetProxy
    {
        /// <inheritdoc />
        public override string Name => "Number Graph";

        /// <inheritdoc />
        public override EditorWindow Open(Editor editor, ContentItem item)
        {
            return new NumberGraphWindow(editor, (JsonAssetItem)item);
        }

        /// <inheritdoc />
        public override Color AccentColor => Color.FromRGB(0x0F0371);

        /// <inheritdoc />
        public override ContentDomain Domain => ContentDomain.Other;

        /// <inheritdoc />
        public override string TypeName { get; } = typeof(NumberGraph).FullName;

        /// <inheritdoc />
        public override bool CanCreate(ContentFolder targetLocation)
        {
            return targetLocation.CanHaveAssets;
        }

        /// <inheritdoc />
        public override void Create(string outputPath, object arg)
        {
            Editor.SaveJsonAsset(outputPath, NumberGraph.New());
        }
    }
}
