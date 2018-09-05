using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaxEngine.GUI
{
    /// <summary>
    /// The drag and drop files.
    /// </summary>
    /// <seealso cref="FlaxEngine.GUI.DragData" />
    public class DragDataFiles : DragData
    {
        /// <summary>
        /// The file paths collection.
        /// </summary>
        public readonly List<string> Files;

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDataFiles"/> class.
        /// </summary>
        /// <param name="files">The files.</param>
        public DragDataFiles(IEnumerable<string> files)
        {
            Files = new List<string>(files);
        }
    }
}
