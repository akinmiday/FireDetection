using System.Collections.Generic;
using OpenCvSharp;

namespace FireDetectionApp
{
    public interface IPostProcessor
    {
        /// <summary>
        /// Convert raw ONNX output into a list of detections
        /// (applies confidence thresholding and NMS).
        /// </summary>
        List<Detection> Process(float[] dets, int numDet, Size frameSize);
    }
}
