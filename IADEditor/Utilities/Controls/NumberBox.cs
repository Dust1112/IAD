using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IADEditor.Utilities.Controls
{
    [TemplatePart(Name = "PART_textBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_textBox", Type = typeof(TextBox))]
    class NumberBox : Control
    {
        public string Value
        {
            get => (string)GetValue(ValueProperty); 
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumberBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );

        public double Multiplier
        {
            get => (double)GetValue(MultiplierProperty);
            set => SetValue(MultiplierProperty, value);
        }
        public static readonly DependencyProperty MultiplierProperty =
            DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(NumberBox),
                new PropertyMetadata(1.0)
                );

        private double originalValue;
        private double mouseXStart;
        private double multiplier;

        private bool captured = false;
        private bool valueChanged = false;

        static NumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), new FrameworkPropertyMetadata(typeof(NumberBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_textBlock") is TextBlock textBlock) 
            {
                textBlock.MouseLeftButtonDown += OnTextBlock_Mouse_LBD;
                textBlock.MouseLeftButtonUp += OnTextBlock_Mouse_LBU;
                textBlock.MouseMove += OnTextBlock_Mouse_MOVE;
            }
        }

        private void OnTextBlock_Mouse_MOVE(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                double mouseX = e.GetPosition(this).X;
                double d = mouseX - mouseXStart;

                if (Math.Abs(d) > SystemParameters.MinimumHorizontalDragDistance) 
                {
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) { multiplier = 0.001; }
                    else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) { multiplier = 0.1; }
                    else { multiplier = 0.01; }

                    double newValue = originalValue + (d * multiplier * Multiplier);
                    Value = string.Format(CultureInfo.InvariantCulture, "{0:N3}", newValue);
                    valueChanged = true;
                }
            }
        }

        private void OnTextBlock_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            if (captured) 
            {
                Mouse.Capture(null);
                captured = false;
                e.Handled = true;
                
                if (!valueChanged && GetTemplateChild("PART_textBox") is TextBox textBox)
                {
                    textBox.Visibility = Visibility.Visible;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void OnTextBlock_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            double.TryParse(Value, out originalValue);

            Mouse.Capture(sender as UIElement);
            captured = true;
            valueChanged = false;
            e.Handled = true;

            mouseXStart = e.GetPosition(this).X;
            Focus();
        }
    }
}
