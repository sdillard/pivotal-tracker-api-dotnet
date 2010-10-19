using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace PivotalTrackerAPI.Util
{
  // Based on this: http://rhyous.com/2010/04/29/generic-xml-serializer-class-for-c-and-an-xml-serialization-usage-example/
  /// <summary>
  /// Helper class for serializing objects to various formats
  /// </summary>
  public static class SerializationHelper
  {
    /// <summary>
    /// Serializes an object of type T to a binary file
    /// </summary>
    /// <typeparam name="T">The Type of object</typeparam>
    /// <param name="t">The object to serialize</param>
    /// <param name="inFilename">File to save</param>
    public static void SerializeToBinaryFile<T>(T t, String inFilename)
    {
      Stream s = null;
      IFormatter f = new BinaryFormatter();
      try
      {
        s = new FileStream(inFilename, FileMode.Create);
        f.Serialize(s, t);
      }
      finally
      {
        if (s != null)
          s.Close();
      }
    }

    /// <summary>
    /// Deserializes an object of type T from a binary file
    /// </summary>
    /// <typeparam name="T">The Type of object</typeparam>
    /// <param name="inFilename">File to read</param>
    public static T DeserializeFromBinaryFile<T>(String inFilename)
    {
      Stream s = null;
      IFormatter f = new BinaryFormatter();
      T retVal = default(T);
      try
      {
        s = new FileStream(inFilename, FileMode.Open);
        retVal = (T)f.Deserialize(s);
      }
      finally
      {
        if (s != null)
          s.Close();
      }
      return retVal;
    }

    /// <summary>
    /// Serializes an object of type T to an Xml file
    /// </summary>
    /// <typeparam name="T">The Type of object</typeparam>
    /// <param name="t">The object to serialize</param>
    /// <param name="inFilename">File to save</param>
    public static void SerializeToXmlFile<T>(T t, String inFilename)
    {
      XmlSerializer serializer = new XmlSerializer(t.GetType());
      TextWriter textWriter = new StreamWriter(inFilename);
      try
      {
        serializer.Serialize(textWriter, t);
      }
      finally
      {
        if (textWriter != null)
          textWriter.Close();
      }
    }
    /// <summary>
    /// Serializes an Xml file to an instance object of type T
    /// </summary>
    /// <typeparam name="T">The Type of object</typeparam>
    /// <param name="inFilename">File to read</param>
    public static T DeserializeFromXmlFile<T>(String inFilename)
    {
      XmlSerializer deserializer = new XmlSerializer(typeof(T));
      TextReader textReader = new StreamReader(inFilename);
      T retVal = default(T);
      try
      {
        retVal = (T)deserializer.Deserialize(textReader);
      }
      finally
      {
        if (textReader != null)
          textReader.Close();
      }
      return retVal;
    }

    /// <summary>
    /// Serializes an object of type T to an Xml string
    /// </summary>
    /// <typeparam name="T">The Type of object</typeparam>
    /// <param name="t">The object to serialize</param>
    public static string SerializeToXmlString<T>(T t)
    {
      XmlSerializer serializer = new XmlSerializer(t.GetType());
      StringBuilder sb = new StringBuilder();
      StringWriter stringWriter = new StringWriter(sb);
      try
      {
        serializer.Serialize(stringWriter, t);
      }
      finally
      {
        if (stringWriter != null)
          stringWriter.Close();
      }
      return sb.ToString();
    }

    /// <summary>
    /// Serializes an Xml string to an instance object of type T
    /// </summary>
    /// <typeparam name="T">The Type of object</typeparam>
    /// <param name="xmlString">string to read</param>
    public static T DeserializeFromXmlString<T>(String xmlString)
    {
      XmlSerializer deserializer = new XmlSerializer(typeof(T));
      StringReader stringReader = new StringReader(xmlString);
      T retVal = default(T);
      try
      {
        retVal = (T)deserializer.Deserialize(stringReader);
      }
      finally
      {
        if (stringReader != null)
          stringReader.Close();
      }
      return retVal;
    }

    /// <summary>
    /// Serializes an object of type T to an XmlDocument
    /// </summary>
    /// <typeparam name="T">The Type of object</typeparam>
    /// <param name="t">The object to serialize</param>
    public static XmlDocument SerializeToXmlDocument<T>(T t)
    {
      XmlSerializer serializer = new XmlSerializer(t.GetType());
      XmlDocument xmlDoc = new XmlDocument();
      StringBuilder sb = new StringBuilder();
      StringWriter stringWriter = new StringWriter(sb);
      try
      {
        serializer.Serialize(stringWriter, t);
      }
      finally
      {
        if (stringWriter != null)
          stringWriter.Close();
      }
      xmlDoc.LoadXml(sb.ToString());
      return xmlDoc;
    }

    /// <summary>
    /// Serializes an XmlDocument to an instance object of type T
    /// </summary>
    /// <typeparam name="T">The Type of object</typeparam>
    /// <param name="xmlDocument">document to read</param>
    public static T DeserializeFromXmlDocument<T>(XmlDocument xmlDocument)
    {
      XmlSerializer deserializer = new XmlSerializer(typeof(T));
      XmlNodeReader nodeReader = new XmlNodeReader(xmlDocument.DocumentElement);
      T retVal = default(T);
      try
      {
        retVal = (T)deserializer.Deserialize(nodeReader);
      }
      finally
      {
        if (nodeReader != null)
          nodeReader.Close();
      }
      return retVal;
    }

    /// <summary>
    /// Uses in-memory serialization to create an identical copy of the source object's properties
    /// </summary>
    /// <returns>A new instance of the item with the same properties</returns>
    public static T Clone<T>(T t)
    {
      XmlDocument doc = SerializeToXmlDocument<T>(t);
      T clone = SerializationHelper.DeserializeFromXmlDocument<T>(doc);
      return clone;
    }

  }
}
