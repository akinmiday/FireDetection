using Microsoft.ML.OnnxRuntime.Tensors;

namespace FireDetectionApp
{
    public interface IDetector : IDisposable
    {
        /// <summary>
        /// Run inference on the preprocessed tensor.
        /// Returns flat output array and sets numDet (the N dimension).
        /// </summary>
        float[] Run(DenseTensor<float> input, out int numDet);
    }
}
