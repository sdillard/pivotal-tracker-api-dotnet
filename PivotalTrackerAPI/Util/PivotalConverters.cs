using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PivotalTrackerAPI.Util
{
  /// <summary>
  /// Provides helper functions for converting values returned by the API to values usable in the system (or vice versa)
  /// </summary>
  public static class PivotalConverters
  {
    /// <summary>
    /// Constructs a DateTime instance from the Pivotal formatted string representation of a date/time
    /// </summary>
    /// <param name="value">The text value for a Pivotal date/time</param>
    /// <returns>a DateTime instance for the value</returns>
    public static DateTime ConvertFromPivotalDateTime(string value)
    {
      return DateTime.ParseExact(value.Substring(0, value.Length - 4), "yyyy/MM/dd hh:mm:ss", new System.Globalization.CultureInfo("en-US", true), System.Globalization.DateTimeStyles.NoCurrentDateDefault);
    }

    /// <summary>
    /// Constructs a string representation in Pivotal's format for a DateTime instance
    /// </summary>
    /// <param name="value">The DateTime to convert</param>
    /// <returns>a string representation of the value</returns>
    public static string ConvertToPivotalDateTime(DateTime value)
    {
      return value.ToString("yyyy/MM/dd hh:mm:ss") + " UTC";
    }
  }
}
