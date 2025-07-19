using OpenCvSharp;

namespace FireDetectionApp
{
    public interface IFrameSource : IDisposable
    {
        /// <summary>
        /// Grab the next frame. Returns false if no more frames.
        /// </summary>
        bool TryGetFrame(out Mat frame);
    }
}
