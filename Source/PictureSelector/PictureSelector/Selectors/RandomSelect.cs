using System;
using System.Collections.Generic;

namespace PictureSelector.Selectors
{
    public class RandomSelect
    {
        /// <summary>
        ///     Select top x file paths randomly.
        /// </summary>
        /// <param name="filePaths">
        ///     List of file paths to select from.
        /// </param>
        /// <param name="top">
        ///     Number of file paths to select.
        /// </param>
        /// <returns>
        ///     Return the list of top file paths.
        /// </returns>
        public List<string> GetTopRandomFiles(List<string> filePaths, int top)
        {
            List<string> topPaths = new List<string>();
            int numberOfPaths = filePaths.Count;
            if (numberOfPaths < top)
            {
                return topPaths;
            }

            Random random = new Random();

            for (int i = 0; i < top; i++)
            {
                int index = random.Next(0, numberOfPaths);
                topPaths.Add(filePaths[index]);
                filePaths.RemoveAt(index);
                numberOfPaths -= 1;
            }

            return topPaths;
        }
    }
}
