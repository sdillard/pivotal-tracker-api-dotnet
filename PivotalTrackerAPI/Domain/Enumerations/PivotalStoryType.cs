using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PivotalTrackerAPI.Domain.Enumerations
{
  /// <summary>
  /// Types of stories used in Pivotal
  /// </summary>
  public enum PivotalStoryType
  {
    /// <summary>
    /// Feature
    /// </summary>
    feature,
    /// <summary>
    /// Chore
    /// </summary>
    chore,
    /// <summary>
    /// Bug
    /// </summary>
    bug,
    /// <summary>
    /// Release
    /// </summary>
    release
  }
}
