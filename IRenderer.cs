using System.Collections.Generic;
using OpenCvSharp;

namespace FireDetectionApp
{
    public interface IRenderer
    {
        void Render(Mat frame, IEnumerable<Detection> detections);
    }
}
