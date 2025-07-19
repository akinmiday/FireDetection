using OpenCvSharp;

namespace FireDetectionApp
{
    /// <summary>
    /// A very simple temporal filter: returns true if the current frame
    /// differs from the last one by more than a pixel‐difference threshold.
    /// </summary>
    public class FlickerFilter
    {
        private Mat? _lastGray;
        private readonly double _threshold;

        /// <param name="threshold">
        /// Mean‐pixel absolute difference threshold (0–255). 
        /// Tweak this: start around 2–5 on 8-bit frames.
        /// </param>
        public FlickerFilter(double threshold = 3.0)
        {
            _threshold = threshold;
            _lastGray = null;
        }

        public bool HasFlicker(Mat frame)
        {
            // Convert to grayscale
            using var gray = frame.CvtColor(ColorConversionCodes.BGR2GRAY);

            // If no previous frame, seed and assume “live”
            if (_lastGray is null)
            {
                _lastGray = gray.Clone();
                return true;
            }

            // Compute absolute difference & mean
            using var diff  = new Mat();
            Cv2.Absdiff(gray, _lastGray, diff);
            var meanVal     = diff.Mean()[0];

            // Update the last frame
            _lastGray.Dispose();
            _lastGray = gray.Clone();

            // Flicker only if mean diff exceeds threshold
            return meanVal > _threshold;
        }
    }
}
