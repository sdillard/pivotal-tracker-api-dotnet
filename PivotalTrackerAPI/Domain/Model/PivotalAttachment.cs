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
  public class PivotalAttachment
  {
    /// <summary>
    /// Attributes that should be removed from the XML before posting the data.
    /// </summary>
    public static string[] ExcludeNodesOnSubmit = new string[] { "id", "status" };

    /// <summary>
    /// Constructor
    /// </summary>
    public PivotalAttachment() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data">The file data</param>
    public PivotalAttachment(byte[] data)
    {
      AttachmentData = data;
    }


    #region Public Properties

    /// <summary>
    /// The id of the attachment
    /// </summary>
    [XmlElement("id", IsNullable = true)]
    public Nullable<int> TaskId { get; set; }

    /// <summary>
    /// The status of the attachment
    /// </summary>
    [XmlElement("status")]
    public string Status { get; set; }

    /// <summary>
    /// The attachment data
    /// </summary>
    [XmlIgnore]
    public byte[] AttachmentData { get; set; }

    #endregion

    #region Instance Methods

    /// <summary>
    /// Uses in-memory serialization to create an identical copy of the source object's properties
    /// </summary>
    /// <returns>A new instance of the item with the same properties</returns>
    public PivotalAttachment Clone()
    {
      return SerializationHelper.Clone<PivotalAttachment>(this);
    }

    #endregion

    #region Data Operations

    /// <summary>
    /// Adds a note (comment) to a story
    /// </summary>
    /// <param name="user">The user to get the ApiToken from</param>
    /// <param name="projectId">The id of the project the story belongs to</param>
    /// <param name="storyId">The id of the story to add the note to</param>
    /// <param name="data">The attachment to add to the story</param>
    /// <returns>The text that was added</returns>
    public static string AddAttachment(PivotalUser user, int projectId, int storyId, byte[] data)
    {
      string url = String.Format("{0}/projects/{1}/stories/{2}/attachments?token={3}", PivotalService.BaseUrl, projectId, storyId, user.ApiToken);
      //System.Net.WebClient client = new System.Net.WebClient();
      //client.UploadData(url, data);
      
      XmlDocument response = PivotalService.SubmitData(url, data, ServiceMethod.POST, null);
      PivotalAttachment addedAttachment = SerializationHelper.DeserializeFromXmlDocument<PivotalAttachment>(response);
      return addedAttachment.Status;
    }

    

    #endregion
  }
}
