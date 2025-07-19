// File: Program.cs
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using OpenCvSharp;

namespace FireDetectionApp
{
    class Program
    {
        static void Main()
        {
            // 1) Load configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            var settings = config.Get<AppSettings>() 
                         ?? throw new InvalidOperationException("appsettings.json missing or malformed");

            // 2) Unpack settings
            int    IN_W            = settings.Input.Width;
            int    IN_H            = settings.Input.Height;
            float  CONF_THRESH     = settings.Thresholds.Confidence;
            float  IOU_THRESH      = settings.Thresholds.IoU;
            double FLICKER_THRESH  = settings.Thresholds.Flicker;
            string MODEL_PATH      = settings.ModelPath;
            string SNAPSHOT_DIR    = settings.SnapshotDirectory;
            TimeSpan THROTTLE_INT  = TimeSpan.FromSeconds(settings.ThrottleIntervalSeconds);
            string FRAME_SOURCE    = settings.FrameSource.Source;

            // 3) Compose pipeline with configured source
            using IFrameSource source      = new CameraFrameSource(FRAME_SOURCE);
            var preprocessor               = new FramePreprocessor(IN_W, IN_H);
            using var detector             = new OnnxFireDetector(MODEL_PATH);
            var postprocessor              = new DetectionPostProcessor(CONF_THRESH, IOU_THRESH, IN_W, IN_H);
            var renderer                   = new FrameRenderer();
            var alerts                     = new ConsoleAlertService();
            var flickerFilter              = new FlickerFilter(FLICKER_THRESH);

            // 4) Prepare snapshot folder & throttle timers
            var snapshotDir                = Path.Combine(Directory.GetCurrentDirectory(), SNAPSHOT_DIR);
            Directory.CreateDirectory(snapshotDir);
            DateTime lastSnapshot          = DateTime.MinValue;
            DateTime lastAlert             = DateTime.MinValue;

            Console.WriteLine("▶️ Press ESC to quit");
            while (true)
            {
                if (!source.TryGetFrame(out var frame))
                    break;

                var now = DateTime.Now;

                // 5) Inference every frame
                var tensor     = preprocessor.Process(frame);
                var rawOutput  = detector.Run(tensor, out int numDet);
                var detections = postprocessor.Process(rawOutput, numDet, frame.Size());

                // 6) If we see real fire *and* the scene is flickering, throttle alerts/snapshots
                if (detections.Count > 0 && flickerFilter.HasFlicker(frame))
                {
                    if (now - lastAlert >= THROTTLE_INT)
                    {
                        alerts.Alert(detections);
                        lastAlert = now;
                    }

                    if (now - lastSnapshot >= THROTTLE_INT)
                    {
                        var fn = Path.Combine(
                            snapshotDir,
                            $"fire_{now:yyyyMMdd_HHmmss}.jpg"
                        );
                        Cv2.ImWrite(fn, frame);
                        lastSnapshot = now;
                    }
                }

                // 7) Draw & display
                renderer.Render(frame, detections);

                if (Cv2.WaitKey(1) == 27)  // ESC
                    break;
            }

            // 8) Clean up OpenCV windows
            Cv2.DestroyAllWindows();
        }
    }
}
