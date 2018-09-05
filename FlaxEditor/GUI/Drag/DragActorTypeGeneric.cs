// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEditor.SceneGraph;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Drag
{
    /// <summary>
    /// Helper class for handling actor type drag and drop (for spawning).
    /// </summary>
    /// <seealso cref="Actor" />
    /// <seealso cref="ActorNode" />
    public sealed class DragActorTypeGeneric : DragHelper<Type>
    {
        /// <inheritdoc />
        protected override void GetherObjects(DragDataTextGeneric<Type> data, Func<Type, bool> validateFunc)
        {
            var items = ParseData(data);
            for (int i = 0; i < items.Length; i++)
            {
                if (validateFunc(items[i]))
                    Objects.Add(items[i]);
            }
        }

        /// <summary>
        /// Tries to parse the drag data to extract <see cref="Type"/> collection.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Gathered objects or empty array if cannot get any valid.</returns>
        public static Type[] ParseData(DragDataTextGeneric<Type> data)
        {

            // Remove prefix and parse splitted names
            var types = data.Text.Split('\n');
            var results = new List<Type>(types.Length);
            var assembly = Utils.GetAssemblyByName("FlaxEngine");
            if (assembly != null)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    // Find type
                    var obj = assembly.GetType(types[i]);
                    if (obj != null)
                        results.Add(obj);
                }

                return results.ToArray();
            }
            else
            {
                Editor.LogWarning("Failed to get FlaxEngine assembly to spawn actor type");
                return new Type[0];
            }
        }

        /// <summary>
        /// Gets the drag data.
        /// </summary>
        /// <param name="actorType">The actor type.</param>
        /// <returns>The data.</returns>
        public static DragDataTextGeneric<Type> GetDragData(Type actorType)
        {
            if (actorType == null)
                throw new ArgumentNullException();

            return new DragDataTextGeneric<Type>(actorType.FullName);
        }
    }
}
