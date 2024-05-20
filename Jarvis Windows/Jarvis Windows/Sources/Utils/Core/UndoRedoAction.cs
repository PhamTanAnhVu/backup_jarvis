using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.Utils.Core
{
    public interface IUndoRedoAction
    {
        public bool IsCanUndo { get; }
        public bool IsCanRedo { get; }
        public void DoAction(string currentText);
        public string Redo(string currentText);
        public string Undo(string currentText);
    }

    public class UndoRedoAction : IUndoRedoAction
    {
        private Stack<string>? _undoStack;
        private Stack<string>? _redoStack;

        public UndoRedoAction()
        {
            _undoStack = new Stack<string>();
            _redoStack = new Stack<string>();
        }

        public Stack<string>? UndoStack { get => _undoStack; }
        public Stack<string>? RedoStack { get => _redoStack; }

        public bool IsCanUndo
        {
            get => _undoStack.Count() > 0;
        }

        public bool IsCanRedo
        {
            get => _redoStack.Count() > 0;
        }

        public void DoAction(string currentText)
        {
            _undoStack.Push(currentText);
        }

        public string Redo(string currentText)
        {
            _undoStack.Push(currentText);
            string result = (_redoStack.Count() > 0) ? _redoStack.Pop() : String.Empty;
            return result;
        }

        public string Undo(string currentText)
        {
            _redoStack.Push(currentText);
            string result = (_undoStack.Count() > 0) ? _undoStack.Pop() : String.Empty;
            return result;
        }
    }
}
