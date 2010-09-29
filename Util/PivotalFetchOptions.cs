using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PivotalTrackerAPI.Util
{
  /// <summary>
  /// Options used when fetching data for a model.  Only used in models that maintain a cache
  /// </summary>
  public struct PivotalFetchOptions
  {
    /// <summary>
    /// Whether to use items that are currently cached (if false and the cache is empty, the cache will retrieve new items)
    /// </summary>
    public bool UseCachedItems { get; set; }
    /// <summary>
    /// Whether to refresh the cache
    /// </summary>
    public bool RefreshCache { get; set; }
  }
}
