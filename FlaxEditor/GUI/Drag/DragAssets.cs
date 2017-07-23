﻿////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using FlaxEditor.Content;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Drag
{
    /// <summary>
    /// Helper class for handling <see cref="AssetItem"/> drag and drop.
    /// </summary>
    /// <seealso cref="AssetItem" />
    public sealed class DragAssets : DragHelper<AssetItem>
    {
        /// <summary>
        /// The default prefix for drag data used for <see cref="ContentItem"/>.
        /// </summary>
        public const string DragPrefix = DragItems.DragPrefix;

        /// <inheritdoc />
        protected override void GetherObjects(DragDataText data, Func<AssetItem, bool> validateFunc)
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
        public static AssetItem[] ParseData(DragDataText data)
        {
            if (data.Text.StartsWith(DragPrefix))
            {
                // Remove prefix and parse splited names
                var paths = data.Text.Remove(0, DragPrefix.Length).Split('\n');
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

            return new AssetItem[0];
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The data.</returns>
        public static DragDataText GetDragData(AssetItem item)
        {
            return DragItems.GetDragData(item);
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The data.</returns>
        public static DragDataText GetDragData(IEnumerable<AssetItem> items)
        {
            return DragItems.GetDragData(items);
        }
    }
}
