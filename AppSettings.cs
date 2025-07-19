// namespace FireDetectionApp
// {
//     public class AppSettings
//     {
//         public string ModelPath { get; set; } = "";
//         public InputSettings Input { get; set; } = new();
//         public ThresholdSettings Thresholds { get; set; } = new();
//         public int ThrottleIntervalSeconds { get; set; }
//         public string SnapshotDirectory { get; set; } = "";

//         public class InputSettings
//         {
//             public int Width  { get; set; }
//             public int Height { get; set; }
//         }

//         public class ThresholdSettings
//         {
//             public float Confidence { get; set; }
//             public float IoU        { get; set; }
//             public double Flicker   { get; set; }
//         }
//     }
// }


namespace FireDetectionApp
{
    public class AppSettings
    {
        public string ModelPath { get; set; } = "";
        public InputSettings Input { get; set; } = new();
        public ThresholdSettings Thresholds { get; set; } = new();
        public int ThrottleIntervalSeconds { get; set; }
        public string SnapshotDirectory { get; set; } = "";
        public FrameSourceSettings FrameSource { get; set; } = new();

        public class InputSettings { public int Width { get; set; } public int Height { get; set; } }
        public class ThresholdSettings { public float Confidence { get; set; } public float IoU { get; set; } public double Flicker { get; set; } }
        public class FrameSourceSettings { public string Source { get; set; } = "0"; }
    }
}
