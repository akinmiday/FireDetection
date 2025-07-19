using System.Collections.Generic;
using OpenCvSharp;

namespace FireDetectionApp
{
    public class FrameRenderer : IRenderer
    {
        public void Render(Mat frame, IEnumerable<Detection> detections)
        {
            foreach (var d in detections)
            {
                Cv2.Rectangle(frame, d.Box, Scalar.Red, 2);
                Cv2.PutText(
                    frame,
                    $"Fire {d.Confidence:0.00}",
                    new Point(d.Box.X, d.Box.Y - 5),
                    HersheyFonts.HersheySimplex, 0.6, Scalar.Red, 2);
            }
            Cv2.ImShow("Fire Detection", frame);
        }
    }
}
