using System;
using PivotalTrackerAPI.Domain.Enumerations;
using System.Collections.Generic;
using PivotalTrackerAPI.Domain.Services;
using PivotalTrackerAPI.Util;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Web;

namespace PivotalTrackerAPI.Domain.Model
{
  /// <summary>
  /// A single story in Pivotal
  /// </summary>
  [XmlRoot("story")]
  public class PivotalStory
  {
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalStory()
    {
      _labelValues = new List<string>();
      StoryType = PivotalStoryType.feature;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="storyType">The story type</param>
    /// <param name="name">Name of the story</param>
    /// <param name="description">Story description</param>
    public PivotalStory(PivotalStoryType storyType, string name, string description)
    {
      StoryType = storyType;
      Name = name;
      Description = description;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="storyType">The story type</param>
    /// <param name="name">Name of the story</param>
    /// <param name="description">Story description</param>
    /// <param name="labels">Labels for the story</param>
    public PivotalStory(PivotalStoryType storyType, string name, string description, string labels) : this(storyType, name, description)
    {
      Labels = labels;
    }

    #endregion

    /// <summary>
    /// Attributes that should be removed from the XML before posting the data.
    /// </summary>
    public static string[] ExcludeNodesOnSubmit = new string[] { "url", "created_at", "accepted_at", "id", "project_id" };

    #region Private Properties

    private string _labels;
    private string _storyTypeString;
    private string _creationDateString;
    private string _acceptedDateString;
    private DateTime _creationDate;
    private DateTime _acceptedDate;
    private PivotalStoryType _storyType;
    private List<string> _labelValues;

    #endregion

    #region Public Properties

    /// <summary>
    /// The Pivotal id of the project the story belongs to
    /// </summary>
    [XmlElement("project_id", IsNullable = true)]
    public Nullable<int> ProjectId { get; set; }

    /// <summary>
    /// The Pivotal id of the story
    /// </summary>
    [XmlElement("id", IsNullable = true)]
    public Nullable<int> Id { get; set; }

    /// <summary>
    /// The type of story as a string (use StoryType to get the enumeration)
    /// </summary>
    [XmlElement("story_type", IsNullable = true)]
    public string StoryTypeString
    {
      get { return _storyTypeString; }
      set
      {
        _storyTypeString = value;
        StoryType = (PivotalStoryType)Enum.Parse(typeof(PivotalStoryType), value);
      }
    }

    /// <summary>
    /// The name of the story
    /// </summary>
    [XmlElement("name", IsNullable = true)]
    public string Name { get; set; }

    /// <summary>
    /// The description for the story
    /// </summary>
    [XmlElement("description", IsNullable = true)]
    public string Description { get; set; }

    /// <summary>
    /// The name of the person that requested the story
    /// </summary>
    [XmlElement("requested_by", IsNullable = true)]
    public string Requestor { get; set; }

    /// <summary>
    /// The name of the owner of the story
    /// </summary>
    [XmlElement("owned_by", IsNullable = true)]
    public string Owner { get; set; }

    /// <summary>
    /// The labels associated with the story as a comma-delimited string
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
    /// The point estimate for the story
    /// </summary>
    [XmlElement("estimate", IsNullable = true)]
    public Nullable<int> Estimate { get; set; }

    /// <summary>
    /// The url for the story
    /// </summary>
    [XmlElement("url", IsNullable = true)]
    public string Url { get; set; }

    /// <summary>
    /// The current state of the story
    /// </summary>
    [XmlElement("current_state", IsNullable = true)]
    public string CurrentStateValue { get; set; }

    /// <summary>
    /// The date the story was created (as the original string from Pivotal).  Use CreationDate for the DateTime value
    /// </summary>
    [XmlElement("created_at", IsNullable = true)]
    public string CreationDateString
    {
      get
      {
        return _creationDateString;
      }
      set
      {
        _creationDateString = value;
        if (value.Length > 4)
        {
          try
          {
            CreationDate = DateTime.ParseExact(value.Substring(0, value.Length - 4), "yyyy/MM/dd hh:mm:ss", new System.Globalization.CultureInfo("en-US", true), System.Globalization.DateTimeStyles.NoCurrentDateDefault);
          }
          catch
          {
            CreationDate = new DateTime();
          }
        }
        else
          CreationDate = new DateTime();
      }
    }

    /// <summary>
    /// The date the story was accepted (as the original string from Pivotal)
    /// </summary>
    [XmlElement("accepted_at", IsNullable = true)]
    public string AcceptedDateString
    {
      get
      {
        return _acceptedDateString;
      }
      set
      {
        _acceptedDateString = value;
        if (value.Length > 4)
        {
          try
          {
            AcceptedDate = DateTime.ParseExact(value.Substring(0, value.Length - 4), "yyyy/MM/dd hh:mm:ss", new System.Globalization.CultureInfo("en-US", true), System.Globalization.DateTimeStyles.NoCurrentDateDefault);
          }
          catch
          {
            AcceptedDate = new DateTime();
          }
        }
        else
          AcceptedDate = new DateTime();
      }
    }

    #region Non-Pivotal Properties (helpers)

    /// <summary>
    /// The type of story
    /// </summary>
    [XmlIgnore]
    public PivotalStoryType StoryType
    {
      get
      {
        return _storyType;
      }
      set
      {
        _storyType = value;
        _storyTypeString = value.ToString();
      }
    }

    /// <summary>
    /// The date the story was accepted
    /// </summary>
    [XmlIgnore]
    public DateTime AcceptedDate
    {
      get
      {
        return _acceptedDate;
      }
      set
      {
        _acceptedDate = value;
        _acceptedDateString = _acceptedDate.ToString("yyyy/MM/dd hh:mm:ss") + " UTC";
      }
    }

    /// <summary>
    /// Labels associated wth the story
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
        _labels = tmpLabels.Substring(0, _labels.Length - 1);
        _labelValues = listVals;
      }
    }

