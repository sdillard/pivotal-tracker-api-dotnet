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
  [XmlRoot("projects")]
  public class PivotalProjectList
  {
    [XmlElement("project")]
    public List<PivotalProject> Projects { get; set; }
  }

  [XmlRoot("project")]
  public class PivotalProject
  {
    private string _labels;
    private List<string> _labelValues;
    private List<PivotalStory> _storyCache;
    public PivotalProject() 
    {
      _labelValues = new List<string>();
      _storyCache = new List<PivotalStory>();
    }
    public PivotalProject(int id, string name) : this()
    {
      Id = id;
      Name = name;
    }

    [XmlElement("id", IsNullable = true)]
    public Nullable<int> Id { get; set; }

    [XmlElement("name", IsNullable = true)]
    public string Name { get; set; }

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

    [XmlElement("iteration_length", IsNullable = true)]
    public Nullable<int> IterationLength { get; set; }

    [XmlElement("week_start_day", IsNullable = true)]
    public string WeekStartDay { get; set; }

    [XmlElement("point_scale", IsNullable = true)]
    public string PointScale { get; set; }

    [XmlElement("velocity_scheme", IsNullable = true)]
    public string VelocityScheme { get; set; }

    [XmlElement("current_velocity", IsNullable = true)]
    public Nullable<int> CurrentVelocity { get; set; }

    [XmlElement("initial_velocity", IsNullable = true)]
    public Nullable<int> InitialVelocity { get; set; }

    [XmlElement("number_of_done_iterations_to_show", IsNullable = true)]
    public Nullable<int> NumberOfDoneIterationsToShow { get; set; }

    [XmlElement("public", IsNullable = true)]
    public Nullable<bool> IsPublic { get; set; }

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
    /// Fetches current stories from Pivotal
    /// </summary>
    /// <param name="service">PivotalService to use for the token</param>
    /// <returns>stories for the project</returns>
    public IList<PivotalStory> FetchStories(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault());
    }

    /// <summary>
    /// Fetches current stories from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <param name="service">PivotalService to use for the token</param>
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
    /// <param name="service">PivotalService to use for the token</param>
    /// <returns>bugs for the project</returns>
    public IList<PivotalStory> FetchBugs(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.bug, ""));
    }

    /// <summary>
    /// Fetches current bugs from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <remarks>The cache is updated with all stories, not just bugs, so this can be more intensive than intended.</remarks>
    /// <param name="service">PivotalService to use for the token</param>
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
    /// <param name="service">PivotalService to use for the token</param>
    /// <returns>bugs for the project</returns>
    public IList<PivotalStory> FetchChores(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.chore, ""));
    }

    /// <summary>
    /// Fetches current chores from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <remarks>The cache is updated with all stories, not just chores, so this can be more intensive than intended.</remarks>
    /// <param name="service">PivotalService to use for the token</param>
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
    /// <param name="service">PivotalService to use for the token</param>
    /// <returns>features for the project</returns>
    public IList<PivotalStory> FetchFeatures(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.feature, ""));
    }

    /// <summary>
    /// Fetches current features from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <remarks>The cache is updated with all stories, not just features, so this can be more intensive than intended.</remarks>
    /// <param name="service">PivotalService to use for the token</param>
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
    /// <param name="service">PivotalService to use for the token</param>
    /// <returns>releases for the project</returns>
    public IList<PivotalStory> FetchReleases(PivotalUser user)
    {
      return PivotalStory.FetchStories(user, Id.GetValueOrDefault(), PivotalFilterHelper.BuildStoryTypeFilter(PivotalStoryType.release, ""));
    }

    /// <summary>
    /// Fetches current releases from Pivotal and optionally reloads the cache and uses the cache
    /// </summary>
    /// <remarks>The cache is updated with all stories, not just releases, so this can be more intensive than intended.</remarks>
    /// <param name="service">PivotalService to use for the token</param>
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
    

    
    public static IList<PivotalProject> FetchProjects(PivotalUser user)
    {
      string url = String.Format("{0}/projects?token={2}", PivotalService.BaseUrl, user.ApiToken);
      XmlDocument xml = PivotalService.GetData(url);
      PivotalProjectList projectList = SerializationHelper.DeserializeFromXmlDocument<PivotalProjectList>(xml);
      return projectList.Projects;
      
    }

    public static PivotalProject FetchProject(PivotalUser user, int projectId)
    {
      string url = String.Format("{0}/project/{1}?token={2}", PivotalService.BaseUrl, projectId.ToString(), user.ApiToken);
      XmlDocument xml = PivotalService.GetData(url);
      PivotalProject project = SerializationHelper.DeserializeFromXmlDocument<PivotalProject>(xml);
      return project;

    }
  }
}
