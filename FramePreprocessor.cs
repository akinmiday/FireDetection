using OpenCvSharp;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace FireDetectionApp
{
    public class FramePreprocessor : IPreprocessor
    {
        private readonly int _width, _height;

        public FramePreprocessor(int width, int height)
            => (_width, _height) = (width, height);

        public DenseTensor<float> Process(Mat frame)
        {
            // Resize & convert to RGB
            var img = frame
                .Resize(new Size(_width, _height), 0, 0, InterpolationFlags.Linear)
                .CvtColor(ColorConversionCodes.BGR2RGB);

            var tensor = new DenseTensor<float>(new[] { 1, 3, _height, _width });
            for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
            {
                var p = img.At<Vec3b>(y, x);
                tensor[0, 0, y, x] = p.Item0 / 255f; // R
                tensor[0, 1, y, x] = p.Item1 / 255f; // G
                tensor[0, 2, y, x] = p.Item2 / 255f; // B
            }
            return tensor;
        }
    }
}
