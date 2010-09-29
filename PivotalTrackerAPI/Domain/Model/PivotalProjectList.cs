using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using PivotalTrackerAPI.Domain.Enumerations;
using PivotalTrackerAPI.Domain.Services;
using PivotalTrackerAPI.Util;

namespace PivotalTrackerAPI.Domain.Model
{
  /// <summary>
  /// Container class for a list of projects
  /// </summary>
  [XmlRoot("projects")]
  public class PivotalProjectList
  {
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalProjectList() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="projects">List of projects</param>
    public PivotalProjectList(IList<PivotalProject> projects)
    {
      Projects = (List<PivotalProject>)projects;
    }

    #endregion

    /// <summary>
    /// The list of projects
    /// </summary>
    [XmlElement("project")]
    public List<PivotalProject> Projects { get; set; }
  }
}
