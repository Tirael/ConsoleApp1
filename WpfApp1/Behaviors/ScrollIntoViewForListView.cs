using System;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace WpfApp1.Behaviors
{
    public class ScrollIntoViewForListView : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListView listView))
                return;

            if (listView.SelectedItem == null)
                return;

            listView.Dispatcher.BeginInvoke((Action) (() =>
            {
                listView.UpdateLayout();

                if (listView.SelectedItem == null)
                    return;

                listView.ScrollIntoView(listView.SelectedItem);
            }));
        }
    }
}