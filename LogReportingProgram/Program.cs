using System;
using System.IO;
using System.Collections.Generic;

namespace LogReportingProgram
{
    class Program
    {
        static string ReplaceChar(string text, string which, string with)
        {
            while (text.Contains(which))
            {
                string afterSpace = text.Substring(text.IndexOf(which) + 1);
                text = text.Remove(text.IndexOf(which)) + with + afterSpace;
            }
            return text;
        }

        static string[] ListAllFilesInLogsDir(string path)
        {
            return Directory.GetFiles(path, "*.log.*");
        }

        static string ExtractDatePartFromFileName(string file)
        {
            string stringDate = file.Substring(file.LastIndexOf(".") + 1);
            return stringDate;
        }

        static string ExtractMonthFromDate(string file)
        {
            string stringDate = file.Substring(4, 2);
            return stringDate;
        }

        static string ExtractYearFromDate(string file)
        {
            string stringDate = file.Substring(0, 4);
            return stringDate;
        }

        static void Main()
        {
            var path = Directory.GetCurrentDirectory() + @"\logs";
            var all_files = ListAllFilesInLogsDir(path);
            var versions = new[] { "free", "basic", "pro", "edge" };
            var version_strings = new[] { "PioSolverFree2", "PIOSOLVER2-basic", "PIOSOLVER2-pro", "PIOSOLVER2-edge", "NLHEfree", "NLHEbasic", "NLHEpro", "NLHEedge" }; // from "NHLEfree" onward for old reports
            var versions_string_compare = new[] { "free", "basic", "pro", "edge", "free", "basic", "pro", "edge" };///.....

            // klucz to data_wersja
            //wartość to ilość wystąpień
            var results = new SortedDictionary<string, int>();

            foreach (var file in all_files)
            {
                var date = ExtractDatePartFromFileName(file);
                var month = ExtractMonthFromDate(date);
                var years = ExtractYearFromDate(date);
                foreach (var version in versions_string_compare)
                {
                    results[date + "_" + version] = 0;
                    results[years + month + "_" + version] = 0;
                    results[years + "_" + version] = 0;
                }
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    for (int i = 0; i < version_strings.Length; i++)
                    {
                        if (line.Contains(version_strings[i]))
                        {
                            results[date + "_" + versions_string_compare[i]] += 1; //tutaj dodać kod do ogarnięcia przypadku,
                            if (date.Substring(0, 4) == ExtractYearFromDate(date))
                            {
                                results[years + "_" + versions_string_compare[i]] += 1;
                            }
                            if (date.Substring(4, 2) == ExtractMonthFromDate(date))
                            {
                                results[years + month + "_" + versions_string_compare[i]] += 1;
                            }
                        }
                    }
                }

            }


            string dateNow = DateTime.Now.ToString();
            dateNow = ReplaceChar(dateNow, " ", "_");
            dateNow = ReplaceChar(dateNow, ":", "");
            dateNow = ReplaceChar(dateNow, ".", "-");

            var savePath = "report" + dateNow + ".csv";
            using (StreamWriter sw = new StreamWriter(savePath))
            {
                foreach (var version in versions)
                {
                    sw.Write(version + ";");
                }
                sw.WriteLine("");

                foreach (var kvp in results)
                {
                    foreach (var version in versions)
                    {
                        if (kvp.Key.Contains(version))
                        {
                            sw.Write(kvp.Value + ";");
                            sw.WriteLine("");
                        }
                    }
                }
            }
        }
    }
}

