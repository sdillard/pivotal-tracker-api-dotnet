using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PivotalTrackerAPI.Util;

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
    public IList<PivotalIteration> Iterations { get; set; }

    #region Instance Methods

    /// <summary>
    /// Uses in-memory serialization to create an identical copy of the source object's properties
    /// </summary>
    /// <returns>A new instance of the item with the same properties</returns>
    public PivotalIterationList Clone()
    {
      return SerializationHelper.Clone<PivotalIterationList>(this);
    }

    #endregion
  }
}
