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
    /// <summary>
    /// Container property for memberships
    /// </summary>
    [XmlElement("membership")]
    public IList<PivotalMembership> Memberships { get; set; }
  }

  /// <summary>
  /// Membership information for a given person
  /// </summary>
  [XmlRoot("membership")]
  public class PivotalMembership
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalMembership() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="person">The person the membership is associated with</param>
    /// <param name="role">The role of the person in the project</param>
    public PivotalMembership(PivotalPerson person, string role)
    {
      Person = person;
      Role = role;
    }
    /// <summary>
    /// The Pivotal id of the membership (not the person?)
    /// </summary>
    [XmlElement("id")]
    public int Id { get; set; }

    /// <summary>
    /// The person associated with the membership
    /// </summary>
    [XmlElement("person")]
    public PivotalPerson Person { get; set; }

    /// <summary>
    /// The role the person has in the project
    /// </summary>
    [XmlElement("role", IsNullable = true)]
    public string Role { get; set; }
  }
}
