using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIM_O365_UpdateDefaultMetaData
{
    public class Log
    {
        public enum enumType { Info, Error };

        public string WebUrl { get; set; }
        public Enum LogType { get; set; }
        public string ListTitle { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public string Message { get; set; }

        static List<Log> list = new List<Log>();
        public static void Info(string WebUrl, string ListTitle, string Field, string Value)
        {
            Log log = new Log();
            log.LogType = enumType.Info;
            log.WebUrl = WebUrl;
            log.ListTitle = ListTitle;
            log.Field = Field;
            log.Value = Value;
            list.Add(log);
        }
        public static void Error(string WebUrl, string ListTitle, string Field, string Message)
        {
            Log log = new Log();
            log.LogType = enumType.Error;
            log.WebUrl = WebUrl;
            log.ListTitle = ListTitle;
            log.Field = Field;
            log.Message = Message;
            list.Add(log);
        }
        public static List<Log> GetLog()
        {
            return list;
        }
        public static void ClearLog()
        {
            list.Clear();
        }

    }
}