using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PivotalTrackerAPI.Domain.Enumerations
{
  /// <summary>
  /// The state of the story in Pivotal
  /// </summary>
  public enum StoryState
  {
    /// <summary>
    /// No state defined (default value and error case)
    /// </summary>
    Unknown,
    /// <summary>
    /// Unscheduled
    /// </summary>
    Unscheduled, 
    /// <summary>
    /// Unstarted
    /// </summary>
    Unstarted, 
    /// <summary>
    /// Started
    /// </summary>
    Started, 
    /// <summary>
    /// Finished
    /// </summary>
    Finished, 
    /// <summary>
    /// Delivered
    /// </summary>
    Delivered, 
    /// <summary>
    /// Accepted
    /// </summary>
    Accepted, 
    /// <summary>
    /// Rejected
    /// </summary>
    Rejected
  }
}
