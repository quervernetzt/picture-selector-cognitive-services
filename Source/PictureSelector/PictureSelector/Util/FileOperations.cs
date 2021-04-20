using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace PictureSelector.Util
{
    public static class FileOperations
    {
        /// <summary>
        ///     Get all paths of files within a given directory path.
        /// </summary>
        /// <param name="directoryFullPath">
        ///     Full path of the directory.
        /// </param>
        /// <param name="allowedFileFormats">
        ///     List of allowed files formats. If not provided or empty list provided all file formats are allowed.
        ///     E.g. ["jpg"]
        /// </param>
        /// <returns>
        ///     Returns an array with the file paths.
        /// </returns>
        public static List<string> GetFilePaths(string directoryFullPath, List<string> allowedFileFormats = null)
        {
            List<string> filePaths = Directory.GetFiles(directoryFullPath).ToList<string>();

            if (allowedFileFormats != null && allowedFileFormats.Count > 0)
            {
                filePaths = filePaths.Where(f => allowedFileFormats.Contains(f.Split(".")[^1])).ToList<string>();
            }

            return filePaths;
        }

        /// <summary>
        ///     Write the results to the result CSV.
        /// </summary>
        /// <param name="resultsFullCSVFilePath">
        ///     The full path to the CSV file.
        /// </param>
        /// <param name="resultsCSVSeparator">
        ///     The separator used in the result CSV.
        /// </param>
        /// <param name="selectorMethod">
        ///     The selector method.
        /// </param>
        /// <param name="iteration">
        ///     The number of iteration to select a picture.
        /// </param>
        /// <param name="resultPathsInOrder">
        ///     The result paths in order.
        /// </param>
        /// <param name="isInit">
        ///     Indicator if this is the first iteration what requires a new CSV file.
        ///     Default is false.
        /// </param>
        public static void WriteResultsCSV(
            string resultsFullCSVFilePath,
            string resultsCSVSeparator,
            string selectorMethod,
            int iteration,
            List<string> resultPathsInOrder,
            bool isInit = false)
        {
            // Create CSV file if it does not exist
            bool csvExists = File.Exists(resultsFullCSVFilePath);
            if (csvExists)
            {
                if (isInit)
                {
                    File.Delete(resultsFullCSVFilePath);
                    string headerRow = $"Method{ resultsCSVSeparator }Iteration{ resultsCSVSeparator }Place{ resultsCSVSeparator }Path{ Environment.NewLine }";
                    File.WriteAllText(resultsFullCSVFilePath, headerRow);
                }
            }
            else
            {
                if (isInit)
                {
                    string headerRow = $"Method{ resultsCSVSeparator }Iteration{ resultsCSVSeparator }Place{ resultsCSVSeparator }Path{ Environment.NewLine }";
                    File.WriteAllText(resultsFullCSVFilePath, headerRow);
                }
                else
                {
                    throw new Exception("Result CSV file should exist...");
                }
            }

            // Write results to csv
            List<string> resultsRows = new List<string>();
            for (int i = 0; i < resultPathsInOrder.Count; i++)
            {
                resultsRows.Add($"{ selectorMethod }{ resultsCSVSeparator }{ iteration }{ resultsCSVSeparator }{ i + 1 }{ resultsCSVSeparator }{ resultPathsInOrder[i] }");
            }
            File.AppendAllLines(resultsFullCSVFilePath, resultsRows);
        }

        /// <summary>
        ///     Write result to a folder.
        /// </summary>
        /// <param name="fullResultPath">
        ///     The full path to the result folder.
        /// </param>
        /// <param name="resultPaths">
        ///     The paths to the results to be copied.
        /// </param>
        public static void WriteResultsToFolder(string fullResultPath, List<string> resultPaths)
        {
            if (Directory.Exists(fullResultPath))
            {
                Directory.Delete(fullResultPath, true);
            }
            Directory.CreateDirectory(fullResultPath);

            for (int i = 1; i <= resultPaths.Count; i++)
            {
                string resultPath = resultPaths[i-1];
                string fileName = resultPath.Split(Path.DirectorySeparatorChar).Last();
                File.Copy(resultPath, $"{ fullResultPath }{ Path.DirectorySeparatorChar }{ i }-{ fileName }");
            }
        }

        /// <summary>
        ///     Display results with a provided program.
        /// </summary>
        /// <param name="resultsPathsOrdered">
        ///     The resulting file paths ordered.
        /// </param>
        /// <param name="displayProgramPath">
        ///     The path to the exe to open the files with.
        /// </param>
        public static void DisplayResults(List<string> resultsPathsOrdered, string displayProgramPath)
        {
            foreach (string resultPath in resultsPathsOrdered)
            {
                Process processInstance = Process.Start(displayProgramPath, resultPath);
                Thread.Sleep(5000);
                processInstance.Kill();
            }
        }
    }
}
