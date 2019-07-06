using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace FlaxEditor.Surface
{
    /// <summary>
    /// Used to save a Visject Surface
    /// </summary>
    public class JsonSurface
    {
        public byte[] Data;


        // TODO: Replace this with some internal FlaxAPI calls or something

        public static string GetSurfacePath(JsonAsset asset)
        {
            return GetSurfacePath(asset.Path);
        }

        public static string GetSurfacePath(string path)
        {
            string extension = ".json";
            int place = path.LastIndexOf(extension);

            if (place == -1)
            {
                throw new ArgumentException($"Path is missing {extension} extension. Not a JsonAsset?", nameof(path));
            }

            return path.Remove(place, extension.Length).Insert(place, ".surface" + extension);
        }

        private class FakeSurfaceContext : ISurfaceContext
        {
            public string SurfaceName => throw new NotImplementedException();

            public byte[] SurfaceData { get; set; }

            public void OnContextCreated(VisjectSurfaceContext context)
            {
                // TODO: Create the root node?

            }
        }

        /// <summary>
        /// Tries to load surface graph from the asset.
        /// </summary>
        /// <param name="createDefaultIfMissing">True if create default surface if missing, otherwise won't load anything.</param>
        /// <returns>Loaded surface bytes or null if cannot load it or it's missing.</returns>
        public static byte[] LoadSurface(JsonAsset asset, bool createDefaultIfMissing)
        {
            // Get the path of the surface item
            string surfacePath = GetSurfacePath(asset);

            var surfaceAsset = FlaxEngine.Content.LoadAsync<JsonAsset>(surfacePath);

            JsonSurface jsonSurface;
            // Load it
            if (!surfaceAsset)
            {
                // Create it if it's missing
                if (createDefaultIfMissing)
                {
                    // Quite a hack
                    // TODO: Create a Visject Graph with a root node and serialize it!
                    var surfaceContext = new VisjectSurfaceContext(null, null, new FakeSurfaceContext());

                    // Add the root node
                    var node = NodeFactory.CreateNode(NumberGraphSurface.NumberGraphGroup, 1, surfaceContext, 1, 1);
                    if (node == null)
                    {
                        Editor.LogWarning("Failed to create node.");
                        return null;
                    }
                    surfaceContext.Nodes.Add(node);
                    // Initialize
                    node.Location = Vector2.Zero;

                    surfaceContext.Save();

                    jsonSurface = new JsonSurface()
                    {
                        Data = surfaceContext.Context.SurfaceData
                    };

                    Editor.SaveJsonAsset(surfacePath, jsonSurface);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                jsonSurface = surfaceAsset.CreateInstance<JsonSurface>();
            }

            // Return its data
            return jsonSurface.Data;
        }

        /// <summary>
        /// Updates the surface graph asset (save new one, discard cached data, reload asset).
        /// </summary>
        /// <param name="data">Surface data.</param>
        /// <returns>True if cannot save it, otherwise false.</returns>
        public static bool SaveSurface(JsonAsset asset, byte[] data)
        {
            bool success = Editor.SaveJsonAsset(GetSurfacePath(asset), new JsonSurface() { Data = data });
            asset.Reload();
            return success;
        }
    }
}
