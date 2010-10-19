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
  /// Membership information for a given person
  /// </summary>
  [XmlRoot("membership")]
  public class PivotalMembership
  {
    #region Constructors

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

    #endregion

    #region Public Properties

    /// <summary>
    /// The Pivotal id of the membership (not the person?)
    /// </summary>
    [XmlElement("id")]
    public Nullable<int> Id { get; set; }

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

    #endregion

    #region Instance Methods

    /// <summary>
    /// Uses in-memory serialization to create an identical copy of the source object's properties
    /// </summary>
    /// <returns>A new instance of the item with the same properties</returns>
    public PivotalMembership Clone()
    {
      return SerializationHelper.Clone<PivotalMembership>(this);
    }

    #endregion
  }
}
