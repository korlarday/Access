using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.GeneralUtils
{
    public static class Logs
    {
        /// <summary>
        /// loggin configuration
        /// </summary> 
        public enum LOGGER_OPTION
        {
            ACTIVATE_SELF_DEBUG,
            THROW_EXCEPTIONS
        }

        static object _Loglocker = new object();
        static string _LogPath { get; set; }
        static LOGGER_OPTION _Option = LOGGER_OPTION.ACTIVATE_SELF_DEBUG;

        public static Func<string, Task> ErrorMonitor { get; set; }
        static CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-US");

        public static void ConfigLogger(string Path, LOGGER_OPTION Option)
        {
            _LogPath = Path;
            _Option = Option;
        }

        public static void logError(string NameSpaces, string errLog, string method, Exception ex = null)
        {
            lock (_Loglocker)
            {
                string TempPath;

                TempPath = @"C:\Allprimetech\";
                //TempPath = (_LogPath == string.Empty) ? TempPath : _LogPath + @"\";
                if (!Directory.Exists(TempPath)) Directory.CreateDirectory(TempPath);

                Assembly ass = Assembly.GetCallingAssembly();
                if (ass != null) errLog = ass.GetName().Name + ", " + ass.GetName().Version.ToString() + ": " + errLog;

                errLog = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss:ffff") + " " + NameSpaces + "------>" + method + " : " + errLog;
                errLog += Environment.NewLine + "__________________________" + Environment.NewLine + Environment.NewLine;

                if (ex != null)
                    errLog += Environment.NewLine + "------>" + ex.InnerException?.InnerException?.Message;

                string pth = TempPath + DateTime.Today.ToString("yyyy.MM.dd") + ".log";

                ErrorMonitor?.Invoke(errLog);

                if (_Option == LOGGER_OPTION.ACTIVATE_SELF_DEBUG)
                    File.AppendAllText(pth, errLog);
                else
                    throw new Exception(errLog);
            }

        }
    }
}
