using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Linq;

namespace _ResX
{
    public enum CompareResult
    {
        StringMatch = 0,
        StringMismatch,
        StringAdded,
        StringDeleted,
        StringIdsDuplicated,
        StringsEmpty
    }

    class StringResourceTable
    {
        public static DataTable Table = null;

        public static bool Initialize()
        {
            try
            {
                // Create data table
                Table = new DataTable();
                Table.Columns.Add("ID", typeof(string));
                Table.Columns.Add("result", typeof(Int32));
                Table.Columns.Add("new", typeof(string));
                Table.Columns.Add("old", typeof(string));
            }
            catch (Exception e)
            {
                return Output.Error("Occurred initializing data table", e);
            }

            Output.Info("Data table initialized");
            return true;
        }

        public static bool ImportBaseData()
        {
            if (!ImportOldResxData()) { return false; }
            if (!ImportNewResxData()) { return false; }
            return true;
        }

        public static bool ImportOldResxData()
        {
            try
            {
                // Get data from old default resx file
                //string resxFile = Path.Combine(Settings.OldResxDir, Settings.DEFAULT_OLD_RESX_FILENAME);
                ResXResourceReader reader = new ResXResourceReader(Settings.OldSourceResxFile);
                foreach (DictionaryEntry entry in reader)
                {
                    if (entry.Value is String)
                    {
                        // Get entry's string ID & value
                        string key = entry.Key.ToString();
                        string value = entry.Value.ToString();

                        // Look for string ID in Table
                        DataRow[] foundRows = Table.Select(String.Format("ID = '{0}'", key));
                        if (foundRows.Length == 1)
                        {
                            // If this entry is found in the "new" column, add the value to the row
                            foundRows[0]["old"] = value;
                        }
                        else if (foundRows.Length < 1)
                        {
                            // If not found, add a new row, add this entry's string ID to its ID column and this entry's value to its new column
                            DataRow row = Table.NewRow();
                            row["ID"] = key;
                            row["old"] = value;
                            Table.Rows.Add(row);
                        }
                    }
                }

                reader.Close();
            }
            catch (Exception e)
            {
                return Output.Error("Occurred importing older string resource data", e);
            }

            Output.Info("Old resx data imported");
            return true;
        }

        private static bool ImportNewResxData()
        {
            try
            {
                // Setting the new resx dir as the current directory, since file-based resources can cause exceptions if the source file is not found
                Environment.CurrentDirectory = Settings.GetNewSourceResxPath();

                // Get data from new default resx file
                ResXResourceReader reader = new ResXResourceReader(Settings.NewSourceResxFile);
                DataRow row;
                foreach (DictionaryEntry entry in reader)
                {
                    if (entry.Value is String)
                    {
                        row = Table.NewRow();
                        row["ID"] = entry.Key.ToString();
                        row["result"] = -1;
                        row["new"] = entry.Value.ToString();
                        Table.Rows.Add(row);
                    }
                }

                reader.Close();
            }
            catch (Exception e)
            {
                return Output.Error("Occurred importing newer string resource data", e);
            }

            Output.Info("New resx data imported");
            return true;
        }

        public static int CompareBaseResxData()
        {
            try
            {
                foreach (DataRow row in Table.Rows)
                {
                    // Get entry's string ID & value
                    string key = row["ID"].ToString();
                    string newValue = row["new"].ToString();
                    string oldValue = row["old"].ToString();

                    if ((newValue == String.Empty) && (oldValue == String.Empty))
                    {
                        row["result"] = (int)CompareResult.StringsEmpty;
                    }
                    else if (oldValue == String.Empty)
                    {
                        row["result"] = (int)CompareResult.StringAdded;
                    }
                    else if (newValue == String.Empty)
                    {
                        row["result"] = (int)CompareResult.StringDeleted;
                    }
                    else if (newValue == oldValue)
                    {
                        row["result"] = (int)CompareResult.StringMatch;
                    }
                    else if (newValue != oldValue)
                    {
                        row["result"] = (int)CompareResult.StringMismatch;
                    }
                }

                // A quick hack (for now) to find resource string ID duplicates; Needed since ResXResourceReader simply merges rows with duplicate IDs
                IEnumerable<string> duplicateIds = XDocument.Load(Settings.NewSourceResxFile)
                    .Descendants("data")
                    .GroupBy(g => (string)g.Attribute("name"))
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);
                DataRow duplicateRow;
                foreach (string duplicateId in duplicateIds)
                {
                    duplicateRow = Table.NewRow();
                    duplicateRow["ID"] = duplicateId;
                    duplicateRow["result"] = CompareResult.StringIdsDuplicated;
                    Table.Rows.Add(duplicateRow);
                }
            }
            catch (Exception e)
            {
                Output.Error("Occurred comparing ResX data", e);
                return 1;
            }

            Output.Info("Resx data compared");
            return 0;
        }

        public static DataView GetDataView(CompareResult compareResult)
        {
            try
            {
                string filter = "result = " + ((int)compareResult).ToString();
                string sort = "ID ASC";
                return new DataView(Table, filter, sort, DataViewRowState.CurrentRows);
            }
            catch (Exception e)
            {
                Output.Error("Occurred setting data view", e);
                return null;
            }
        }

        public static class Results
        {

        }
    }
}
