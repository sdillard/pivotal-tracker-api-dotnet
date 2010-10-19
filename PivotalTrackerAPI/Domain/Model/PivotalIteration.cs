using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PivotalTrackerAPI.Domain.Enumerations;
using PivotalTrackerAPI.Domain.Services;
using PivotalTrackerAPI.Util;

namespace PivotalTrackerAPI.Domain.Model
{
  /// <summary>
  /// A single iteration in Pivotal
  /// </summary>
  [XmlRoot("iteration")]
  public class PivotalIteration
  {
    #region Private Properties

    private string _startDateString;
    private DateTime _startDate;
    private string _finishDateString;
    private DateTime _finishDate;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalIteration() { }

    #endregion

    #region Public Properties

    /// <summary>
    /// The id for the iteration
    /// </summary>
    [XmlElement("id", IsNullable = true)]
    public Nullable<int> Id { get; set; }

    /// <summary>
    /// The order of the iteration
    /// </summary>
    [XmlElement("number", IsNullable=true)]
    public Nullable<int> IterationNumber { get; set; }

    /// <summary>
    /// The date the iteration started, or is expected to start (as the original string from Pivotal).  Use StartDate for the DateTime value
    /// </summary>
    [XmlElement("start", IsNullable = true)]
    public string StartDateString
    {
      get
      {
        return _startDateString;
      }
      set
      {
        _startDateString = value;
        if (value != null && value.Length > 4)
        {
          try
          {
            StartDate = DateTime.ParseExact(value.Substring(0, value.Length - 4), "yyyy/MM/dd hh:mm:ss", new System.Globalization.CultureInfo("en-US", true), System.Globalization.DateTimeStyles.NoCurrentDateDefault);
          }
          catch
          {
            StartDate = new DateTime();
          }
        }
        else
          StartDate = new DateTime();
      }
    }

    /// <summary>
    /// The date the iteration finished, or is expected to finish (as the original string from Pivotal).  Use FinishDate for the DateTime value
    /// </summary>
    [XmlElement("finish", IsNullable = true)]
    public string FinishDateString
    {
      get
      {
        return _finishDateString;
      }
      set
      {
        _finishDateString = value;
        if (value != null && value.Length > 4)
        {
          try
          {
            FinishDate = DateTime.ParseExact(value.Substring(0, value.Length - 4), "yyyy/MM/dd hh:mm:ss", new System.Globalization.CultureInfo("en-US", true), System.Globalization.DateTimeStyles.NoCurrentDateDefault);
          }
          catch
          {
            FinishDate = new DateTime();
          }
        }
        else
          FinishDate = new DateTime();
      }
    }

    /// <summary>
    /// The stories associated with the iteration
    /// </summary>
    [XmlElement("stories")]
    public PivotalStoryList Stories { get; set; }

    #region Non-Pivotal Properties (helpers)

    /// <summary>
    /// The date the iteration started (or is expected to start)
    /// </summary>
    [XmlIgnore]
    public DateTime StartDate
    {
      get
      {
        return _startDate;
      }
      set
      {
        _startDate = value;
        _startDateString = _startDate.ToString("yyyy/MM/dd hh:mm:ss") + " UTC";
      }
    }

    /// <summary>
    /// The date the iteration finished (or is expected to finish)
    /// </summary>
    [XmlIgnore]
    public DateTime FinishDate
    {
      get
      {
        return _finishDate;
      }
      set
      {
        _finishDate = value;
        _finishDateString = _finishDate.ToString("yyyy/MM/dd hh:mm:ss") + " UTC";
      }
    }

    #endregion

    #endregion

    #region Instance Methods

    /// <summary>
    /// Calculates the iteration's velocity based on the estimates in the stories in the iteration
    /// </summary>
    /// <returns>The iteration's velocity</returns>
    public float CalculateVelocity()
    {
      float velocity = 0.0f;
      foreach (PivotalStory s in this.Stories.Stories)
      {
        velocity += s.Estimate.GetValueOrDefault();
      }
      return velocity;
    }

    #endregion

    #region Data Operations

    /// <summary>
    /// Gets the stories for a project using a filter to limit the data returned
    /// </summary>
    /// <see cref="PivotalTrackerAPI.Util.PivotalFilterHelper"/>
    /// <remarks>See http://www.pivotaltracker.com/help/api?version=v3#get_iterations_with_limit_and_offset</remarks>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project to get stories for</param>
    /// <param name="iterationGroup">The group of iterations to constrain the list to (use unknown or all to disregard this option)</param>
    /// <returns>the stories grouped by iteration for the project</returns>
    public static IList<PivotalIteration> FetchIterations(PivotalUser user, int projectId, PivotalIterationGroup iterationGroup)
    {
      return FetchIterations(user, projectId, iterationGroup, null, null);
    }

    /// <summary>
    /// Gets the stories for a project using a filter to limit the data returned
    /// </summary>
    /// <see cref="PivotalTrackerAPI.Util.PivotalFilterHelper"/>
    /// <remarks>See http://www.pivotaltracker.com/help/api?version=v3#get_iterations_with_limit_and_offset</remarks>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project to get stories for</param>
    /// <param name="iterationGroup">The group of iterations to constrain the list to (use unknown or all to disregard this option)</param>
    /// <param name="limit">The maximum number of iterations to return</param>
    /// <param name="offset">The number of iterations to skip from the beginning of the result.</param>
    /// <returns>the stories grouped by iteration for the project</returns>
    public static IList<PivotalIteration> FetchIterations(PivotalUser user, int projectId, PivotalIterationGroup iterationGroup, Nullable<int> limit, Nullable<int> offset)
    {
      string groupSelector = "";
      if (iterationGroup != PivotalIterationGroup.unknown && iterationGroup != PivotalIterationGroup.all)
        groupSelector = "/" + iterationGroup.ToString();
      string url = String.Format("{0}/projects/{1}/iterations{2}?token={3}", PivotalService.BaseUrl, projectId.ToString(), groupSelector, user.ApiToken);
      if (limit.HasValue)
        url += "&limit=" + limit.ToString();
      if (offset.HasValue)
        url += "&offset=" + offset.ToString();
      XmlDocument xmlDoc = PivotalService.GetData(url);
      PivotalIterationList iterationList = SerializationHelper.DeserializeFromXmlDocument<PivotalIterationList>(xmlDoc);
      return iterationList.Iterations;
    }

    #endregion
  }
}
