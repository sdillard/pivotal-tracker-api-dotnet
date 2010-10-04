using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PivotalTrackerAPI.Domain.Enumerations
{
  /// <summary>
  /// The general groups of iterations in Pivotal
  /// </summary>
  public enum PivotalIterationGroup
  {
    /// <summary>
    /// Default value -- used when the group is not known or has not been set
    /// </summary>
    unknown,
    /// <summary>
    /// Used for retrieval -- indicates fetching records regardless of the state of the iteration
    /// </summary>
    all,
    /// <summary>
    /// completed iterations
    /// </summary>
    done,
    /// <summary>
    /// current iteration
    /// </summary>
    current,
    /// <summary>
    /// the backlog of unstarted iterations
    /// </summary>
    backlog
  }
}
