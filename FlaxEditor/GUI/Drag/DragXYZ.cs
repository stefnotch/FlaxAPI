using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Drag
{
    //TODO: Think up a better name
    //TODO: Comments
    public class DragXYZ
    {
        private readonly List<DragHelper<object>> _dragHelpers = new List<DragHelper<object>>();

        public void AddDragHelper<T>(DragHelper<T> dragHelper)
        {
            _dragHelpers.Add(dragHelper as DragHelper<object>);
        }

        public void InvalidDrag()
        {
            foreach (var helper in _dragHelpers)
            {
                helper.InvalidDrag();
            }
        }

        public DragHelper<T> OnDragEnter<T>(DragData data, Func<T, bool> validateFunc)
        {
            return FirstWithType<T>(dragHelper => dragHelper.OnDragEnter(data, validateFunc));
        }

        public void OnDragLeave()
        {
            foreach (var helper in _dragHelpers)
            {
                helper.OnDragLeave();
            }
        }

        public void OnDragDrop()
        {
            foreach (var helper in _dragHelpers)
            {
                helper.OnDragDrop();
            }
        }

        public DragHelper<object> HasValidDrag()
        {
            return FirstWithType<object>(dragHelper => dragHelper.HasValidDrag);
        }

        public DragHelper<T> HasValidDrag<T>()
        {
            return FirstWithType<T>(dragHelper => dragHelper.HasValidDrag);
        }

        public DragHelper<T> FirstWithType<T>()
        {
            return _dragHelpers.DefaultIfEmpty(null).OfType<DragHelper<T>>().First();
        }

        public DragHelper<T> FirstWithType<T>(Func<DragHelper<T>, bool> predicate)
        {
            return _dragHelpers.DefaultIfEmpty(null).OfType<DragHelper<T>>().First(predicate);
        }
    }
}
