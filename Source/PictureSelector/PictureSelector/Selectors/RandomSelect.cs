using PictureSelector.Models;
using System;
using System.Collections.Generic;

namespace PictureSelector.Selectors
{
    public class RandomSelect
    {
        /// <summary>
        ///     Select top x images randomly.
        /// </summary>
        /// <param name="filePaths">
        ///     List of file paths to select from.
        /// </param>
        /// <param name="top">
        ///     Number of file paths to select.
        /// </param>
        /// <returns>
        ///     Return the list of top images.
        /// </returns>
        public List<ImageDescriptionExtended> GetTopRandomFiles(List<string> filePaths, int top)
        {
            List<ImageDescriptionExtended> topImages = new List<ImageDescriptionExtended>();
            int numberOfPaths = filePaths.Count;
            if (numberOfPaths < top)
            {
                return topImages;
            }

            Random random = new Random();

            for (int i = 0; i < top; i++)
            {
                int index = random.Next(0, numberOfPaths);
                ImageDescriptionExtended image = new ImageDescriptionExtended();
                image.FilePath = filePaths[index];
                topImages.Add(image);
                filePaths.RemoveAt(index);
                numberOfPaths -= 1;
            }

            return topImages;
        }
    }
}
