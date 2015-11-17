using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasyDatebookCustomControlLibrary
{
    /// <summary>
    /// A customization of the Calendar control which supports the highlighting of a list of dates.
    /// </summary>
    public class HighlightedCalendar : Calendar
    {
        /// <summary>
        /// The list of dates to be highlighted.
        /// </summary>
        public static DependencyProperty HighlightedDatesProperty =
            DependencyProperty.Register("HighlightedDates", typeof(List<DateTime>), typeof(HighlightedCalendar), new PropertyMetadata());

        /// <summary>
        /// The background brush used for highlighting dates.
        /// </summary>
        public static DependencyProperty HighlightedDatesBrushProperty =
            DependencyProperty.Register("HighlightedDatesBrush", typeof(Brush), typeof(HighlightedCalendar), new PropertyMetadata(new SolidColorBrush(Colors.Maroon)));

        static HighlightedCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HighlightedCalendar), new FrameworkPropertyMetadata(typeof(HighlightedCalendar)));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public HighlightedCalendar()
        {
            HighlightedDates = new List<DateTime>();
        }

        /// <summary>
        /// The background brush used for highlighting dates.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public Brush HighlightedDatesBrush
        {
            get { return (Brush)GetValue(HighlightedDatesBrushProperty); }
            set { SetValue(HighlightedDatesBrushProperty, value); }
        }

        /// <summary>
        /// The list of dates to be highlighted.
        /// </summary>
        [Browsable(true)]
        [Category("Highlighting")]
        public List<DateTime> HighlightedDates
        {
            get { return (List<DateTime>)GetValue(HighlightedDatesProperty); }
            set { SetValue(HighlightedDatesProperty, value); }
        }

        /// <summary>
        /// Refreshes the calendar's highlighting.
        /// </summary>
        public void Refresh()
        {
            // Simply switching the display date will do this automatically.
            var temp = DisplayDate;
            DisplayDate = DisplayDateStart ?? DateTime.MinValue;
            DisplayDate = temp;
        }
    }
}
