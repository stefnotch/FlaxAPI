using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Drag.Generic
{
    public class DragHelpers
    {
        public bool OnDragEnter<T>(DragData data, Func<T, bool> validateFunc)
        {
            if (data == null || validateFunc == null)
                throw new ArgumentNullException();

            if (data is DragDataGeneric<T> genericData)
            {
                return OnDragEnter<T>(genericData, validateFunc);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Called when drag enters.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="validateFunc">The validate function. Check if gathered object is valid to drop it.</param>
        /// <returns>True if drag event is valid and can be performed, otherwise false.</returns>
        public bool OnDragEnter<T>(DragDataGeneric<T> data, Func<T, bool> validateFunc)
        {
            if (data == null || validateFunc == null)
                throw new ArgumentNullException();

            return false;
            /*Objects.Clear();

            if (data is DragDataTextGeneric<T> text)
                GetherObjects(text, validateFunc);
            else if (data is DragDataFilesGeneric<T> files)
                GetherObjects(files, validateFunc);

            return HasValidDrag;*/
        }
    }
}
