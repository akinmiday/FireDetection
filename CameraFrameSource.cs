// File: CameraFrameSource.cs
using System;
using OpenCvSharp;

namespace FireDetectionApp
{
    /// <summary>
    /// Opens either:
    ///  • a webcam by index ("0", "1", …)
    ///  • an RTSP/HTTP stream or video file via URL/path
    /// </summary>
    public class CameraFrameSource : IFrameSource
    {
        private readonly VideoCapture _capture;

        public CameraFrameSource(string source = "0")
        {
            if (int.TryParse(source, out var idx))
            {
                _capture = new VideoCapture(idx);
            }
            else
            {
                _capture = new VideoCapture(source);
            }

            if (!_capture.IsOpened())
                throw new Exception($"Cannot open video source: {source}");
        }

        public bool TryGetFrame(out Mat frame)
        {
            frame = new Mat();
            return _capture.Read(frame) && !frame.Empty();
        }

        public void Dispose() => _capture.Release();
    }
}
