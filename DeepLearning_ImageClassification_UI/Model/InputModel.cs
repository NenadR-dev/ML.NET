using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearning_ImageClassification_UI.Model
{
    /// <summary>
    /// Input model class used for creating a prediction
    /// </summary>
    public class InputModel
    {
        public byte[] Image { get; set; } //Array of bytes of image

        public UInt32 LabelAsKey { get; set; } //Key used for the Machine learning process

        public string ImagePath { get; set; } //Image path

        public string Label { get; set; } // Label of the image
    }
}
