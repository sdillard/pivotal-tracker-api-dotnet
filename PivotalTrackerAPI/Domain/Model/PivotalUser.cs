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
  /// <summary>
  /// User account in Pivotal.  This information cannot be updated via the API
  /// </summary>
  [XmlRoot("token")]
  public class PivotalUser
  {
    #region Public Properties

    /// <summary>
    /// The ApiToken for the user -- used for all access to the Pivotal API.
    /// </summary>
    [XmlElement("guid")]
    public string ApiToken { get; set; }

    /// <summary>
    /// The user's id in Pivotal
    /// </summary>
    [XmlElement("id")]
    public Nullable<int> Id { get; set; }

    #endregion

    #region Operations

    /// <summary>
    /// Retrieves the user's information, including the token, from Pivotal
    /// </summary>
    /// <param name="login">The user's login</param>
    /// <param name="password">The user's password</param>
    /// <returns>A Pivotal User containing the ApiToken for the user</returns>
    public static PivotalUser GetUserFromCredentials(string login, string password)
    {
      string url = String.Format("{0}/tokens/active", PivotalService.BaseUrlHttps);
      XmlDocument xmlDoc = PivotalService.GetDataWithCredentials(url, login, password);
      PivotalUser user = SerializationHelper.DeserializeFromXmlDocument<PivotalUser>(xmlDoc);
      return user;
    }

    #endregion
  }
}
