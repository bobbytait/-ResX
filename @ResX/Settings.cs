using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _ResX
{
    public static class Settings
    {
        public const string DEFAULT_RESX_FILENAME = "Resources.resx";

        // ResX file in base language from earlier release that we want to compare to
        public static string OldSourceResxFile = null;

        // ResX file in base language from current release
        public static string NewSourceResxFile = null;

        // If true, output a report on this type of finding
        // (Not fully implemented)
        public static bool IsReportDuplicateIds = true;
        public static bool IsReportMismatches = true;
        public static bool IsReportEmptyStrings = true;
        public static bool IsReportAdds = true;
        public static bool IsReportDeletes = true;
        public static bool IsReportMatches = false;

        // If true, after running, waits for a keypress to return to caller
        public static bool IsWaitForKeypressOnFinish = false;

        // If true, return a non-zero error code to the caller when a discrepency is encountered
        // (Not fully implemented)
        public static bool IsReturnFailureOnDiscrepency = false;


        public static bool ProcessArgs(string[] args)
        {
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string argName = args[i].ToLower();

                    // Specify path & filename of ResX file in base language from earlier release that we want to compare to
                    if (argName == "/oldresx")
                    {
                        OldSourceResxFile = args[++i];

                        if ((OldSourceResxFile == null) || (OldSourceResxFile == String.Empty) || OldSourceResxFile.StartsWith("/"))
                        {
                            return Output.Error(String.Format("Invalid /oldresx: '{0}'", OldSourceResxFile));
                        }

                        if (!File.Exists(Path.Combine(OldSourceResxFile)))
                        {
                            return Output.Error(String.Format("Missing /oldresx: '{0}'", OldSourceResxFile));
                        }

                        continue;
                    }

                    // Specify path & filename of ResX file in base language from current release
                    if (argName == "/newresx")
                    {
                        NewSourceResxFile = args[++i];

                        if ((NewSourceResxFile == null) || (NewSourceResxFile == String.Empty) || NewSourceResxFile.StartsWith("/"))
                        {
                            return Output.Error(String.Format("Invalid /newresx: '{0}'", NewSourceResxFile));
                        }

                        if (!File.Exists(Path.Combine(NewSourceResxFile)))
                        {
                            return Output.Error(String.Format("Missing /newresx: '{0}'", NewSourceResxFile));
                        }

                        continue;
                    }

                    // After running, waits for a keypress to return to comnmand prompt
                    if (argName == "/wait")
                    {
                        IsWaitForKeypressOnFinish = true;
                        continue;
                    }
                }

                if (OldSourceResxFile == null)
                {
                    OldSourceResxFile = Path.Combine(Environment.CurrentDirectory, DEFAULT_RESX_FILENAME + ".old");
                    if (!File.Exists(Path.Combine(OldSourceResxFile)))
                    {
                        return Output.Error(String.Format("Missing /oldresx: '{0}'", OldSourceResxFile));
                    }
                }

                if (NewSourceResxFile == null)
                {
                    NewSourceResxFile = Path.Combine(Environment.CurrentDirectory, DEFAULT_RESX_FILENAME);
                    if (!File.Exists(Path.Combine(NewSourceResxFile)))
                    {
                        return Output.Error(String.Format("Missing /newresx: '{0}'", NewSourceResxFile));
                    }
                }

                if (OldSourceResxFile == NewSourceResxFile)
                {
                    return Output.Error(String.Format("/oldresx and /newresx values cannot be the same: {0}", NewSourceResxFile));
                }

            }
            catch (Exception e)
            {
                return Output.Error("Occurred processing command-line parameters", e);
            }

            return true;
        }

        public static string GetNewSourceResxPath()
        {
            return Path.GetDirectoryName(NewSourceResxFile);
        }
    }
}
