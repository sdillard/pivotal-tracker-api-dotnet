using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace PivotalTrackerAPI.Util
{
  // Borrowed from http://west-wind.com/weblog/posts/617930.aspx
  class WebEncoding
  {
    /// <summary>
    /// UrlEncodes a string without the requirement for System.Web
    /// </summary>
    /// <param name="text">The string to encode</param>
    /// <returns></returns>
    // [Obsolete("Use System.Uri.EscapeDataString instead")]
    public static string UrlEncode(string text)
    {
      // Sytem.Uri provides reliable parsing
      return System.Uri.EscapeDataString(text);
    }

    /// <summary>
    /// UrlDecodes a string without requiring System.Web
    /// </summary>
    /// <param name="text">String to decode.</param>
    /// <returns>decoded string</returns>
    public static string UrlDecode(string text)
    {
      // pre-process for + sign space formatting since System.Uri doesn't handle it
      // plus literals are encoded as %2b normally so this should be safe
      text = text.Replace("+", " ");
      return System.Uri.UnescapeDataString(text);
    }

    /// <summary>
    /// Retrieves a value by key from a UrlEncoded string.
    /// </summary>
    /// <param name="urlEncoded">UrlEncoded String</param>
    /// <param name="key">Key to retrieve value for</param>
    /// <returns>returns the value or "" if the key is not found or the value is blank</returns>
    public static string GetUrlEncodedKey(string urlEncoded, string key)
    {
      urlEncoded = "&" + urlEncoded + "&";

      int Index = urlEncoded.IndexOf("&" + key + "=", StringComparison.OrdinalIgnoreCase);
      if (Index < 0)
        return "";

      int lnStart = Index + 2 + key.Length;

      int Index2 = urlEncoded.IndexOf("&", lnStart);
      if (Index2 < 0)
        return "";

      return UrlDecode(urlEncoded.Substring(lnStart, Index2 - lnStart));
    }

    

    /// <summary>
    /// HTML-encodes a string and returns the encoded string.
    /// </summary>
    /// <param name="text">The text string to encode. </param>
    /// <returns>The HTML-encoded text.</returns>
    public static string HtmlEncode(string text)
    {
        if (text == null)
            return null;

        StringBuilder sb = new StringBuilder(text.Length);

        int len = text.Length;
        for (int i = 0; i < len; i++)
        {
            switch (text[i])
            {

                case '<':
                    sb.Append("&lt;");
                    break;
                case '>':
                    sb.Append("&gt;");
                    break;
                case '"':
                    sb.Append("&quot;");
                    break;
                case '&':
                    sb.Append("&amp;");
                    break;
                default:
                    if (text[i] > 159)
                    {
                        // decimal numeric entity
                        sb.Append("&#");
                        sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
                        sb.Append(";");
                    }
                    else
                        sb.Append(text[i]);
                    break;
            }
        }
        return sb.ToString();
    }

  }
}
