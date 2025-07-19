using OpenCvSharp;

namespace FireDetectionApp
{
    class Program
    {
        // Shared constants
        const int IN_W = 640, IN_H = 640;
        const float CONF_THRESH = 0.5f;
        const float IOU_THRESH  = 0.45f;

        static void Main()
        {
            // 1) Compose pipeline components
            using var source       = new CameraFrameSource();
            var preprocessor       = new FramePreprocessor(IN_W, IN_H);
            using var detector     = new OnnxFireDetector("Models/fire_detection.onnx");
            var postprocessor      = new DetectionPostProcessor(CONF_THRESH, IOU_THRESH, IN_W, IN_H);
            var renderer           = new FrameRenderer();
            var alerts             = new ConsoleAlertService();
            var flickerFilter      = new FlickerFilter(threshold: 3.0);

            // 2) Snapshot folder & throttle setup
            var snapshotDir        = Path.Combine(Environment.CurrentDirectory, "Snapshots");
            Directory.CreateDirectory(snapshotDir);
            var snapshotInterval   = TimeSpan.FromSeconds(5);
            DateTime lastSnapshot  = DateTime.MinValue;

            Console.WriteLine("▶️ Press ESC to quit");

            while (true)
            {
                if (!source.TryGetFrame(out var frame))
                    break;

                var now = DateTime.Now;

                // 3) Flicker check
                bool flicker = flickerFilter.HasFlicker(frame);
                Console.WriteLine($"[Debug] Flicker check: {flicker:True/False}");

                var detections = new List<Detection>();

                if (flicker)
                {
                    // 4) Run inference every time we see flicker
                    Console.WriteLine("[Debug] Running ONNX inference…");
                    var tensor = preprocessor.Process(frame);
                    var raw    = detector.Run(tensor, out int numDet);
                    detections = postprocessor.Process(raw, numDet, frame.Size());
                    Console.WriteLine($"[Debug] Raw detections: {detections.Count}");
                }
                else
                {
                    Console.WriteLine("[Debug] Skipping inference (no flicker)");
                }

                // 5) Alert & snapshot throttle
                if (detections.Count > 0)
                {
                    alerts.Alert(detections);

                    if (now - lastSnapshot >= snapshotInterval)
                    {
                        var fn = Path.Combine(
                            snapshotDir,
                            $"fire_{now:yyyyMMdd_HHmmss}.jpg"
                        );
                        Cv2.ImWrite(fn, frame);
                        Console.WriteLine($"[Debug] Saved snapshot: {fn}");
                        lastSnapshot = now;
                    }
                    else
                    {
                        Console.WriteLine($"[Debug] Detected but throttled snapshot (next in {(snapshotInterval - (now - lastSnapshot)).TotalSeconds:0.0}s)");
                    }
                }

                // 6) Draw & display
                renderer.Render(frame, detections);

                if (Cv2.WaitKey(1) == 27)  // ESC
                    break;
            }
        }
    }
}
