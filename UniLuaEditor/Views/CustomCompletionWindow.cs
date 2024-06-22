using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace UniLuaEditor.Views
{

    internal class CustomCompletionWindow : CompletionWindow
    {
        private bool _isSoftSelectionActive;
        private KeyEventArgs _keyDownArgs;

        public CustomCompletionWindow(TextArea textArea) : base(textArea)
        {
            _isSoftSelectionActive = true;
            CompletionList.SelectionChanged += CompletionListOnSelectionChanged;

            Initialize();
        }

        void Initialize()
        {
            CompletionList.ListBox.BorderThickness = new Thickness(0);
            CompletionList.ListBox.PreviewMouseDown += (o, e) => _isSoftSelectionActive = false;
        }

        private void CompletionListOnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (!UseHardSelection &&
                _isSoftSelectionActive && _keyDownArgs?.Handled != true
                && args.AddedItems?.Count > 0)
            {
                CompletionList.SelectedItem = null;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Home || e.Key == Key.End)
                return;

            _keyDownArgs = e;

            base.OnKeyDown(e);

            SetSoftSelection(e);
        }

        private void SetSoftSelection(RoutedEventArgs e)
        {
            if (e.Handled)
            {
                _isSoftSelectionActive = false;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Local
        public bool UseHardSelection { get; set; }
    }

}
