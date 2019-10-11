using System;
using System.Windows;
using System.Windows.Controls;

namespace windows_party.Controls.DecoratedTextBox
{
    public class DecoratedTextBox : TextBox
    {
        #region constructor/destructor
        static DecoratedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DecoratedTextBox), new FrameworkPropertyMetadata(typeof(DecoratedTextBox)));
            TextProperty.OverrideMetadata(typeof(DecoratedTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));
        }
        #endregion

        #region dependency properties
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(DecoratedTextBox), new FrameworkPropertyMetadata(String.Empty));

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        private static readonly DependencyPropertyKey RemoveWatermarkPropertyKey = DependencyProperty.RegisterReadOnly("RemoveWatermark", typeof(bool), typeof(DecoratedTextBox), new FrameworkPropertyMetadata((bool)false));

        public static readonly DependencyProperty RemoveWatermarkProperty = RemoveWatermarkPropertyKey.DependencyProperty;

        public bool RemoveWatermark
        {
            get { return (bool)GetValue(RemoveWatermarkProperty); }
        }

        static void TextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DecoratedTextBox watermarkTextBox = (DecoratedTextBox)sender;

            bool textExists = watermarkTextBox.Text.Length > 0;
            if (textExists != watermarkTextBox.RemoveWatermark)
            {
                watermarkTextBox.SetValue(RemoveWatermarkPropertyKey, textExists);
            }
        }
        #endregion
    }
}
