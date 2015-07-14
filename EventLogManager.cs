using System.Diagnostics;

namespace EventLogManager
{
    class Logging
    {

        private EventLog eLog;

        /*
         * Create the Event Source (if required). 
         * Create a header to the event log file for each time the service is started.
         */ 
        public Logging(string eventLogName, string eventLogShort)
        {
            // If the Event source does not exist create it
            if (!EventLog.SourceExists(eventLogName))
            {
                EventLog.CreateEventSource(eventLogName, eventLogShort);
            }

            // Set up the event object
            eLog = new EventLog();
            eLog.Source = eventLogName;

            // Write to the event log stipulating the service was started (starting ID 100)
            eLog.WriteEntry(eventLogName + " has been started.", EventLogEntryType.Information, 100);

        }

        /*
         * Provide methods to add messages to the event log, the basic method must take a string
         * that will be used to fill in the even message, this will always be an information event type
         * with an EventID of 300. The second method allows for more control over the event and requires an EventLogEntryType
         * and integer to use as the event ID.
         */ 
        public void AddEvent(string logEvent)
        {
            // Write the Event as an information type with ID 300
            eLog.WriteEntry(logEvent, EventLogEntryType.Information, 300);
        }

        public void AddEvent(string logEvent, EventLogEntryType logType, int eventID)
        {
            // Write a specific event log, event type and ID
            eLog.WriteEntry(logEvent, logType, eventID);
        }
    }
}
