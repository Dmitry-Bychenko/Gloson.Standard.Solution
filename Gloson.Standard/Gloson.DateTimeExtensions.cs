using System;
using System.Globalization;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// DateTime Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class DateTimeExtensions {
    #region Public

    /// <summary>
    /// Week Start 
    /// </summary>
    /// <param name="date">date</param>
    /// <param name="format">date format</param>
    /// <returns>week start</returns>
    public static DateTime WeekStart(this DateTime date, DateTimeFormatInfo format) {
      DayOfWeek start = (format ?? CultureInfo.CurrentCulture.DateTimeFormat).FirstDayOfWeek;
      DayOfWeek current = date.DayOfWeek;

      return (current >= start)
        ? date.Date.AddDays(start - current)
        : date.Date.AddDays(-7 + (int)start - (int)current);
    }

    /// <summary>
    /// Week Start
    /// </summary>
    /// <param name="date">date</param>
    /// <param name="culture">culture to use</param>
    /// <returns>week start</returns>
    public static DateTime WeekStart(this DateTime date, CultureInfo culture) =>
      WeekStart(date, (culture ?? CultureInfo.CurrentCulture).DateTimeFormat);

    /// <summary>
    /// Week Start (current culture)
    /// </summary>
    /// <param name="date">date</param>
    /// <returns>week start</returns>
    public static DateTime WeekStart(this DateTime date) =>
      WeekStart(date, CultureInfo.CurrentCulture.DateTimeFormat);

    /// <summary>
    /// Next day of week starting from a iven date
    /// </summary>
    /// <param name="fromDate">Date to start from</param>
    /// <param name="dayOfWeek">Day Of Week</param>
    /// <returns>The current of future date of day of week</returns>
    public static DateTime NextDayOfWeek(this DateTime fromDate, DayOfWeek dayOfWeek) =>
      fromDate.Date.AddDays((7 + (int)dayOfWeek - (int)fromDate.DayOfWeek) % 7);

    #endregion Public
  }
}
