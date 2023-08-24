using System.Windows;
using System.Windows.Media;

namespace IADEditor;
    
public static class VisualExtensions
{
    public static T FindVisualParent<T>(this DependencyObject dependencyObject) where T : DependencyObject
    {
        if (!(dependencyObject is Visual)) { return null; }

        var parent = VisualTreeHelper.GetParent(dependencyObject);
        while (parent != null)
        {
            if (parent is T type) { return type; }
            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }
}