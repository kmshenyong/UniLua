using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConfigurationEditor.Browse
{
    /// <summary>
    /// Interaction logic for ConfigurationBrowse
    /// </summary>
    public partial class ConfigurationBrowse : UserControl
    {
        public ConfigurationBrowse()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                SectionTree.Focus();
            };
        }

        public ConfigurationBrowseViewModel ViewModel => DataContext as ConfigurationBrowseViewModel;

        /// <summary>
        /// ListViewItem 鼠标双击无法绑定 InputBindings，因此这么写
        /// /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_MouseDoubleClickExecuteEditCommand(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.EditValueCommand.CanExecute())
                ViewModel.EditValueCommand.Execute();
        }

        /// <summary>
        /// if one listViewItem has focus, then another listViewItem is selected, then the focus is move to another listViewItem
        /// </summary>
        /// <remarks>On the listView, the focus is on the selection</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_FocusWhenSelected(object sender, RoutedEventArgs e)
        {
            var item = sender as ListViewItem;
            Debug.Assert(item != null, nameof(item) + " != null");

            // retrieve keyboard focus
            var focusElement = Keyboard.FocusedElement as DependencyObject;
            if (focusElement == null)
                return;

            // find focus scope
            var focusScope = FocusManager.GetFocusScope(focusElement);

            // if focus scope is ListView
            if (focusScope == ValueListView && item.IsSelected)
                item.Focus();
        }

        private void SectionTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var focus = Keyboard.FocusedElement as TreeViewItem;
            if (focus != null && SectionTree.IsFocused == false)
                SectionTree.Focus();
        }
    }
}
