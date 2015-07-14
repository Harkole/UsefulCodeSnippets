using System;
using System.IO;
using System.Reflection;

namespace LogFileManager
{
    class Logging
    {

        // File writer
        private StreamWriter sw = null;
        private string logFileLoc = null;

        /*
         * Create the Event Source (if required). 
         * Create a header to the event log file for each time the service is started.
         */ 
        public Logging(string app, string logPath = "")
        {
            // If the path to the log file has not been passed in use the location of the exe
            if (logPath == "")
            {
                logPath = Assembly.GetExecutingAssembly().Location; // Directory path only
                logPath += "ClearBackupLog.txt";                    // adds the file name
            }

            FileInfo file = new FileInfo(logPath);

            // Check to see if the log file is getting too large (1 Megabyte)
            if (file.Length > 1000000)
            {
                // Move existing (large file) to an archived file and start a new log
                
                // Build new file name using the date
                DateTime date = new DateTime();
                date = DateTime.Today;
                string archivePath = Assembly.GetExecutingAssembly().Location;
                archivePath += "ClearOldBackups_Archived_" + date.ToString();

                file.CopyTo(archivePath);
                sw = new StreamWriter(logPath, false);
            }
            else // continue adding to file
            {
                sw = new StreamWriter(logPath, true);
            }

            try
            {
                // Write to the log stipulating the application was started
                sw.WriteLine("Starting " + app);
                sw.Close();
            }
            catch
            {
                // Silent fail
                // This prevents the program terminating for failing to write
                // to the log file.
            }

            // Set the log file location so we can use it for adding to the file in other methods
            logFileLoc = logPath;
        }

        /*
         * Provide a method for adding a new even to the log file
         */ 
        public void AddEvent(string logEvent)
        {
            try // write to the log file
            {
                sw = new StreamWriter(logFileLoc, true);
                sw.WriteLine(logEvent);
                sw.Close();
            }
            catch
            {
                // Do nothing as we have no log file to write to!
                // This prevents the program terminating for failing to write
                // to the log file.
            }
        }
    }
}
