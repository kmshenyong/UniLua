using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;

namespace UniLuaEditor
{
    public static class WpfExtensions
    {
        public static Window GetWindow(this UIElement o) => Window.GetWindow(o);

        public static Dispatcher GetDispatcher(this DispatcherObject o) => o.Dispatcher;

        public static Size GetRenderSize(this UIElement element) => element.RenderSize;

        public static void HookupLoadedUnloadedAction(this FrameworkElement element, Action<bool> action)
        {
            if (element.IsLoaded)
            {
                action(true);
            }

            element.Loaded += (o, e) => action(true);
            element.Unloaded += (o, e) => action(false);
        }

        public static void HookupLoadedAction(this FrameworkElement element, Action action)
        {
            if (element.IsLoaded)
            {
                action();
            }

            element.Loaded += (o, e) => action();
        }

        public static void HookupUnloadedAction(this FrameworkElement element, Action action)
        {
            element.Unloaded += (o, e) => action();
        }


        public static void AttachLocationChanged(this Window topLevel, EventHandler handler)
        {
            topLevel.LocationChanged += handler;
        }

        public static void DetachLocationChanged(this Window topLevel, EventHandler handler)
        {
            topLevel.LocationChanged -= handler;
        }

        public static T AsFrozen<T>(this T freezable) where T : Freezable
        {
            freezable.Freeze();
            return freezable;
        }

        public static void BeginFigure(this StreamGeometryContext context, Point point, bool isFilled)
        {
            context.BeginFigure(point, isFilled, isClosed: false);
        }

        public static void SetBorderThickness(this Control control, double thickness)
        {
            control.BorderThickness = new Thickness(thickness);
        }

        public static bool HasModifiers(this KeyEventArgs args, ModifierKeys modifier) =>
            (args.KeyboardDevice.Modifiers & modifier) == modifier;

        public static void Open(this ToolTip toolTip, FrameworkElement control) => toolTip.IsOpen = true;
        public static void Close(this ToolTip toolTip, FrameworkElement control) => toolTip.IsOpen = false;
        public static void SetContent(this ToolTip toolTip, Control control, object content) => toolTip.Content = content;

        public static void SetItems(this ItemsControl itemsControl, System.Collections.IEnumerable enumerable) =>
            itemsControl.ItemsSource = enumerable;

        public static void Open(this ContextMenu contextMenu, FrameworkElement element)
        {
            contextMenu.PlacementTarget = element;
            contextMenu.IsOpen = true;
        }

        public static void Open(this ContextMenu contextMenu)
        {
            contextMenu.IsOpen = true;
        }

        public static void Close(this ContextMenu contextMenu)
        {
            contextMenu.IsOpen = false;
        }
    }
    public sealed class StyledProperty<TValue>
    {
        public DependencyProperty Property { get; }

        public StyledProperty(DependencyProperty property)
        {
            Property = property;
        }

        public StyledProperty<TValue> AddOwner<TOwner>() =>
            new StyledProperty<TValue>(Property.AddOwner(typeof(TOwner)));

        public Type PropertyType => Property.PropertyType;
    }
    [Flags]
    public enum PropertyOptions
    {
        None,
        AffectsRender = 1,
        AffectsArrange = 2,
        AffectsMeasure = 4,
        BindsTwoWay = 8,
        Inherits = 16,
    }
    public class CommonPropertyChangedArgs<T>
    {
        public T OldValue { get; }

        public T NewValue { get; }

        public CommonPropertyChangedArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
    public static class PropertyExtensions
    {
        public static bool Has(this PropertyOptions options, PropertyOptions value) =>
            (options & value) == value;
    }
    public static class CommonProperty
    {
        public static StyledProperty<TValue> Register<TOwner, TValue>(string name,
            TValue defaultValue = default, PropertyOptions options = PropertyOptions.None,
            Action<TOwner, CommonPropertyChangedArgs<TValue>> onChanged = null)
            where TOwner : DependencyObject
        {
            var metadataOptions = FrameworkPropertyMetadataOptions.None;

            if (options.Has(PropertyOptions.AffectsRender))
            {
                metadataOptions |= FrameworkPropertyMetadataOptions.AffectsRender;
            }

            if (options.Has(PropertyOptions.AffectsArrange))
            {
                metadataOptions |= FrameworkPropertyMetadataOptions.AffectsArrange;
            }

            if (options.Has(PropertyOptions.AffectsMeasure))
            {
                metadataOptions |= FrameworkPropertyMetadataOptions.AffectsMeasure;
            }

            if (options.Has(PropertyOptions.Inherits))
            {
                metadataOptions |= FrameworkPropertyMetadataOptions.Inherits;
            }

            if (options.Has(PropertyOptions.BindsTwoWay))
            {
                metadataOptions |= FrameworkPropertyMetadataOptions.BindsTwoWayByDefault;
            }

            var changedCallback = onChanged != null
                ? new PropertyChangedCallback((o, e) => onChanged((TOwner)o, new CommonPropertyChangedArgs<TValue>((TValue)e.OldValue, (TValue)e.NewValue)))
                : null;
            var metadata = new FrameworkPropertyMetadata(defaultValue, metadataOptions, changedCallback);
            var property = DependencyProperty.Register(name, typeof(TValue), typeof(TOwner), metadata);

            return new StyledProperty<TValue>(property);
        }

        public static TValue GetValue<TValue>(this DependencyObject o, StyledProperty<TValue> property)
        {
            return (TValue)o.GetValue(property.Property);
        }

        public static void SetValue<TValue>(this DependencyObject o, StyledProperty<TValue> property, TValue value)
        {
            o.SetValue(property.Property, value);
        }
    }
}
