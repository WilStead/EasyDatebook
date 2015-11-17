using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasyDatebookCustomControlLibrary
{
    /// <summary>
    /// Converts between a DateTime and bool, indicating whether or not that DateTime should be highlighted on its HighlightedCalendar.
    /// </summary>
    public class HighlightedDateConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a DateTime to a bool, indicating whether or not that DateTime should be highlighted on its HighlightedCalendar.
        /// </summary>
        /// <param name="values">
        /// The arrary of values to be converted.
        /// Should have two elements: the DateTime, and the HighlightedCalendar which invoked this conversion.
        /// </param>
        /// <param name="targetType">The type of the target property [IGNORED].</param>
        /// <param name="parameter">A converter parameter [IGNORED].</param>
        /// <param name="culture">A culture to be observed by the conversion [IGNORED].</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return false;
            if (values[0] == null || values[1] == null) return false;

            DateTime date = (DateTime)values[0];
            HighlightedCalendar calendar = (HighlightedCalendar)values[1];

            if (calendar.HighlightedDates == null) return false;

            if (calendar.DisplayMode == CalendarMode.Year)
                return calendar.HighlightedDates.Any(d => d.Year == date.Year && d.Month == date.Month);
            else if (calendar.DisplayMode == CalendarMode.Decade)
                return calendar.HighlightedDates.Any(d => d.Year == date.Year);
            else return calendar.HighlightedDates.Contains(date);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
