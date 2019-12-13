using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Utilities;
using Boo.Lang;
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
            try
            {
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
                    sw.WriteLine(finalString);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void AppendAnglesToReport(System.Collections.Generic.List<Angles> angles)
        {
            VerfyDirectory();
            VerfiyFile();
            try
            {
                using (StreamWriter sw = File.AppendText(GetFilePath()))
                {
                    var finalString = "";
                    foreach (var angle in angles)
                    {
                        finalString += angle.LeftAngle;
                        finalString += _reportSeperator;
                        finalString += angle.RightAngle;
                        finalString += _reportSeperator;
                        finalString += angle.TimeStamp;

                        sw.WriteLine(finalString);
                        finalString = "";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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

        #endregion
    }
}

