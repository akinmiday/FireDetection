using System;
using OpenCvSharp;

namespace FireDetectionApp
{
    public class CameraFrameSource : IFrameSource
    {
        private readonly VideoCapture _capture;

        public CameraFrameSource(int device = 0)
        {
            _capture = new VideoCapture(device);
            if (!_capture.IsOpened())
                throw new Exception("Cannot open camera");
        }

        public bool TryGetFrame(out Mat frame)
        {
            frame = new Mat();
            return _capture.Read(frame) && !frame.Empty();
        }

        public void Dispose() => _capture.Release();
    }
}
