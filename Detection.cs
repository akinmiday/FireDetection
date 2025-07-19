using OpenCvSharp;

namespace FireDetectionApp
{
    // Simple DTO for one fire detection
    public record Detection(Rect Box, float Confidence);
}
