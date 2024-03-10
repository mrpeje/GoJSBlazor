using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoJsWrapper.EventInterceptors
{
    public class UndoRedoEventInterceptor
    {
        public delegate void UndoRedoHandler();
        public event UndoRedoHandler Undo;
        public event UndoRedoHandler Redo;

        [JSInvokable]
        public void OnUndoEvent() => Undo?.Invoke();
        
        [JSInvokable]
        public void OnRedoEvent() => Redo?.Invoke();

    }
}
