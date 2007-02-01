using System;
using System.Diagnostics;
using Rubicon.Utilities;

namespace Rubicon.Logging
{
  public class Log4NetTraceListener : TraceListener
  {
    // With each method using that param ,possibly param descr: eventCache is ignored.
    // NOTE: FOR DOCU :eventCache is ignored, as (most of) the information is available in Log4Net anyway.
    //      hence TraceOptions are not relevant, too.

    public static LogLevel Convert (TraceEventType eventType)
    {
      switch (eventType)
      {
        case TraceEventType.Verbose:
          return LogLevel.Debug;
        case TraceEventType.Information:
          return LogLevel.Info;
        case TraceEventType.Warning:
          return LogLevel.Warn;
        case TraceEventType.Error:
          return LogLevel.Error;
        case TraceEventType.Critical:
          return LogLevel.Fatal;
        default:
          throw new ArgumentException (string.Format ("LogLevel does not support value {0}.", eventType), "logLevel");
      }
    }

    private Log4NetLogManager _logManager = new Log4NetLogManager ();

    public Log4NetTraceListener()
    {
    }

    public Log4NetTraceListener (string name) : base (name)
    {
    }


    /*
     * TODO: Note in Docu that no filtering is done!

       TODO: #### if ((this.Filter == null) || this.Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, message))
     
      Write/WriteLine end up with EventLogEntryType.Information when using EventLogTraceListener.
     */
    public override void Write (string message)
    {
      _logManager.GetLogger (string.Empty).Log (LogLevel.Info, 0, message);
    }

    public override void WriteLine (string message)
    {
      Write (message);
    }

    
    public override void TraceEvent (TraceEventCache eventCache, string source, TraceEventType eventType, int id)
    {
      TraceEvent (eventCache, source, eventType, id, string.Empty, new object[0]);
    }

    public override void TraceEvent (TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
    {
      TraceEvent (eventCache, source, eventType, id, message, new object[0]);
    }

    public override void TraceEvent (
        TraceEventCache eventCache, 
        string source, 
        TraceEventType eventType, int id, 
        string format, 
        params Object[] args)
    {
      if (ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
        _logManager.GetLogger (source).LogFormat (Convert (eventType), id, format, args);
    }


    public override void TraceData (TraceEventCache eventCache, string source, TraceEventType eventType, int id, Object data)
    {
      TraceData (eventCache, source, eventType, id, new object[] { data } );
    }

    public override void TraceData (TraceEventCache eventCache, string source, TraceEventType eventType, int id, params Object[] data)
    {
      if (ShouldTrace (eventCache, source, eventType, id, null, null, null, data)) 
      {
        string message = string.Empty;
        if (data != null)
          message = StringUtility.ConcatWithSeparator(data, ", ");

        _logManager.GetLogger (source).Log (Convert (eventType), id, message);
      }
    }


    /*
    * The TraceTransfer method is used for the correlation of related traces. 
    * The TraceTransfer method calls the TraceEvent method to process the call, 
    * with the eventType level set to Transfer and the relatedActivityIdGuid as 
    * a string appended to the message.
     * 
     * //## INTO REMARKS (or NOTE when multiple entries)
      TraceEventType.Transfer is used in TraceListener.TraceTransfer :

      Like EventLogTraceListener.CreateEventInstance: treat TraceEventType.Transfer as "Information" level.
    */
    public override void TraceTransfer (TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId) 
    {
      TraceEvent (eventCache, source, TraceEventType.Information, id,
          message + ", relatedActivityId=" + relatedActivityId.ToString (), new object[0]);
    }


    private bool ShouldTrace (
        TraceEventCache cache,
        string source,
        TraceEventType eventType,
        int id,
        string formatOrMessage,
        object[] args,
        object data1,
        object[] data)
    {
      return ((Filter == null) || Filter.ShouldTrace (cache, source, eventType, id, formatOrMessage, args, data1, data));
    }
  }
}
