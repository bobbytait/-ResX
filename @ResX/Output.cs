using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _ResX
{
    public static class Output
    {
        private const char FAKE_CR = '↵';

        private static void WriteOutput(string output)
        {
            Console.WriteLine(output);
        }

        public static void Info(string message)
        {
            WriteOutput(message);
        }

        public static bool Error(string message, Exception e = null)
        {
            WriteOutput(" [ERROR] " + message + ((e != null) ? "\n" + e.Message + "\n" + e.StackTrace : ""));
            return false;
        }

        public static bool OutputResults()
        {
            DataView dataView;
            string id, newVal, oldVal;

            try
            {
                // Filter on duplicate string IDs & sort
                if (Settings.IsReportDuplicateIds)
                {
                    dataView = StringResourceTable.GetDataView(CompareResult.StringIdsDuplicated);
                    Info(String.Format("\nResX Duplicate String IDs: {0} ----------------------------------------\n", dataView.Count));
                    foreach (DataRowView item in dataView)
                    {
                        Info(String.Format("  {0}", item["ID"].ToString()));
                    }
                }

                // Filter on mismatched strings & sort
                if (Settings.IsReportMismatches)
                {
                    dataView = StringResourceTable.GetDataView(CompareResult.StringMismatch);
                    Info(String.Format("\n\nResX Default String Mismatches: {0} ----------------------------------------", dataView.Count));
                    foreach (DataRowView item in dataView)
                    {
                        id = item["ID"].ToString();
                        newVal = item["new"].ToString().Replace('\n', FAKE_CR).Replace('\r', FAKE_CR);
                        oldVal = item["old"].ToString().Replace('\n', FAKE_CR).Replace('\r', FAKE_CR);
                        Info(String.Format("\n  {0}\n    New: {1}\n    Old: {2}", id, newVal, oldVal));
                    }
                }

                // Filter on empty strings & sort
                if (Settings.IsReportEmptyStrings)
                {
                    dataView = StringResourceTable.GetDataView(CompareResult.StringsEmpty);
                    Info(String.Format("\n\nResX Default Empty Strings: {0} ----------------------------------------\n", dataView.Count));
                    foreach (DataRowView item in dataView)
                    {
                        Info(String.Format("  {0}", item["ID"].ToString()));
                    }
                }

                // Filter on deleted strings & sort
                if (Settings.IsReportDeletes)
                {
                    dataView = StringResourceTable.GetDataView(CompareResult.StringDeleted);
                    Info(String.Format("\n\nResX Default String Deletions: {0} ----------------------------------------\n", dataView.Count));
                    foreach (DataRowView item in dataView)
                    {
                        id = item["ID"].ToString();
                        oldVal = item["old"].ToString().Replace('\n', FAKE_CR).Replace('\r', FAKE_CR);
                        Info(String.Format("  {0}  //  {1}", id, oldVal));
                    }
                }

                // Filter on added strings & sort
                if (Settings.IsReportAdds)
                {
                    dataView = StringResourceTable.GetDataView(CompareResult.StringAdded);
                    Info(String.Format("\n\nResX Default String Additions: {0} ----------------------------------------\n", dataView.Count));
                    foreach (DataRowView item in dataView)
                    {
                        id = item["ID"].ToString();
                        newVal = item["new"].ToString().Replace('\n', FAKE_CR).Replace('\r', FAKE_CR);
                        Info(String.Format("  {0}  //  {1}", id, newVal));
                    }
                }

                // Filter on matching strings & sort
                if (Settings.IsReportMatches)
                {
                    dataView = StringResourceTable.GetDataView(CompareResult.StringMatch);
                    Info(String.Format("\n\nResX String Matches: {0} ----------------------------------------\n", dataView.Count));
                    foreach (DataRowView item in dataView)
                    {
                        id = item["ID"].ToString();
                        newVal = item["new"].ToString().Replace('\n', FAKE_CR).Replace('\r', FAKE_CR);
                        Info(String.Format("  {0}  //  {1}", id, newVal));
                    }
                }
            }
            catch (Exception e)
            {
                Error("Occurred outputting results", e);
            }

            return true;
        }
    }
}
