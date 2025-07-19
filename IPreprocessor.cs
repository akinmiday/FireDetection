using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace FireDetectionApp
{
    public interface IPreprocessor
    {
        /// <summary>
        /// Convert a BGR OpenCV Mat into a normalized CHW tensor.
        /// </summary>
        DenseTensor<float> Process(Mat frame);
    }
}
