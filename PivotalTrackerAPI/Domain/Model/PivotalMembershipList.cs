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
  /// Container class for lists of memberships
  /// </summary>
  [XmlRoot("memberships")]
  public class PivotalMembershipList
  {
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalMembershipList() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="members">The members associated with the group</param>
    public PivotalMembershipList(IList<PivotalMembership> members)
    {
      Memberships = members;
    }

    #endregion

    /// <summary>
    /// Container property for memberships
    /// </summary>
    [XmlElement("membership")]
    public IList<PivotalMembership> Memberships { get; set; }

    #region Instance Methods

    /// <summary>
    /// Uses in-memory serialization to create an identical copy of the source object's properties
    /// </summary>
    /// <returns>A new instance of the item with the same properties</returns>
    public PivotalMembershipList Clone()
    {
      return SerializationHelper.Clone<PivotalMembershipList>(this);
    }

    #endregion
  }
}
