using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PivotalTrackerAPI.Domain.Enumerations
{
  /// <summary>
  /// Methods for communicating with the server
  /// </summary>
  public enum ServiceMethod
  {
    /// <summary>
    /// GET request
    /// </summary>
    GET,
    /// <summary>
    /// POST request
    /// </summary>
    POST,
    /// <summary>
    /// PUT request
    /// </summary>
    PUT,
    /// <summary>
    /// DELETE request
    /// </summary>
    DELETE
  }
}
