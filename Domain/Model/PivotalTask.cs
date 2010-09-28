using System;
using System.Collections.Generic;
using PivotalTrackerAPI.Domain.Enumerations;
using PivotalTrackerAPI.Domain.Services;
using PivotalTrackerAPI.Util;
using System.Xml;
using System.Xml.Serialization;

namespace PivotalTrackerAPI.Domain.Model
{
  [XmlRoot("tasks")]
  public class PivotalTaskList
  {
    [XmlElement("task")]
    public List<PivotalTask> Tasks { get; set; }
  }

  [XmlRoot("task")]
  public class PivotalTask
  {
    /// <summary>
    /// Attributes that should be removed from the XML before posting the data.
    /// </summary>
    public static string[] ExcludeNodesOnSubmit = new string[] { "id", "created_at", "position" };

    private string _creationDateString;
    private DateTime _creationDate;

    [XmlIgnore]
    public string StoryId { get; set; }

    [XmlElement("id", IsNullable = true)]
    public Nullable<int> TaskId { get; set; }

    [XmlElement("position", IsNullable = true)]
    public Nullable<int> Position { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlElement("complete", IsNullable = true)]
    public Nullable<bool> Complete { get; set; }

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

    public PivotalTask AddTask(PivotalUser user, string projectId, string storyId, PivotalTask task)
    {
      string url = String.Format("{0}/projects/{1}/stories/{2}/tasks?token={3}", PivotalService.BaseUrl, projectId, storyId, user.ApiToken);

      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalTask>(task);

      string taskXml = PivotalService.CleanXmlForSubmission(xml, "//story/", ExcludeNodesOnSubmit, true);

      XmlDocument response = PivotalService.SubmitData(url, taskXml, ServiceMethod.POST);
      return SerializationHelper.DeserializeFromXmlDocument<PivotalTask>(response);
    }

    /// <summary>
    /// Updates a task without requiring a reference
    /// </summary>
    /// <param name="user.ApiToken">The API Token</param>
    /// <param name="projectId">The project id</param>
    /// <param name="storyId">The story id</param>
    /// <param name="task">The task to update</param>
    /// <returns>The updated task instance</returns>
    public static PivotalTask UpdateTask(PivotalUser user, string projectId, string storyId, PivotalTask task)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}/tasks/{3}?token={4}", PivotalService.BaseUrl, projectId, storyId, task.TaskId.GetValueOrDefault().ToString(), user.ApiToken);

      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalTask>(task);

      string taskXml = PivotalService.CleanXmlForSubmission(xml, "//task/", ExcludeNodesOnSubmit, true);

      XmlDocument response = PivotalService.SubmitData(url, taskXml, ServiceMethod.PUT);
      return SerializationHelper.DeserializeFromXmlDocument<PivotalTask>(response);
    }

    /// <summary>
    /// Updates the task with new values
    /// </summary>
    /// <remarks>Uses reflection to iterate properties, so does carry some overhead in terms of performance</remarks>
    /// <param name="user.ApiToken">The API Token</param>
    /// <returns>The updated task instance</returns>
    public PivotalTask UpdateTask(PivotalUser user, PivotalStory story)
    {
      PivotalTask updatedTask = PivotalTask.UpdateTask(user, story.ProjectId.GetValueOrDefault().ToString(), story.StoryId.GetValueOrDefault().ToString(), this);
      System.Reflection.PropertyInfo[] properties = this.GetType().GetProperties();
      foreach (System.Reflection.PropertyInfo p in properties)
      {
        p.SetValue(this, p.GetValue(updatedTask, null), null);
      }

      return this;
    }

    public static PivotalTask DeleteTask(PivotalUser user, string projectId, string storyId, PivotalTask task)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}/task/{3}?token={4}", PivotalService.BaseUrl, projectId, storyId, task.TaskId.GetValueOrDefault().ToString(), user.ApiToken);

      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalTask>(task);

      string taskXml = PivotalService.CleanXmlForSubmission(xml, "//task/", ExcludeNodesOnSubmit, true);

      XmlDocument response = PivotalService.SubmitData(url, taskXml, ServiceMethod.DELETE);
      return task;
    }

    public static IList<PivotalTask> FetchTasks(PivotalUser user, string projectId, string storyId, string filter)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}/tasks?token={3}", PivotalService.BaseUrl, projectId, storyId, user.ApiToken);
      if (!string.IsNullOrEmpty(filter))
        url += "&" + filter;
      XmlDocument xmlDoc = PivotalService.GetData(url);
      PivotalTaskList taskList = SerializationHelper.DeserializeFromXmlDocument<PivotalTaskList>(xmlDoc);
      return taskList.Tasks;
    }
  }
}
