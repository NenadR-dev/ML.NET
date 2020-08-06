using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearning_ImageClassification_UI.Model
{
    /// <summary>
    /// Output model class used for prediction
    /// </summary>
    public class OutputModel
    {
        public string ImagePath { get; set; } //path to image

        public string Label { get; set; } //Label of the image

        public string PredictedLabel { get; set; } //Predicted label 
    }
}
