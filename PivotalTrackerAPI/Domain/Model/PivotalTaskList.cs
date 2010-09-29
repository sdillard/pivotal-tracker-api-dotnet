using System;
using System.Collections.Generic;
using PivotalTrackerAPI.Domain.Enumerations;
using PivotalTrackerAPI.Domain.Services;
using PivotalTrackerAPI.Util;
using System.Xml;
using System.Xml.Serialization;

namespace PivotalTrackerAPI.Domain.Model
{
  /// <summary>
  /// Container for a task list
  /// </summary>
  [XmlRoot("tasks")]
  public class PivotalTaskList
  {
    /// <summary>
    /// List of tasks
    /// </summary>
    [XmlElement("task")]
    public List<PivotalTask> Tasks { get; set; }
  }
}
