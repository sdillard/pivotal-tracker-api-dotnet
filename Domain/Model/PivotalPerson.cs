using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PivotalTrackerAPI.Domain.Model
{
  /// <summary>
  /// A person (account) in Pivotal
  /// </summary>
  [XmlRoot("person")]
  public class PivotalPerson
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalPerson() { }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">The person's name</param>
    /// <param name="email">The person's email</param>
    public PivotalPerson(string name, string email)
    {
      Name = name;
      Email = email;
    }

    /// <summary>
    /// Constuctor
    /// </summary>
    /// <param name="name">The person's name</param>
    /// <param name="email">The person's email</param>
    /// <param name="initials">The person's initials</param>
    public PivotalPerson(string name, string email, string initials) : this(name, email)
    {
      Initials = initials;
    }

    /// <summary>
    /// The person's email address
    /// </summary>
    [XmlElement("email", IsNullable = true)]
    public string Email { get; set; }

    /// <summary>
    /// The person's full (display) name
    /// </summary>
    [XmlElement("name", IsNullable = true)]
    public string Name { get; set; }

    /// <summary>
    /// The person's initials
    /// </summary>
    [XmlElement("initials", IsNullable = true)]
    public string Initials { get; set; }
  }
}
