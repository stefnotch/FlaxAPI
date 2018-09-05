// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEditor.Content;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Drag
{
    /// <summary>
    /// Helper class for handling <see cref="ScriptItem"/> drag and drop.
    /// </summary>
    /// <seealso cref="ScriptItem" />
    public sealed class DragScriptItemsGeneric : DragHelperGeneric<ScriptItem>
    {
        /// <inheritdoc />
        protected override void GetherObjects(DragDataTextGeneric<ScriptItem> data, Func<ScriptItem, bool> validateFunc)
        {
            var items = ParseData(data);
            for (int i = 0; i < items.Length; i++)
            {
                if (validateFunc(items[i]))
                    Objects.Add(items[i]);
            }
        }

        /// <summary>
        /// Tries to parse the drag data to extract <see cref="ScriptItem"/> collection.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Gathered objects or empty array if cannot get any valid.</returns>
        public static ScriptItem[] ParseData(DragDataTextGeneric<ScriptItem> data)
        {
            // Remove prefix and parse splitted names
            var paths = data.Text.Split('\n');
            var results = new List<ScriptItem>(paths.Length);
            for (int i = 0; i < paths.Length; i++)
            {
                // Find element
                var obj = Editor.Instance.ContentDatabase.FindScript(paths[i]);

                // Check it
                if (obj != null)
                    results.Add(obj);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<ScriptItem> GetDragData(ScriptItem item)
        {
            return DragItemsGeneric.GetDragData<ScriptItem>(item);
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<ScriptItem> GetDragData(IEnumerable<ScriptItem> items)
        {
            return DragItemsGeneric.GetDragData<ScriptItem>(items);
        }
    }
}
