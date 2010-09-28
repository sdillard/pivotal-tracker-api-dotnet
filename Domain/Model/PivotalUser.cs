using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PivotalTrackerAPI.Domain.Services;
using PivotalTrackerAPI.Util;
using System.Xml;
using System.Xml.Serialization;

namespace PivotalTrackerAPI.Domain.Model
{
  [XmlRoot("token")]
  public class PivotalUser
  {
    [XmlElement("guid")]
    public string ApiToken { get; set; }

    [XmlElement("id")]
    public Nullable<int> Id { get; set; }

    public static PivotalUser GetUserFromCredentials(string login, string password)
    {
      string url = String.Format("{0}/tokens/active", PivotalService.BaseUrlHttps);
      XmlDocument xmlDoc = PivotalService.GetDataWithCredentials(url, login, password);
      PivotalUser user = SerializationHelper.DeserializeFromXmlDocument<PivotalUser>(xmlDoc);
      return user;
    }
  }
}
