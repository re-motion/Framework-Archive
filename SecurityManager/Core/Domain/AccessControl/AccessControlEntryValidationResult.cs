using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public class AccessControlEntryValidationResult
  {
    // types

    // static members

    // member fields

    private bool _isValid = true;
    private bool _isSpecificClientMissing = false;

    // construction and disposing

    public AccessControlEntryValidationResult ()
    {
    }

    // methods and properties

    public bool IsValid
    {
      get { return _isValid; }
    }

    public bool IsSpecificClientMissing
    {
      get { return _isSpecificClientMissing; }
    }

    public void SetSpecificClientMissing ()
    {
      _isValid = false;
      _isSpecificClientMissing = true;
    }
  }
}