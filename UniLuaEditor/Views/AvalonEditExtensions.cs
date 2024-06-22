using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UniLuaEditor.Views
{
    public static class AvalonEditExtensions
    {
        public static bool IsOpen(this CompletionWindowBase window) => window?.IsVisible == true;
    }
    public static class CommonEvent
    {
        public static RoutedEvent Register<TOwner, TEventArgs>(string name, RoutingStrategy routing)
            where TEventArgs : RoutedEventArgs
        {
            return EventManager.RegisterRoutedEvent(name, routing, typeof(EventHandler<TEventArgs>), typeof(TOwner));
        }
    }

    public sealed class ToolTipRequestEventArgs : RoutedEventArgs
    {
        public ToolTipRequestEventArgs()
        {
            RoutedEvent = CodeTextEditor.ToolTipRequestEvent;
        }

        public bool InDocument { get; set; }

        public TextLocation LogicalPosition { get; set; }

        public int Position { get; set; }

        public object ContentToShow { get; set; }

        public void SetToolTip(object content)
        {
            Handled = true;
            ContentToShow = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
