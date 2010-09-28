using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PivotalTrackerAPI.Domain.Model
{
  [XmlRoot("person")]
  public class PivotalPerson
  {
    public PivotalPerson() { }
    public PivotalPerson(string name, string email)
    {
      Name = name;
      Email = email;
    }
    public PivotalPerson(string name, string email, string initials) : this(name, email)
    {
      Initials = initials;
    }
    [XmlElement("email", IsNullable = true)]
    public string Email { get; set; }

    [XmlElement("name", IsNullable = true)]
    public string Name { get; set; }

    [XmlElement("initials", IsNullable = true)]
    public string Initials { get; set; }
  }
}