    /// <summary>
    /// The current state of the story as an enumeration
    /// </summary>
    [XmlIgnore]
    public StoryState CurrentState
    {
      get
      {
        string stateText = CurrentStateValue.ToLower();
        StoryState state = StoryState.Unknown;
        try
        {
          state = (StoryState)Enum.Parse(typeof(StoryState), stateText);
        }
        catch
        {
        }
        return state;
      }
    }

    /// <summary>
    /// The creation date of the story
    /// </summary>
    [XmlIgnore]
    public DateTime CreationDate
    {
      get
      {
        return _creationDate;
      }
      set
      {
        _creationDate = value;
        _creationDateString = _creationDate.ToString("yyyy/MM/dd hh:mm:ss") + " UTC";
      }
    }

    /// <summary>
    /// The cached list of tasks for the story.  Be sure to use LoadTasks before calling this.  Once that method is called, use this property to access previously retrieved tasks
    /// </summary>
    [XmlIgnore]
    public IList<PivotalTask> Tasks { get; private set; }

    #endregion

    #endregion

    #region Data Operations

    #region Story Retrieval

    /// <summary>
    /// Gets all the stories for a project
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project to get stories for</param>
    /// <returns>the stories for the project</returns>
    public static IList<PivotalStory> FetchStories(PivotalUser user, int projectId)
    {
      return FetchStories(user, projectId, "");
    }

