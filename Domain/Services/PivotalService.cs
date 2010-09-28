using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PivotalTrackerAPI.Util;
using PivotalTrackerAPI.Domain.Enumerations;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PivotalTrackerAPI.Domain.Services
{
  /// <summary>
  /// The core class used to communicate with Pivotal.  This class is more of a helper to ensure that a method in a model has a consistent way to get data from Pivotal
  /// </summary>
  public static class PivotalService
  {
    /// <summary>
    /// Url for the services (assumes http)
    /// </summary>
    public const string BaseUrl = "http://www.pivotaltracker.com/services/v3";
    /// <summary>
    /// Url for the services (assumes https)
    /// </summary>
    public const string BaseUrlHttps = "https://www.pivotaltracker.com/services/v3";
    
    /// <summary>
    /// Generic method to post data to the API
    /// </summary>
    /// <remarks>This method does not catch errors, so the caller needs to handle any failures to communicate with Pivotal</remarks>
    /// <param name="url">Url to submit to</param>
    /// <param name="data">Xml data as a string</param>
    /// <returns>response from Pivotal API</returns>
    public static XmlDocument SubmitData(string url, string data, ServiceMethod method)
    {
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

      request.ContentType = "application/xml";
      //request.ContentType = "application/x-www-form-urlencoded";
      request.Method = "POST";
      byte[] byteArray = Encoding.UTF8.GetBytes(data);
      request.ContentLength = byteArray.Length;

      Stream dataStream = null;
      try
      {
        dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
      }
      finally
      {
        if (dataStream != null)
          dataStream.Close();
      }
      HttpWebResponse response = null;
      StreamReader reader = null;
      XmlDocument xmlDoc = new XmlDocument();
      try
      {
        response = request.GetResponse() as HttpWebResponse;
        reader = new StreamReader(response.GetResponseStream());
        xmlDoc.Load(reader);
      }
      finally
      {
        if (response != null)
          response.Close();
        if (reader != null)
          reader.Close();
      }
      
      return xmlDoc;
    }

    /// <summary>
    /// Generic method for getting data from the Pivotal API and returning the response
    /// </summary>
    /// <param name="url">The url to submit to</param>
    /// <returns>The response from Pivotal</returns>
    public static XmlDocument GetData(string url)
    {
      var uri = new Uri(url);
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

      Stream objStream;
      objStream = request.GetResponse().GetResponseStream();

      StreamReader objReader = new StreamReader(objStream);

      XmlDocument xmlDoc = new XmlDocument();

      xmlDoc.Load(objReader);
      return xmlDoc;
    }

    /// <summary>
    /// Generic method for getting data from the Pivotal API and returning the response but uses Basic Auth
    /// </summary>
    /// <param name="url">The url to submit to</param>
    /// <param name="login">The user's login</param>
    /// <param name="password">The user's password</param>
    /// <returns>The response from Pivotal</returns>
    public static XmlDocument GetDataWithCredentials(string url, string login, string password)
    {
      var uri = new Uri(url);
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
      request.Credentials = new NetworkCredential(login, password);
      Stream objStream;
      objStream = request.GetResponse().GetResponseStream();

      StreamReader objReader = new StreamReader(objStream);

      XmlDocument xmlDoc = new XmlDocument();

      xmlDoc.Load(objReader);
      return xmlDoc;
    }

    public static string CleanXmlForSubmission(XmlDocument xml, string rootXpath, string[] ignoredNodes, bool removeIfHasAttributes)
    {
      XmlNode toRemove = null;
      XmlNode rootNode = xml.SelectSingleNode(rootXpath);
      foreach (string s in ignoredNodes)
      {
        toRemove = xml.SelectSingleNode(rootXpath + s);
        if (toRemove != null)
          rootNode.RemoveChild(toRemove);
        //xml.RemoveChild(toRemove);
      }
      // The only way a node in the story would have an attribute is if it is null
      if (removeIfHasAttributes)
      {
        XmlNodeList emptyNodes = rootNode.ChildNodes;
        int nodeCount = emptyNodes.Count;
        for (int i = 0; i < nodeCount; i++)
        {
          if (emptyNodes[i] != null && emptyNodes[i].Attributes.Count > 0)
          {
            rootNode.RemoveChild(emptyNodes[i]);
            i--;
          }
          if (i >= emptyNodes.Count)
            break;
        }
        rootNode.Attributes.RemoveAll();
      }

      return xml.DocumentElement.OuterXml;
    }
    


  }
}
