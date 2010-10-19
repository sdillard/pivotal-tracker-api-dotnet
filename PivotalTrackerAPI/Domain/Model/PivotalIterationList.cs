using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PivotalTrackerAPI.Domain.Model
{
  /// <summary>
  /// Container class for groups of iterations
  /// </summary>
  [XmlRoot("iterations")]
  public class PivotalIterationList
  {
    /// <summary>
    /// The iterations in the response from Pivotal
    /// </summary>
    [XmlElement("iteration")]
    public List<PivotalIteration> Iterations { get; set; }
  }
}
