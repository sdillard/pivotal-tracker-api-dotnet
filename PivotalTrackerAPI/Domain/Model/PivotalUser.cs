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
  [Serializable]
  [XmlRoot("token")]
  public class PivotalUser
  {
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalUser() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="apiToken">The user's api token</param>
    public PivotalUser(string apiToken)
    {
      ApiToken = apiToken;
    }

    #endregion

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

    #region Non-Pivotal Properties

    /// <summary>
    /// Returns stored projects that are set when the LoadProjects method is invoked
    /// </summary>
    [XmlIgnore]
    public IList<PivotalProject> Projects { get; private set; }

    #endregion

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

    /// <summary>
    /// Retrieves the projects the user has access to
    /// </summary>
    /// <returns>The list of projects</returns>
    public IList<PivotalProject> FetchProjects()
    {
      return PivotalProject.FetchProjects(this);
    }

    /// <summary>
    /// Fetches projects for the user and saves them in the Projects property
    /// </summary>
    /// <returns>The list of projects</returns>
    public IList<PivotalProject> LoadProjects()
    {
      Projects = FetchProjects();
      return Projects;
    }

    #endregion
  }
}
