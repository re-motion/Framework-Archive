using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using log4net;

namespace Rubicon.Xml
{
  public class XmlSchemaValidationHandler
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (XmlSchemaValidationHandler));

    private string _context;
    private IXmlLineInfo _lineInfo;
    private List<XmlSchemaValidationErrorInfo> _messages = new List<XmlSchemaValidationErrorInfo>();
    private int _warnings = 0;
    private int _errors = 0;
    private XmlSchemaValidationErrorInfo _firstError = null;

    public XmlSchemaValidationHandler (string context)
    {
      _context = context;
    }

    public int Warnings
    {
      get { return _warnings; }
    }

    public int Errors
    {
      get { return _errors; }
    }

    public ValidationEventHandler Handler
    {
      get { return new ValidationEventHandler (HandleValidation); }
    }

    private void HandleValidation (object sender, ValidationEventArgs args)
    {
      // WORKAROUND: known bug in .NET framework
      if (args.Message.IndexOf ("http://www.w3.org/XML/1998/namespace:base") >= 0)
      {
        s_log.DebugFormat (
            "Ignoring the following schema validation error in {0}, because it is considered a known .NET framework bug: {1}",
            _context,
            args.Message);
        return;
      }

      XmlSchemaValidationErrorInfo errorInfo = new XmlSchemaValidationErrorInfo (args.Message, _context, _lineInfo, args.Severity);
      _messages.Add (errorInfo);

      if (args.Severity == XmlSeverityType.Error)
      {
        s_log.Error (errorInfo);
        if (_firstError == null)
          _firstError = errorInfo;
        ++ _errors;
      }
      else
      {
        s_log.Warn (errorInfo);
        ++ _warnings;
      }
    }

    public void EnsureNoErrors()
    {
      if (_errors > 0)
      {
        throw new XmlSchemaValidationException (
          string.Format (
              "Schema verification failed with {0} errors and {1} warnings in '{2}'. First error: {3}", 
              _errors, 
              _warnings, 
              _context, 
              _firstError.ErrorMessage), 
          null,
          _firstError.LineNumber, 
          _firstError.LinePosition);
      }
    }
  }
}