// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEditor.Content;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Drag
{
    /// <summary>
    /// Helper class for handling <see cref="ContentItem"/> drag and drop.
    /// </summary>
    /// <seealso cref="ContentItem" />
    public sealed class DragItemsGeneric : DragHelperGeneric<ContentItem>
    {
        /// <inheritdoc />
        protected override void GetherObjects(DragDataTextGeneric<ContentItem> data, Func<ContentItem, bool> validateFunc)
        {
            var items = ParseData(data);
            for (int i = 0; i < items.Length; i++)
            {
                if (validateFunc(items[i]))
                    Objects.Add(items[i]);
            }
        }

        /// <summary>
        /// Tries to parse the drag data to extract <see cref="ContentItem"/> collection.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Gathered objects or empty array if cannot get any valid.</returns>
        public static ContentItem[] ParseData(DragDataTextGeneric<ContentItem> data)
        {
            // Remove prefix and parse splitted names
            var paths = data.Text.Split('\n');
            var results = new List<ContentItem>(paths.Length);
            for (int i = 0; i < paths.Length; i++)
            {
                // Find element
                var obj = Editor.Instance.ContentDatabase.Find(paths[i]);

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
        public static DragDataTextGeneric<ContentItem> GetDragData(ContentItem item)
        {
            return GetDragData<ContentItem>(item);
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="path">The full content item path.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<ContentItem> GetDragData(string path)
        {
            return GetDragData<ContentItem>(path);
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<ContentItem> GetDragData(IEnumerable<ContentItem> items)
        {
            return GetDragData<ContentItem>(items);
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<T> GetDragData<T>(ContentItem item) where T : ContentItem
        {
            if (item == null)
                throw new ArgumentNullException();

            return new DragDataTextGeneric<T>(item.Path);
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="path">The full content item path.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<T> GetDragData<T>(string path) where T : ContentItem
        {
            if (path == null)
                throw new ArgumentNullException();

            return new DragDataTextGeneric<T>(path);
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<T> GetDragData<T>(IEnumerable<ContentItem> items) where T : ContentItem
        {
            if (items == null)
                throw new ArgumentNullException();

            string text = "";
            foreach (var item in items)
                text += item.Path + '\n';
            return new DragDataTextGeneric<T>(text);
        }
    }
}
