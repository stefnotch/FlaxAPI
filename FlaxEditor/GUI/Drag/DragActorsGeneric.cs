// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEditor.SceneGraph;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Drag
{
    /// <summary>
    /// Helper class for handling <see cref="ActorNode"/> drag and drop.
    /// </summary>
    /// <seealso cref="Actor" />
    /// <seealso cref="ActorNode" />
    public sealed class DragActorsGeneric : DragHelperGeneric<ActorNode>
    {

        /// <inheritdoc />
        protected override void GetherObjects(DragDataTextGeneric<ActorNode> data, Func<ActorNode, bool> validateFunc)
        {
            var items = ParseData(data);
            for (int i = 0; i < items.Length; i++)
            {
                if (validateFunc(items[i]))
                    Objects.Add(items[i]);
            }
        }

        /// <summary>
        /// Tries to parse the drag data to extract <see cref="ActorNode"/> collection.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Gathered objects or empty array if cannot get any valid.</returns>
        public static ActorNode[] ParseData(DragDataTextGeneric<ActorNode> data)
        {
            // Remove prefix and parse splitted names
            var ids = data.Text.Split('\n');
            var results = new List<ActorNode>(ids.Length);
            for (int i = 0; i < ids.Length; i++)
            {
                // Find element
                Guid id;
                if (Guid.TryParse(ids[i], out id))
                {
                    var obj = Editor.Instance.Scene.GetActorNode(id);

                    // Check it
                    if (obj != null)
                        results.Add(obj);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<ActorNode> GetDragData(Actor actor)
        {
            if (actor == null)
                throw new ArgumentNullException();

            return new DragDataTextGeneric<ActorNode>(actor.ID.ToString("N"));
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<ActorNode> GetDragData(ActorNode item)
        {
            if (item == null)
                throw new ArgumentNullException();

            return new DragDataTextGeneric<ActorNode>(item.ID.ToString("N"));
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<ActorNode> GetDragData(IEnumerable<ActorNode> items)
        {
            if (items == null)
                throw new ArgumentNullException();

            string text = "";
            foreach (var item in items)
                text += item.ID.ToString("N") + '\n';
            return new DragDataTextGeneric<ActorNode>(text);
        }
    }
}
