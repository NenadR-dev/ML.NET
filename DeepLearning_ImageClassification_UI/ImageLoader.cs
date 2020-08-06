using DeepLearning_ImageClassification_UI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearning_ImageClassification_UI
{
    public static class ImageLoader
    {
        /// <summary>
        /// Loads Images from file path
        /// </summary>
        /// <param name="folder">file path of images</param>
        /// <param name="useFolderNameAsLabel">use folder name as image label in the predictions</param>
        /// <returns>IEnumerables of ImageData object</returns>
        public static IEnumerable<ImageData> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel = true)
        {
            var files = Directory.GetFiles(folder, "*",
                searchOption: SearchOption.AllDirectories);

            //Searches every directory and loads images from it
            foreach (var file in files)
            {
                if ((Path.GetExtension(file) != ".jpg") && (Path.GetExtension(file) != ".png"))
                    continue;

                var label = Path.GetFileName(file); //uses file name as prediction label if not set to true

                if (useFolderNameAsLabel)
                    label = Directory.GetParent(file).Name;
                else
                {
                    for (int index = 0; index < label.Length; index++)
                    {
                        if (!char.IsLetter(label[index]))
                        {
                            label = label.Substring(0, index);
                            break;
                        }
                    }
                }

                yield return new ImageData()
                {
                    ImagePath = file,
                    Label = label
                };
            }
        }
    }
}
