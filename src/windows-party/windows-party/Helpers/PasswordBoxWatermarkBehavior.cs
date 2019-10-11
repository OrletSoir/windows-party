using System;
using System.Windows;
using System.Windows.Controls;

namespace windows_party.Helpers
{
    /**
     * Based on the TextBoxWatermarkBehaviour logic from this blog post:
     * 
     * https://blindmeis.wordpress.com/2010/07/16/wpf-watermark-textbox-behavior/
     * 
     * But modified to work with applied ControlTemplate'd TextBlock instead,
     * so we're avoiding all the extra adorner layers for text boxes and pixel-peeping alignment.
     */

    public class PasswordBoxWatermarkBehavior : System.Windows.Interactivity.Behavior<PasswordBox>
    {
        private TextBlock watermark;
        private WeakPropertyChangeNotifier notifier;

        #region DependencyProperty's
        public static readonly DependencyProperty LabelNameProperty =
            DependencyProperty.RegisterAttached("LabelName", typeof(string), typeof(PasswordBoxWatermarkBehavior));

        public string LabelName
        {
            get { return (string)GetValue(LabelNameProperty); }
            set { SetValue(LabelNameProperty, value); }
        }
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObjectLoaded;
            AssociatedObject.PasswordChanged += AssociatedObjectPasswordChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
            AssociatedObject.PasswordChanged -= AssociatedObjectPasswordChanged;

            notifier = null;
        }

        private void AssociatedObjectPasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateWatermark();
        }

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            // try to locate the innner template text block
            watermark = (TextBlock)AssociatedObject.Template.FindName(LabelName, AssociatedObject);

            UpdateWatermark();

            //AddValueChanged for IsFocused in a weak manner
            notifier = new WeakPropertyChangeNotifier(AssociatedObject, UIElement.IsFocusedProperty);
            notifier.ValueChanged += new EventHandler(UpdateAdorner);
        }

        private void UpdateAdorner(object sender, EventArgs e)
        {
            UpdateWatermark();
        }


        private void UpdateWatermark()
        {
            if (!string.IsNullOrEmpty(AssociatedObject.Password) || AssociatedObject.IsFocused)
            {
                // update inner watermark field
                if (watermark != null)
                    watermark.Visibility = Visibility.Collapsed;
            }
            else
            {
                // update inner watermark field
                if (watermark != null)
                    watermark.Visibility = Visibility.Visible;
            }
        }
    }
}
