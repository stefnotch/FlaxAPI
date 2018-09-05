using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaxEngine.GUI
{
    /// <summary>
    /// The drag and drop text data.
    /// </summary>
    /// <seealso cref="FlaxEngine.GUI.DragData" />
    public class DragDataTextGeneric<T> : DragDataGeneric<T>
    {
        /// <summary>
        /// The text.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDataText"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public DragDataTextGeneric(string text)
        {
            Text = text;
        }
    }
}
