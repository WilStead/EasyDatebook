using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace EasyDatebook_Screens
{
    /// <summary>
    /// Allows binding a rich text property represented by a XAML Document.
    /// </summary>
    public class RichTextBoxBinder : DependencyObject
    {
        private static HashSet<Thread> _doNotLoop = new HashSet<Thread>();

        /// <summary>
        /// Retrieve the bound rich text xaml associated with this property.
        /// </summary>
        /// <param name="obj">The dependency object which represents this binding.</param>
        /// <returns>The rich text xaml associated with this property, as a string.</returns>
        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }

        /// <summary>
        /// Sets the bound rich text xaml associated with this property.
        /// </summary>
        /// <param name="obj">The dependency object which represents this binding.</param>
        /// <param name="value">The rich text xaml to be associated with this property, as a string.</param>
        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            // Avoid recursion.
            _doNotLoop.Add(Thread.CurrentThread);

            obj.SetValue(DocumentXamlProperty, value);

            _doNotLoop.Remove(Thread.CurrentThread);
        }

        /// <summary>
        /// A bindable rich text property.
        /// </summary>
        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.RegisterAttached
        (
            "DocumentXaml",
            typeof(string),
            typeof(RichTextBoxBinder),
            new FrameworkPropertyMetadata
            (
                "",
                FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnDocumentXamlChanged
            )
        );

        /// <summary>
        /// Handles changes to the bound XAML Document property.
        /// </summary>
        /// <param name="obj">The dependency object which represents this binding.</param>
        /// <param name="e">A DependencyPropertyChangedEventArgs object. Not used.</param>
        protected static void OnDocumentXamlChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            // Avoid recursion.
            if (_doNotLoop.Contains(Thread.CurrentThread)) return;

            RichTextBox richTextBox = (RichTextBox)obj;

            try
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDocumentXaml(richTextBox)));
                var document = (FlowDocument)XamlReader.Load(stream);
                richTextBox.Document = document;
            }
            catch (Exception) { richTextBox.Document = new FlowDocument(); }
        }
    }
}
