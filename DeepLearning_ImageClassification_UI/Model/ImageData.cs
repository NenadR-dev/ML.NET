using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearning_ImageClassification_UI.Model
{
    /// <summary>
    /// Image data class used as a base model of the images we want to process
    /// </summary>
    public class ImageData
    {
        public string ImagePath { get; set; } //image path

        public string Label { get; set; } // label of the image

        public override string ToString() // used for Listbox in WPF
        {
            return Label;
        }
    }
}
