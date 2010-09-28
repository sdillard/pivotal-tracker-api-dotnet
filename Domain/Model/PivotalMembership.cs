using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PivotalTrackerAPI.Domain.Model
{
  [XmlRoot("memberships")]
  public class PivotalMembershipList
  {
    [XmlElement("membership")]
    public IList<PivotalMembership> Memberships { get; set; }
  }

  [XmlRoot("membership")]
  public class PivotalMembership
  {
    [XmlElement("id")]
    public int Id { get; set; }

    [XmlElement("person")]
    public PivotalPerson Person { get; set; }

    [XmlElement("role", IsNullable = true)]
    public string Role { get; set; }
  }
}
