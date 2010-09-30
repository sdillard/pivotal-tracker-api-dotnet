using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using PivotalTrackerAPI.Domain.Enumerations;
using PivotalTrackerAPI.Domain.Services;
using PivotalTrackerAPI.Util;

namespace PivotalTrackerAPI.Domain.Model
{

  /// <summary>
  /// A project in Pivotal
  /// </summary>
  [XmlRoot("project")]
  public class PivotalProject
  {
    /// <summary>
    /// Attributes that should be removed from the XML before posting the data.
    /// </summary>
    public static string[] ExcludeNodesOnSubmit = new string[] { "id", "labels", "current_velocity" };

    #region Private Properties

    private string _labels;
    private List<string> _labelValues;
    private List<PivotalStory> _storyCache;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalProject() 
    {
      _labelValues = new List<string>();
      _storyCache = new List<PivotalStory>();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="id">The Pivotal id of the project</param>
    /// <param name="name">The project name</param>
    public PivotalProject(int id, string name) : this()
    {
      Id = id;
      Name = name;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// The Pivotal Id of the project
    /// </summary>
    [XmlElement("id", IsNullable = true)]
    public Nullable<int> Id { get; set; }

    /// <summary>
    /// The Name of the project
    /// </summary>
    [XmlElement("name", IsNullable = true)]
    public string Name { get; set; }

    //TODO: Determine if the labels can be set or if they are only read from stories
    /// <summary>
    /// Labels used in the project (not sure what happens if this is set when creating the project.  May be only retrievable?)
    /// </summary>
    [XmlElement("labels", IsNullable = true)]
    public string Labels
    {
      get
      {
        return _labels;
      }
      set
      {
        _labels = value;
        LabelValues = new List<string>(_labels.Split(",".ToCharArray()));
      }
    }

    /// <summary>
    /// The length of iterations in the project.  Usually either 1 week or 2 weeks
    /// </summary>
    [XmlElement("iteration_length", IsNullable = true)]
    public Nullable<int> IterationLength { get; set; }

    /// <summary>
    /// The string name of the day iterations start (Use WeekStartDay to get the number of the day)
    /// </summary>
    [XmlElement("week_start_day", IsNullable = true)]
    public string WeekStart { get; set; }

    // TODO: Verify the point scale can be set in the API
    /// <summary>
    /// The point scale to use for the project (Not sure this can be set?)
    /// </summary>
    [XmlElement("point_scale", IsNullable = true)]
    public string PointScale { get; set; }

    // TODO: Verify that the velocity scheme can be set in the API
    /// <summary>
    /// The velocity scheme used to calculate the current velocity (Not sure this can be set?)
    /// </summary>
    [XmlElement("velocity_scheme", IsNullable = true)]
    public string VelocityScheme { get; set; }

    /// <summary>
    /// The current velocity of the project
    /// </summary>
    [XmlElement("current_velocity", IsNullable = true)]
    public Nullable<int> CurrentVelocity { get; set; }

    /// <summary>
    /// The initial velocity of the project
    /// </summary>
    [XmlElement("initial_velocity", IsNullable = true)]
    public Nullable<int> InitialVelocity { get; set; }

    /// <summary>
    /// Number of iterations completed to show in the UI
    /// </summary>
    [XmlElement("number_of_done_iterations_to_show", IsNullable = true)]
    public Nullable<int> NumberOfDoneIterationsToShow { get; set; }

    /// <summary>
    /// Flag for whether the project is public
    /// </summary>
    [XmlElement("public", IsNullable = true)]
    public Nullable<bool> IsPublic { get; set; }

    #region Properties that are not in the XML (helper properties)

    /// <summary>
    /// Returns the day of the week iterations start on and null if the name of the day was not found
    /// </summary>
    [XmlIgnore]
    public Nullable<int> WeekStartDay
    {
      get
      {
        for (int i = 0; i < System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.DayNames.Length; i++ )
        {
          if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.DayNames[i] == WeekStart)
            return i;
        }
        return null;
      }
    }

    /// <summary>
    /// Labels used in the project
    /// </summary>
    [XmlIgnore]
    public IList<string> LabelValues
    {
      get
      {
        return _labelValues;
      }
      set
      {
        List<string> listVals = (List<string>)value;
        StringBuilder sb = new StringBuilder();
        foreach (string s in listVals)
        {
          sb.Append(s + ",");
        }
        string tmpLabels = sb.ToString();
        if (tmpLabels.Length > 0)
          _labels = tmpLabels.Substring(0, tmpLabels.Length - 1);
        else
          _labels = "";
        _labelValues = listVals;
      }
    }

    /// <summary>
    /// Holds the stories for the project, set when the LoadStories method is invoked
    /// </summary>
    [XmlIgnore]
    public IList<PivotalStory> Stories { get; private set; }

    #endregion

    #endregion

    #region Data Retrieval Methods

    #region Story Retrieval

    /// <summary>
    /// Loads the stories for the project and sets them in the Stories property
    /// </summary>
    /// <param name="user">The account to get the ApiToken from</param>
    /// <returns>List of stories</returns>
    public IList<PivotalStory> LoadStories(PivotalUser user)
    {
      Stories = FetchStories(user);
      return Stories;
    }
    /// <summary>
    /// Fetches current stories from Pivotal
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <returns>stories for the project</returns>
    public IList<PivotalStory> FetchStories(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault());
    }

