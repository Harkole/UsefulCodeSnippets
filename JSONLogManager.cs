using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Json_Data_Helper
{
    /// <summary>
    /// Handles the collection and out put of a txt file with
    /// JSON data relating to the current running status of
    /// the local server
    /// </summary>
    class JSONLog
    {
        // Log file set up and file location
        private StreamWriter sw = null;
        private string logFileLoc = null;

        // JSON data must be in Key: Value pairs, this array holds the information until we're ready for it
        private List<jsonObject> jsonData = null;

        /// <summary>
        /// JSON Object to hold the data in key value pairs ready for parsing
        /// </summary>
        private class jsonObject
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// Creates the object that will be used to handle the JSON data
        /// and the file location.
        /// </summary>
        /// <param name="app">Name of the application that created this JSON data set</param>
        /// <param name="logPath">The file location, name and extension to create the output file</param>
        public JSONLog(string app = "", string logPath = "")
        {
            // Check to see if we have a application name
            if (string.IsNullOrEmpty(app))
            {
                app = Assembly.GetExecutingAssembly().FullName;
            }

            // Check to see if we have a log file location, if not use the current executable location
            if (string.IsNullOrEmpty(logPath))
            {
                logPath = Assembly.GetExecutingAssembly().Location;     // Get the location of this executable program
                logPath = Path.GetDirectoryName(logPath);               // Strip out the app name and extension
                logPath += "\\" + app + "_json.txt";                    // add the the name of our txt file
            }

            // Set the location of the file path
            logFileLoc = logPath;

            // Create our JSON data list
            jsonData = new List<jsonObject>();
        }

        /// <summary>
        /// Helper class for collating JSON data on the fly
        /// </summary>
        /// <param name="key">Identity key for the data value</param>
        /// <param name="value">Actual data value to store</param>
        public void AddDataPair(string key, string value)
        {
            // Validate there is both a key and value to work with
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                // Create new object for JSON data
                jsonObject jd = new jsonObject();
                
                // Assign key and value to the object
                jd.Key = key;
                jd.Value = value;

                // Add JSON data pair to the list
                jsonData.Add(jd);
                
            }
            else
            {
                // Throw mising argument exception
                throw new ArgumentNullException("One or more expected parameters were null or empty.");
            }
        }

        /// <summary>
        /// Converts the list of key values in to proper JSON and returns it in string format
        /// but serialised as JSON should be.
        /// </summary>
        /// <returns></returns>
        public void ParseJSON()
        {
            // Add a date time stamp to the out put
            this.AddDataPair("DateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            JavaScriptSerializer json = new JavaScriptSerializer(); // To output the JSON data as a valid string return the json object directly
            sw = new StreamWriter(logFileLoc, false);               // Overwrite the file each time this is called
            sw.Write(json.Serialize(jsonData));                     // Output the data to the file
            sw.Flush();                                             // Flush the data out to ensure we have everything
            sw.Close();                                             // Close the file

            // Clear the list down so that it doesn't grow with each run!
            jsonData.Clear();
        }
    }
}
