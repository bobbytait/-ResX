using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _ResX
{
    class Program
    {
        static int Main(string[] args)
        {
            if (!Settings.ProcessArgs(args)) { return 1; }

            // Start building a list of resx files, starting our old & new base language files
            List<ResXFile> resxFiles = new List<ResXFile>();
            resxFiles.Add(new ResXFile(Settings.OldSourceResxFile));
            resxFiles.Add(new ResXFile(Settings.NewSourceResxFile));

            Output.Info("Loading & processing resx data...");

            // Initialise our base language comparison table
            if (!StringResourceTable.Initialize()) { return 1; }

            // Import our base language data
            if (!StringResourceTable.ImportBaseData()) { return 1; }

            int testResults = StringResourceTable.CompareBaseResxData();

            // TODO

            // If everything's the same, return success

            // If there are new or updated strings, prepare for translation

            // If there are only deleted strings, prepare for removing those rows

            // Remove deleted rows

            // If translations needed, do them

            // If all went well, display report onscreen (log file option?)

            // Output new spreadsheet





            //--------------------------------------------------------------------------------------

            // Add resx files to be translated to, to our list of resx files
            foreach (string resxFile in Directory.GetFiles(Path.GetDirectoryName(Settings.NewSourceResxFile), "*.resx"))
            {
                resxFiles.Add(new ResXFile(resxFile));
            }

            Console.WriteLine("Loading & processing resx data...");


            int testResults = StringResourceTable.CompareResxData();

            if (!StringResourceTable.OutputResults())
            {
                // Error messages written inside above function
                return 1;
            }

            Output.Info("\nDone!");

            // Finish up
            if (Settings.IsWaitForKeypressOnFinish)
            {
                Console.ReadLine();
            }

            return (Settings.IsReturnFailureOnDiscrepency) ? testResults : 0;
        }
    }
}
