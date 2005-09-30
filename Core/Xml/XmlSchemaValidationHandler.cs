using System;
using System.Xml;
using System.Xml.Schema;
using log4net;

namespace Rubicon.Xml
{
public class XmlSchemaValidationHandler
{
  private static readonly ILog s_log = LogManager.GetLogger (typeof (XmlSchemaValidationHandler));

  private string _context;
  private int _warnings = 0;
  private int _errors = 0;
  private string _firstError;

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
    get { return new ValidationEventHandler (this.HandleValidation); }
  }

  private void HandleValidation (object sender, ValidationEventArgs args)
  {
    // WORKAROUND: known bug in .NET framework
    if (args.Message.IndexOf ("http://www.w3.org/XML/1998/namespace:base") >= 0)
    {
      s_log.Debug (string.Format ("Ignoring the following schema validation error in {0}, because it is considered a known .NET framework bug: {1}", _context, args.Message));
      return; 
    }

    if (args.Severity == XmlSeverityType.Error)
    {
      s_log.Error (string.Format ("Schema validation error in {0}: {1}", _context, args.Message));
      if (_firstError == null)
        _firstError = args.Message;
      ++ _errors;
    }
    else
    {
      s_log.Warn (string.Format ("Schema validation warning in {0}: {1}", _context, args.Message));
      ++ _warnings;
    }
  }

  public void EnsureNoErrors ()
  {
    if (_errors > 0)
    {
      throw new XmlSchemaException (
          string.Format ("Schema verification failed with {0} errors and {1} warnings in \"{2}\". First error: {3}", 
              _errors, _warnings, _context, _firstError),
          null);
    }
  }
}

}