    /// <summary>
    /// Fetches current stories from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="options">Options for caching</param>
    /// <returns>stories for the project</returns>
    public IList<PivotalStory> FetchStories(PivotalUser user, PivotalFetchOptions options)
    {
      if (options.RefreshCache)
        _storyCache = (List<PivotalStory>)PivotalStory.FetchStories(user, Id.GetValueOrDefault());
      if (options.UseCachedItems)
        return _storyCache;
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault());
    }

    /// <summary>
    /// Fetches current bugs from Pivotal
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <returns>bugs for the project</returns>
    public IList<PivotalStory> FetchBugs(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.bug, ""));
    }

    /// <summary>
    /// Fetches current bugs from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <remarks>The cache is updated with all stories, not just bugs, so this can be more intensive than intended.</remarks>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="options">Options for caching</param>
    /// <returns>bugs for the project</returns>
    public IList<PivotalStory> FetchBugs(PivotalUser user, PivotalFetchOptions options)
    {
      if (options.RefreshCache)
        _storyCache = (List<PivotalStory>)PivotalStory.FetchStories(user, Id.GetValueOrDefault());
      if (options.UseCachedItems)
        return _storyCache.Where(x => x.StoryType == PivotalStoryType.bug).ToList();
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.bug, ""));
    }

    /// <summary>
    /// Fetches current chores from Pivotal
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <returns>bugs for the project</returns>
    public IList<PivotalStory> FetchChores(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.chore, ""));
    }

    /// <summary>
    /// Fetches current chores from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <remarks>The cache is updated with all stories, not just chores, so this can be more intensive than intended.</remarks>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="options">Options for caching</param>
    /// <returns>chores for the project</returns>
    public IList<PivotalStory> FetchChores(PivotalUser user, PivotalFetchOptions options)
    {
      if (options.RefreshCache)
        _storyCache = (List<PivotalStory>)PivotalStory.FetchStories(user, Id.GetValueOrDefault());
      if (options.UseCachedItems)
        return _storyCache.Where(x => x.StoryType == PivotalStoryType.chore).ToList();
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.chore, ""));
    }

    /// <summary>
    /// Fetches current features from Pivotal
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <returns>features for the project</returns>
    public IList<PivotalStory> FetchFeatures(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.feature, ""));
    }

    /// <summary>
    /// Fetches current features from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <remarks>The cache is updated with all stories, not just features, so this can be more intensive than intended.</remarks>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="options">Options for caching</param>
    /// <returns>features for the project</returns>
    public IList<PivotalStory> FetchFeatures(PivotalUser user, PivotalFetchOptions options)
    {
      if (options.RefreshCache)
        _storyCache = (List<PivotalStory>)PivotalStory.FetchStories(user, Id.GetValueOrDefault());
      if (options.UseCachedItems)
        return _storyCache.Where(x => x.StoryType == PivotalStoryType.feature).ToList();
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.feature, ""));
    }

    /// <summary>
    /// Fetches current releases from Pivotal
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <returns>releases for the project</returns>
    public IList<PivotalStory> FetchReleases(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.release, ""));
    }

    /// <summary>
    /// Fetches current releases from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <remarks>The cache is updated with all stories, not just releases, so this can be more intensive than intended.</remarks>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="options">Options for caching</param>
    /// <returns>releases for the project</returns>
    public IList<PivotalStory> FetchReleases(PivotalUser user, PivotalFetchOptions options)
    {
      if (options.RefreshCache)
        _storyCache = (List<PivotalStory>)PivotalStory.FetchStories(user, Id.GetValueOrDefault());
      if (options.UseCachedItems)
        return _storyCache.Where(x => x.StoryType == PivotalStoryType.release).ToList();
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.release, ""));
    }

    #endregion

    #region Project Operations

    /// <summary>
    /// Fetches current projects from Pivot for a given user
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <returns>List of projects accessible to the user</returns>
    public static IList<PivotalProject> FetchProjects(PivotalUser user)
    {
      string url = String.Format("{0}/projects?token={2}", PivotalService.BaseUrl, user.ApiToken);
      XmlDocument xml = PivotalService.GetData(url);
      PivotalProjectList projectList = SerializationHelper.DeserializeFromXmlDocument<PivotalProjectList>(xml);
      return projectList.Projects;
    }

    /// <summary>
    /// Fetches a single project from Pivotal and does not fetch the stories
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project to retrieve</param>
    /// <returns>project</returns>
    public static PivotalProject FetchProject(PivotalUser user, int projectId)
    {
      return PivotalProject.FetchProject(user, projectId, false);
    }

    /// <summary>
    /// Fetches a single project from Pivotal and optionally retrieves the stories and sets them in the Stories property
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project to retrieve</param>
    /// <param name="loadStories">Whether to fetch stories and save them in the Stories property</param>
    /// <returns>project</returns>
    public static PivotalProject FetchProject(PivotalUser user, int projectId, bool loadStories)
    {
      string url = String.Format("{0}/project/{1}?token={2}", PivotalService.BaseUrl, projectId.ToString(), user.ApiToken);
      XmlDocument xml = PivotalService.GetData(url);
      PivotalProject project = SerializationHelper.DeserializeFromXmlDocument<PivotalProject>(xml);
      if (loadStories)
      {
        project.LoadStories(user);
      }
      return project;
    }

    /// <summary>
    /// Adds a project to Pivotal
    /// </summary>
    /// <param name="user">The user to get the ApiToken from (user will be owner of the project)</param>
    /// <param name="project">Project information to add (anything not included in the project instance will not be submitted</param>
    /// <returns>The added project</returns>
    public static PivotalProject AddProject(PivotalUser user, PivotalProject project)
    {
      string url = String.Format("{0}/projects?token={2}", PivotalService.BaseUrl, user.ApiToken);

      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalProject>(project);

      string projectXml = PivotalService.CleanXmlForSubmission(xml, "//project/", ExcludeNodesOnSubmit, true);

      XmlDocument response = PivotalService.SubmitData(url, projectXml, ServiceMethod.POST);
      return SerializationHelper.DeserializeFromXmlDocument<PivotalProject>(response);
    }

    #endregion

    #endregion
  }
}
