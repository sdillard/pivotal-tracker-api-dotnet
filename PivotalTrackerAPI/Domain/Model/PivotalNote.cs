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
  /// A note/comment associated with a story
  /// </summary>
  [XmlRoot("note")]
  public class PivotalNote
  {
    /// <summary>
    /// Attributes that should be removed from the XML before posting the data.
    /// </summary>
    public static string[] ExcludeNodesOnSubmit = new string[] { "id", "noted_at" };

    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalNote() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="noteText">The note/comment</param>
    public PivotalNote(string noteText)
    {
      NoteText = noteText;
    }

   

    #region Private Properties

    private string _notedAtDateString;
    private DateTime _notedAtDate;

    #endregion

    #region Public Properties

    /// <summary>
    /// The id of the task
    /// </summary>
    [XmlElement("id", IsNullable = true)]
    public Nullable<int> TaskId { get; set; }

    /// <summary>
    /// The note/comment text
    /// </summary>
    [XmlElement("text")]
    public string NoteText { get; set; }

    /// <summary>
    /// The note/comment creator
    /// </summary>
    [XmlElement("author")]
    public string Author { get; set; }

    /// <summary>
    /// The string representing the date the task was created (as returned by Pivotal).  See CreationDate for the value
    /// </summary>
    [XmlElement("noted_at", IsNullable = true)]
    public string NotedAtDateString
    {
      get
      {
        return _notedAtDateString;
      }
      set
      {
        _notedAtDateString = value;
        if (value.Length > 4)
        {
          try
          {
            NotedAtDate = PivotalConverters.ConvertFromPivotalDateTime(value);
          }
          catch
          {
            NotedAtDate = new DateTime();
          }
        }
        else
          NotedAtDate = new DateTime();
      }
    }

    #region Non-Pivotal Properties (helpers)

    /// <summary>
    /// The date the note/comment was created
    /// </summary>
    [XmlIgnore]
    public DateTime NotedAtDate
    {
      get
      {
        return _notedAtDate;
      }
      set
      {
        _notedAtDate = value;
        _notedAtDateString = PivotalConverters.ConvertToPivotalDateTime(_notedAtDate);
      }
    }

    #endregion

    #endregion

    #region Instance Methods

    /// <summary>
    /// Uses in-memory serialization to create an identical copy of the source object's properties
    /// </summary>
    /// <returns>A new instance of the item with the same properties</returns>
    public PivotalNote Clone()
    {
      return SerializationHelper.Clone<PivotalNote>(this);
    }

    #endregion

    #region Data Operations

    /// <summary>
    /// Adds a note (comment) to a story
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project the story belongs to</param>
    /// <param name="storyId">The id of the story to add the note to</param>
    /// <param name="noteText">The text to add to the story</param>
    /// <returns>The text that was added</returns>
    public static string AddNote(PivotalUser user, int projectId, int storyId, string noteText)
    {
      return AddNote(user, projectId, storyId, noteText, null);
    }

    /// <summary>
    /// Adds a note (comment) to a story
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project the story belongs to</param>
    /// <param name="storyId">The id of the story to add the note to</param>
    /// <param name="noteText">The text to add to the story</param>
    /// <param name="author">The name of the author for the note</param>
    /// <returns>The text that was added</returns>
    public static string AddNote(PivotalUser user, int projectId, int storyId, string noteText, string author)
    {
      PivotalNote note = new PivotalNote(noteText);
      note.Author = author;
      string url = String.Format("{0}/projects/{1}/stories/{2}/notes?token={3}", PivotalService.BaseUrl, projectId, storyId, user.ApiToken);
      XmlDocument xml = SerializationHelper.SerializeToXmlDocument<PivotalNote>(note);

      string noteXml = PivotalService.CleanXmlForSubmission(xml, "//note", ExcludeNodesOnSubmit, true);

      XmlDocument response = PivotalService.SubmitData(url, noteXml, ServiceMethod.POST);
      PivotalNote addedNote = SerializationHelper.DeserializeFromXmlDocument<PivotalNote>(response);
      return addedNote.NoteText;

    }

    #endregion
  }
}