    /// <summary>
    /// Gets the stories for a project using a filter to limit the data returned
    /// </summary>
    /// <see cref="PivotalTrackerAPI.Util.PivotalFilterHelper"/>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project to get stories for</param>
    /// <param name="filter">The filter to limit the data returned</param>
    /// <returns>the stories for the project</returns>
    public static IList<PivotalStory> FetchStories(PivotalUser user, int projectId, string filter)
    {
      string url = String.Format("{0}/projects/{1}/stories?token={2}", PivotalService.BaseUrl, projectId.ToString(), user.ApiToken);
      if (!string.IsNullOrEmpty(filter))
        url += "&" + filter;
      XmlDocument xmlDoc = PivotalService.GetData(url);
      PivotalStoryList storyList = SerializationHelper.DeserializeFromXmlDocument<PivotalStoryList>(xmlDoc);
      return storyList.Stories;
    }
    /// <summary>
    /// Gets s single story for a project
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project to get stories for</param>
    /// <param name="storyId">The id of the story to get</param>
    /// <returns>the stories for the project</returns>
    public static PivotalStory FetchStory(PivotalUser user, int projectId, string storyId)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}?token={3}", PivotalService.BaseUrl, projectId.ToString(), storyId, user.ApiToken);
      XmlDocument xmlDoc = PivotalService.GetData(url);
      PivotalStory story = SerializationHelper.DeserializeFromXmlDocument<PivotalStory>(xmlDoc);
      return story;
    }

    #endregion

    #region Task Operations

    /// <summary>
    /// Updates the cache of tasks for the story and returns the list
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <returns></returns>
    public IList<PivotalTask> LoadTasks(PivotalUser user)
    {
      Tasks = PivotalTask.FetchTasks(user, ProjectId.ToString(), Id.GetValueOrDefault().ToString(), "");
      return Tasks;
    }

    #endregion

    #region Data Manipulation Operations

    /// <summary>
    /// Adds a story to Pivotal
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project</param>
    /// <param name="story">The story to add</param>
    /// <returns>The created story</returns>
    public static PivotalStory AddStory(PivotalUser user, int projectId, PivotalStory story)
    {
      string url = String.Format("{0}/projects/{1}/stories?token={2}", PivotalService.BaseUrl, projectId.ToString(), user.ApiToken);
      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalStory>(story);
      string storyXml = PivotalService.CleanXmlForSubmission(xml, "//story/", ExcludeNodesOnSubmit, true);
      XmlDocument response = PivotalService.SubmitData(url, storyXml, ServiceMethod.POST);
      return SerializationHelper.DeserializeFromXmlDocument<PivotalStory>(response);
    }

    /// <summary>
    /// Updates a story without requiring a reference
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The project id</param>
    /// <param name="story">The story id</param>
    /// <returns>The updated story instance</returns>
    public static PivotalStory UpdateStory(PivotalUser user, string projectId, PivotalStory story)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}?token={3}", PivotalService.BaseUrl, projectId, story.Id, user.ApiToken);
      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalStory>(story);
      string storyXml = PivotalService.CleanXmlForSubmission(xml, "//story/", ExcludeNodesOnSubmit, true);
      XmlDocument response = PivotalService.SubmitData(url, storyXml, ServiceMethod.PUT);
      return SerializationHelper.DeserializeFromXmlDocument<PivotalStory>(response);
    }

    /// <summary>
    /// Updates the story with new values
    /// </summary>
    /// <remarks>Uses reflection to iterate properties, so does carry some overhead in terms of performance</remarks>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <returns>The updated story instance</returns>
    public PivotalStory UpdateStory(PivotalUser user)
    {
      PivotalStory updatedStory = PivotalStory.UpdateStory(user, ProjectId.GetValueOrDefault().ToString(), this);
      System.Reflection.PropertyInfo[] properties = this.GetType().GetProperties();
      foreach (System.Reflection.PropertyInfo p in properties)
      {
        p.SetValue(this, p.GetValue(updatedStory, null), null);
      }
      return this;
    }

    /// <summary>
    /// Deletes a story from Pivotal
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The project id</param>
    /// <param name="story">The story id</param>
    /// <returns>The story that was deleted</returns>
    public static PivotalStory DeleteStory(PivotalUser user, string projectId, PivotalStory story)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}?token={3}", PivotalService.BaseUrl, projectId, story.Id, user.ApiToken);
      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalStory>(story);
      string storyXml = PivotalService.CleanXmlForSubmission(xml, "//story/", ExcludeNodesOnSubmit, true);
      XmlDocument response = PivotalService.SubmitData(url, storyXml, ServiceMethod.DELETE);
      return story;
    }

    /// <summary>
    /// Adds a note (comment) to a story
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="noteText">The text to add to the story</param>
    /// <returns>The text that was added</returns>
    public string AddNote(PivotalUser user, string noteText)
    {
      string url = String.Format("{0}/projects/{1}/stories/{2}?token={3}", PivotalService.BaseUrl, ProjectId, Id, user.ApiToken);

      string storyXml = String.Format("<note><text>{0}</text></note>", WebEncoding.UrlEncode(noteText));

      XmlDocument response = PivotalService.SubmitData(url, storyXml, ServiceMethod.POST);
      return noteText;
    }

    #endregion

    #endregion

    

    
  }
}