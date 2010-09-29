using System;
using System.Collections.Generic;
using PivotalTrackerAPI.Domain.Enumerations;
using PivotalTrackerAPI.Domain.Services;
using PivotalTrackerAPI.Util;
using System.Xml;
using System.Xml.Serialization;

namespace PivotalTrackerAPI.Domain.Model
{

  /// <summary>
  /// A task associated with a story
  /// </summary>
  [XmlRoot("task")]
  public class PivotalTask
  {
    /// <summary>
    /// Attributes that should be removed from the XML before posting the data.
    /// </summary>
    public static string[] ExcludeNodesOnSubmit = new string[] { "id", "created_at", "position" };

    #region Private Properties

    private string _creationDateString;
    private DateTime _creationDate;

    #endregion

    #region Public Properties

    /// <summary>
    /// The id of the task
    /// </summary>
    [XmlElement("id", IsNullable = true)]
    public Nullable<int> TaskId { get; set; }

    // TODO: Determine if the position can be updated via the API
    /// <summary>
    /// The position of the task in the list (not sure this can be set via the API?)
    /// </summary>
    [XmlElement("position", IsNullable = true)]
    public Nullable<int> Position { get; set; }

    /// <summary>
    /// The description (content) of the task
    /// </summary>
    [XmlElement("description")]
    public string Description { get; set; }

    /// <summary>
    /// Whether the task has been completed
    /// </summary>
    [XmlElement("complete", IsNullable = true)]
    public Nullable<bool> Complete { get; set; }

    /// <summary>
    /// The string representing the date the task was created (as returned by Pivotal).  See CreationDate for the value
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

    #region Non-Pivotal Properties (helpers)

    /// <summary>
    /// The date the task was created
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

    #endregion

    #endregion

    #region Data Retrieval

    /// <summary>
    /// Retrieves tasks with an optional filter
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The project id</param>
    /// <param name="storyId">The story id</param>
    /// <param name="filter">Filter to pass to Pivotal</param>
    /// <returns></returns>
    public static IList<PivotalTask> FetchTasks(PivotalUser user, string projectId, string storyId, string filter)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}/tasks?token={3}", PivotalService.BaseUrl, projectId, storyId, user.ApiToken);
      if (!string.IsNullOrEmpty(filter))
        url += "&" + filter;
      XmlDocument xmlDoc = PivotalService.GetData(url);
      PivotalTaskList taskList = SerializationHelper.DeserializeFromXmlDocument<PivotalTaskList>(xmlDoc);
      return taskList.Tasks;
    }

    #region Data Manipulation Methods

    /// <summary>
    /// Adds a task to the story
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The project id</param>
    /// <param name="storyId">The story id</param>
    /// <param name="task">The task to add</param>
    /// <returns>The created task</returns>
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
    /// <param name="user">The user to get the ApiToken from</param>
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
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="story">The story the task belongs to</param>
    /// <returns>The updated task instance</returns>
    public PivotalTask UpdateTask(PivotalUser user, PivotalStory story)
    {
      PivotalTask updatedTask = PivotalTask.UpdateTask(user, story.ProjectId.GetValueOrDefault().ToString(), story.Id.GetValueOrDefault().ToString(), this);
      System.Reflection.PropertyInfo[] properties = this.GetType().GetProperties();
      foreach (System.Reflection.PropertyInfo p in properties)
      {
        p.SetValue(this, p.GetValue(updatedTask, null), null);
      }

      return this;
    }

    /// <summary>
    /// Deletes a task from a story
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The project id</param>
    /// <param name="storyId">The story id</param>
    /// <param name="task">The task to delete</param>
    /// <returns></returns>
    public static PivotalTask DeleteTask(PivotalUser user, string projectId, string storyId, PivotalTask task)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}/task/{3}?token={4}", PivotalService.BaseUrl, projectId, storyId, task.TaskId.GetValueOrDefault().ToString(), user.ApiToken);

      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalTask>(task);

      string taskXml = PivotalService.CleanXmlForSubmission(xml, "//task/", ExcludeNodesOnSubmit, true);

      XmlDocument response = PivotalService.SubmitData(url, taskXml, ServiceMethod.DELETE);
      return task;
    }

    /// <summary>
    /// Deletes a task from a story without requiring a task instance
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The project id</param>
    /// <param name="storyId">The story id</param>
    /// <param name="taskId">The task to delete</param>
    public static void DeleteTask(PivotalUser user, string projectId, string storyId, int taskId)
    {
      string url = String.Format("{0}/projects/{1}/story/{2}/task/{3}?token={4}", PivotalService.BaseUrl, projectId, storyId, taskId, user.ApiToken);
      XmlDocument response = PivotalService.SubmitData(url, null, ServiceMethod.DELETE);
    }

    #endregion

    #endregion
  }
}
