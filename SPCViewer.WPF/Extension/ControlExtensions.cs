using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace SPCViewer.WPF.Extension
{
    public static class ControlExtensions
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(PackIconKind), typeof(ControlExtensions), new PropertyMetadata(PackIconKind.None));

        public static void SetIcon(UIElement element, PackIconKind value) => element.SetValue(IconProperty, value);

        public static PackIconKind GetIcon(UIElement element) => (PackIconKind)element.GetValue(IconProperty);

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(ControlExtensions), new PropertyMetadata(Orientation.Horizontal));

        public static void SetOrientation(UIElement element, Orientation value) => element.SetValue(OrientationProperty, value);

        public static Orientation GetOrientation(UIElement element) => (Orientation)element.GetValue(OrientationProperty);
    }
}
