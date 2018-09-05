// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEditor.Content;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Drag
{
    /// <summary>
    /// Helper class for handling <see cref="AssetItem"/> drag and drop.
    /// </summary>
    /// <seealso cref="AssetItem" />
    public sealed class DragAssetsGeneric : DragHelper<AssetItem>
    {
        /// <inheritdoc />
        protected override void GetherObjects(DragDataTextGeneric<AssetItem> data, Func<AssetItem, bool> validateFunc)
        {
            var items = ParseData(data);
            for (int i = 0; i < items.Length; i++)
            {
                if (validateFunc(items[i]))
                    Objects.Add(items[i]);
            }
        }

        /// <summary>
        /// Tries to parse the drag data to extract <see cref="AssetItem"/> collection.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Gathered objects or empty array if cannot get any valid.</returns>
        public static AssetItem[] ParseData(DragDataTextGeneric<AssetItem> data)
        {
            // Remove prefix and parse splitted names
            var paths = data.Text.Split('\n');
            var results = new List<AssetItem>(paths.Length);
            for (int i = 0; i < paths.Length; i++)
            {
                // Find element
                var obj = Editor.Instance.ContentDatabase.Find(paths[i]) as AssetItem;

                // Check it
                if (obj != null)
                    results.Add(obj);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Gets the drag data (finds asset item).
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<AssetItem> GetDragData(Asset asset)
        {
            return DragItemsGeneric.GetDragData<AssetItem>(Editor.Instance.ContentDatabase.Find(asset.ID));
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<AssetItem> GetDragData(AssetItem item)
        {
            return DragItemsGeneric.GetDragData<AssetItem>(item);
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<AssetItem> GetDragData(IEnumerable<AssetItem> items)
        {
            return DragItemsGeneric.GetDragData<AssetItem>(items);
        }
    }
}
