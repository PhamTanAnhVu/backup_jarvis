using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Automation;

namespace Jarvis_Windows.Sources.Utils.Services
{
    public interface IAutomationElementValueService
    {
        public void AddElement(AutomationElement? automationElement);
        void CheckUndoRedo(AutomationElement focusingElement);
        public void Redo(AutomationElement? automationElement);
        void StoreAction(AutomationElement? automationElement, string previousTextFromInput);
        public void Undo(AutomationElement? automationElement);
    }

    public class AutomationElementValueService : IAutomationElementValueService, INotifyPropertyChanged
    {
        private static int HANDLE_CAPACITY = 10;
        private static Dictionary<AutomationElement, IUndoRedoAction>? _automationElementValueDictionary;
        private static bool _isCanUndo;
        private static bool _isCanRedo;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal static Dictionary<AutomationElement, IUndoRedoAction> AutomationElementValueDictionary 
        { 
            get => _automationElementValueDictionary; 
            set => _automationElementValueDictionary = value; 
        }
        public bool IsCanUndo
        {
            get => _isCanUndo;
            set
            {
                _isCanUndo = value;
                OnPropertyChanged("IsCanUndo");
            }
        }

        public bool IsCanRedo
        {
            get => _isCanRedo;
            set
            {
                _isCanRedo = value;
                OnPropertyChanged("IsCanRedo");
            }
        }

        public AutomationElementValueService()
        {
            AutomationElementValueDictionary = new Dictionary<AutomationElement, IUndoRedoAction>();
        }

        public void AddElement(AutomationElement? automationElement)
        {
            if (automationElement == null)
                return;

            if (!AutomationElementValueDictionary.ContainsKey(automationElement))
            {
                if(AutomationElementValueDictionary.Count >= HANDLE_CAPACITY)
                {
                    AutomationElementValueDictionary.Remove(AutomationElementValueDictionary.Keys.Last());
                }
                AutomationElementValueDictionary.Add(automationElement, new UndoRedoAction());
            }
        }

        public void Redo(AutomationElement? automationElement)
        {
            if(automationElement == null)
                return;

            if (!AutomationElementValueDictionary.ContainsKey(automationElement))
            {
                _automationElementValueDictionary.Add(automationElement, new UndoRedoAction());
            }
            else
            {
                try
                {
                    if (automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out object objectPattern))
                    {
                        ValuePattern valuePattern = (ValuePattern)objectPattern;
                        string destText = _automationElementValueDictionary[automationElement].Redo(valuePattern.Current.Value);
                        valuePattern.SetValue(destText);
                    }
                }
                catch (ElementNotAvailableException)
                {

                }
                catch (Exception)
                {

                }
            }
            IsCanUndo = CheckCanUndo(automationElement);
            IsCanRedo = CheckCanRedo(automationElement);
        }

        public void Undo(AutomationElement? automationElement)
        {
            if (automationElement == null)
                return;

            if (!AutomationElementValueDictionary.ContainsKey(automationElement))
            {
                _automationElementValueDictionary.Add(automationElement, new UndoRedoAction());
            }
            else
            {
                try
                {
                    if (automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out object objectPattern))
                    {
                        ValuePattern valuePattern = (ValuePattern)objectPattern;
                        string destText = _automationElementValueDictionary[automationElement].Undo(valuePattern.Current.Value);
                        valuePattern.SetValue(destText);
                    }
                }
                catch (ElementNotAvailableException)
                {

                }
                catch (Exception)
                {

                }
            }
            IsCanUndo = CheckCanUndo(automationElement);
            IsCanRedo = CheckCanRedo(automationElement);
        }

        public void StoreAction(AutomationElement? automationElement, string previousTextFromInput)
        {
            if(AutomationElementValueDictionary.ContainsKey(automationElement))
            {
                AutomationElementValueDictionary[automationElement].DoAction(previousTextFromInput);
            }
            else
            {
                AutomationElementValueDictionary.Add(automationElement, new UndoRedoAction());
                AutomationElementValueDictionary[automationElement].DoAction(previousTextFromInput);
            }
            IsCanUndo = CheckCanUndo(automationElement);
            IsCanRedo = CheckCanRedo(automationElement);
        }

        public static bool CheckCanUndo(AutomationElement? automationElement)
        {
            if (automationElement == null)
                return false;

            if (!AutomationElementValueDictionary.ContainsKey(automationElement))
            {
                return false;
            }
            else
            {
                return AutomationElementValueDictionary[automationElement].IsCanUndo;
            }
        }

        public static bool CheckCanRedo(AutomationElement? automationElement)
        {
            if (automationElement == null)
                return false;

            if (!AutomationElementValueDictionary.ContainsKey(automationElement))
            {
                return false;
            }
            else
            {
                return AutomationElementValueDictionary[automationElement].IsCanRedo;
            }
        }

        public void CheckUndoRedo(AutomationElement focusingElement)
        {
            IsCanRedo = CheckCanRedo(focusingElement);
            IsCanUndo = CheckCanUndo(focusingElement);
        }
    }
}