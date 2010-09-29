using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
  }
}
