using System;
using PivotalTrackerAPI.Domain.Enumerations;
using System.Collections.Generic;
using PivotalTrackerAPI.Domain.Services;
using PivotalTrackerAPI.Util;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Web;

namespace PivotalTrackerAPI.Domain.Model
{
  /// <summary>
  /// Container class for a list of stories
  /// </summary>
  [XmlRoot("stories")]
  public class PivotalStoryList
  {
    /// <summary>
    /// List of stories
    /// </summary>
    [XmlElement("story")]
    public List<PivotalStory> Stories { get; set; }
  }
}
