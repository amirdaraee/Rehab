using System.IO;
using UnityEngine;

namespace Data
{
    public static class CSVMananger
    {
        private static string _reportDirectoryName = "Report";
        private static string _reportFileName = "report.csv";
        private static string _reportSeperator = ",";
        private static string[] _reportHeaders = new string[2]
        {
            "Right Hand Angle",
            "Left Hand Angle"
        };
        private static string timeStampHeader = "time.stamp";

        #region Interactions
        public static void AppendToReport(string[] strings)
        {
            VerfyDirectory();
            VerfiyFile();
            using (StreamWriter sw = File.AppendText(GetFilePath()))
            {
                var finalString = "";
                foreach (var str in strings)
                {
                    if (finalString != "")
                    {
                        finalString += _reportSeperator;
                    }

                    finalString += str;
                }

                finalString += GetTimeStamp();
                sw.WriteLine(finalString);

            }
        }
        public static void CreateReport()
        {
            using (StreamWriter sw = File.CreateText(GetFilePath()))
            {
                var finalString = "";
                foreach (var header in _reportHeaders)
                {
                    if (finalString != "")
                    {
                        finalString += _reportSeperator;
                    }
                    finalString += header;
                }

                finalString += _reportSeperator + "/" + timeStampHeader;
                sw.WriteLine(finalString);
            }
        }
        #endregion

        #region Operations
        private static void VerfyDirectory()
        {
            string dir = GetDirectoryPath();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        private static void VerfiyFile()
        {
            string file = GetFilePath();
            if (!File.Exists(file))
            {
                CreateReport();
            }
        }
        #endregion

        #region Queries
        private static string GetDirectoryPath()
        {
            return Application.dataPath + "/" + _reportDirectoryName;
        }
        static string GetFilePath()
        {
            return GetDirectoryPath() + "/" + _reportFileName;
        }
        static string GetTimeStamp()
        {
            return System.DateTime.Now.ToString();
        }  
        #endregion
    }
}

