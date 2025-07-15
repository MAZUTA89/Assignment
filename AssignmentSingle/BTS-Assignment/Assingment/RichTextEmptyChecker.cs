using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BTS_Assignment
{
    public class RichTextEmptyChecker : DependencyObject
    {
        public static readonly DependencyProperty IsEmptyProperty =
            DependencyProperty.RegisterAttached(
                "IsEmpty",
                typeof(bool),
                typeof(RichTextEmptyChecker),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static bool GetIsEmpty(DependencyObject obj)
            => (bool)obj.GetValue(IsEmptyProperty);

        public static void SetIsEmpty(DependencyObject obj, bool value)
            => obj.SetValue(IsEmptyProperty, value);

        // Опционально: автоматическая проверка при изменении текста
        public static readonly DependencyProperty AutoCheckProperty =
            DependencyProperty.RegisterAttached(
                "AutoCheck",
                typeof(bool),
                typeof(RichTextEmptyChecker),
                new PropertyMetadata(false, OnAutoCheckChanged));

        private static void OnAutoCheckChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox rtb && (bool)e.NewValue)
            {
                rtb.TextChanged += (s, args) =>
                {
                    SetIsEmpty(rtb, new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text.Trim() == "");
                };
            }
        }
    }
}
