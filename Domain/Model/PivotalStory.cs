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
  [XmlRoot("stories")]
  public class PivotalStoryList
  {
    [XmlElement("story")]
    public List<PivotalStory> Stories { get; set; }
  }

  [XmlRoot("story")]
  public class PivotalStory
  {
    public PivotalStory()
    {
      _labelValues = new List<string>();
      StoryType = PivotalStoryType.feature;
    }

    /// <summary>
    /// Attributes that should be removed from the XML before posting the data.
    /// </summary>
    public static string[] ExcludeNodesOnSubmit = new string[] { "url", "created_at", "accepted_at", "id", "project_id" };

    private string _labels;
    private string _storyTypeString;
    private string _creationDateString;
    private string _acceptedDateString;
    private DateTime _creationDate;
    private DateTime _acceptedDate;
    private PivotalStoryType _storyType;
    private List<string> _labelValues;

    [XmlElement("project_id", IsNullable = true)]
    public Nullable<int> ProjectId { get; set; }

    [XmlElement("id", IsNullable = true)]
    public Nullable<int> StoryId { get; set; }

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

    [XmlElement("name", IsNullable = true)]
    public string Name { get; set; }

    [XmlElement("description", IsNullable = true)]
    public string Description { get; set; }

    [XmlElement("requested_by", IsNullable = true)]
    public string Requestor { get; set; }

    [XmlElement("owned_by", IsNullable = true)]
    public string Owner { get; set; }

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

    [XmlElement("estimate", IsNullable = true)]
    public Nullable<int> Estimate { get; set; }

    [XmlElement("url", IsNullable = true)]
    public string Url { get; set; }

    [XmlElement("current_state", IsNullable = true)]
    public string CurrentState { get; set; }

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
    /// Gets all the stories for a project
    /// </summary>
    /// <param name="service">The service containing the token</param>
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
    /// <param name="service">The service containing the token</param>
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

    public static PivotalStory FetchStory(PivotalUser user, int projectId, string storyId)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}?token={3}", PivotalService.BaseUrl, projectId.ToString(), storyId, user.ApiToken);
      XmlDocument xmlDoc = PivotalService.GetData(url);
      PivotalStory story = SerializationHelper.DeserializeFromXmlDocument<PivotalStory>(xmlDoc);
      return story;
    }

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
    /// <param name="user.ApiToken">The API Token</param>
    /// <param name="projectId">The project id</param>
    /// <param name="story">The story id</param>
    /// <returns>The updated story instance</returns>
    public static PivotalStory UpdateStory(PivotalUser user, string projectId, PivotalStory story)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}?token={3}", PivotalService.BaseUrl, projectId, story.StoryId, user.ApiToken);

      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalStory>(story);

      string storyXml = PivotalService.CleanXmlForSubmission(xml, "//story/", ExcludeNodesOnSubmit, true);

      XmlDocument response = PivotalService.SubmitData(url, storyXml, ServiceMethod.PUT);
      return SerializationHelper.DeserializeFromXmlDocument<PivotalStory>(response);
    }

    /// <summary>
    /// Updates the story with new values
    /// </summary>
    /// <remarks>Uses reflection to iterate properties, so does carry some overhead in terms of performance</remarks>
    /// <param name="user.ApiToken">The API Token</param>
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

    public static PivotalStory DeleteStory(PivotalUser user, string projectId, PivotalStory story)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}?token={3}", PivotalService.BaseUrl, projectId, story.StoryId, user.ApiToken);

      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalStory>(story);

      string storyXml = PivotalService.CleanXmlForSubmission(xml, "//story/", ExcludeNodesOnSubmit, true);

      XmlDocument response = PivotalService.SubmitData(url, storyXml, ServiceMethod.DELETE);
      return story;
    }

    public string AddNote(PivotalUser user, string noteText)
    {
      string url = String.Format("{0}/projects/{1}/stories/{2}?token={3}", PivotalService.BaseUrl, ProjectId, StoryId, user.ApiToken);

      string storyXml = String.Format("<note><text>{0}</text></note>", WebEncoding.UrlEncode(noteText));

      XmlDocument response = PivotalService.SubmitData(url, storyXml, ServiceMethod.POST);
      return noteText;
    }

    [XmlIgnore]
    public IList<PivotalTask> Tasks{ get; private set; }

    public IList<PivotalTask> LoadTasks(PivotalUser user, string projectId)
    {
      Tasks = PivotalTask.FetchTasks(user, projectId, StoryId.GetValueOrDefault().ToString(), "");
      return Tasks;
    }
  }
}