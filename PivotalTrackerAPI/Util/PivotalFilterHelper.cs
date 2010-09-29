using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PivotalTrackerAPI.Domain.Enumerations;

namespace PivotalTrackerAPI.Util
{
  /// <summary>
  /// Helpers used in constructing filters to use when querying Pivotal
  /// </summary>
  public static class PivotalFilterHelper
  {
    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="label">The label to filter on</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildLabelFilter(string label, string baseFilter)
    {
      return ConstructFilter(baseFilter, "label", label);
    }
    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="labels">The labels to filter on</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildLabelFilter(IList<string> labels, string baseFilter)
    {
      string labelString = "";
      foreach (string s in labels)
      {
        labelString += s + ",";
      }
      return ConstructFilter(baseFilter, "label", labelString);
    }

    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="storyType">The story type</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildStoryTypeFilter(PivotalStoryType storyType, string baseFilter)
    {
      return ConstructFilter(baseFilter, "type", storyType.ToString());
    }

    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="ids">The list of ids to filter on</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildIdFilter(IList<int> ids, string baseFilter)
    {
      string idString = "";
      foreach (int i in ids)
      {
        idString += i.ToString() + ",";
      }
      return ConstructFilter(baseFilter, "id", idString);
    }

    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="state">The state of the items to query (unscheduled, unstarted, started, finished, delivered, accepted, or rejected)</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildStateFilter(string state, string baseFilter)
    {
      return ConstructFilter(baseFilter, "state", state);
    }

    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="states">The states of the items to query (unscheduled, unstarted, started, finished, delivered, accepted, or rejected)</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildStateFilter(IList<string> states, string baseFilter)
    {
      string statesString = "";
      foreach (string s in states)
      {
        statesString += s + ",";
      }
      return ConstructFilter(baseFilter, "state", statesString);
    }

    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="startDate">The beginning date to filter on</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildCreatedSinceFilter(DateTime startDate, string baseFilter)
    {
      return ConstructFilter(baseFilter, "created_since", startDate.ToShortDateString());
    }

    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="startDate">The beginning date to filter on</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildModifiedSinceFilter(DateTime startDate, string baseFilter)
    {
      return ConstructFilter(baseFilter, "modified_since", startDate.ToShortDateString());
    }

    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="id">The external id to filter on</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildExternalIdFilter(string id, string baseFilter)
    {
      return ConstructFilter(baseFilter, "external_id", id);
    }

    /// <summary>
    /// Creates a query string to use as a filter parameter (does not prepend the ampersand)
    /// </summary>
    /// <param name="ids">The list of external ids to filter on</param>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <returns>A query string</returns>
    public static string BuildExternalIdFilter(IList<int> ids, string baseFilter)
    {
      string idString = "";
      foreach (int i in ids)
      {
        idString += i.ToString() + ",";
      }
      return ConstructFilter(baseFilter, "external_id", idString);
    }

    /// <summary>
    /// General helper for creating a filter, mostly for use in the other methods in the PivotalFilterHelper (does not prepend the ampersand)
    /// </summary>
    /// <param name="baseFilter">the filter to extend.  Leave blank or null to create a new filter base</param>
    /// <param name="filterName">The filter name in the query string</param>
    /// <param name="filterValue">The filter value for the query string</param>
    /// <returns>A query string</returns>
    public static string ConstructFilter(string baseFilter, string filterName, string filterValue)
    {
      string filter = "";
      if (string.IsNullOrEmpty(baseFilter))
        filter = "filter=";
      else
        filter = " ";
      return String.Format("{0}{1}:{2}", filter, filterName, filterValue);
    }
  }
}
