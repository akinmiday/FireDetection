using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace FireDetectionApp
{
    public class DetectionPostProcessor : IPostProcessor
    {
        private readonly float _confThresh, _iouThresh;
        private readonly int _modelW, _modelH;

        public DetectionPostProcessor(
            float confThresh, float iouThresh, int modelW, int modelH)
            => (_confThresh, _iouThresh, _modelW, _modelH)
            = (confThresh, iouThresh, modelW, modelH);

        private static float IoU(Rect a, Rect b)
        {
            int x1 = Math.Max(a.X, b.X), y1 = Math.Max(a.Y, b.Y);
            int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            int w = x2 - x1, h = y2 - y1;
            if (w <= 0 || h <= 0) return 0;
            int inter = w * h;
            int union = a.Width * a.Height + b.Width * b.Height - inter;
            return inter / (float)union;
        }

        public List<Detection> Process(float[] dets, int numDet, Size frameSize)
        {
            var rects = new List<Rect>();
            var scores = new List<float>();
            float scaleX = frameSize.Width  / (float)_modelW;
            float scaleY = frameSize.Height / (float)_modelH;

            for (int i = 0; i < numDet; i++)
            {
                float conf = dets[i*6 + 4];
                if (conf < _confThresh) continue;

                float x1 = dets[i*6 + 0], y1 = dets[i*6 + 1];
                float x2 = dets[i*6 + 2], y2 = dets[i*6 + 3];

                var r = new Rect(
                    (int)(x1 * scaleX),
                    (int)(y1 * scaleY),
                    (int)((x2 - x1) * scaleX),
                    (int)((y2 - y1) * scaleY)
                );
                rects.Add(r);
                scores.Add(conf);
            }

            // NMS
            var idxs = Enumerable.Range(0, scores.Count)
                .OrderByDescending(i => scores[i]).ToList();
            var keep = new List<int>();
            while (idxs.Count > 0)
            {
                int i = idxs[0];
                keep.Add(i);
                var rest = new List<int>();
                for (int j = 1; j < idxs.Count; j++)
                    if (IoU(rects[i], rects[idxs[j]]) <= _iouThresh)
                        rest.Add(idxs[j]);
                idxs = rest;
            }

            return keep.Select(i => new Detection(rects[i], scores[i])).ToList();
        }
    }
}
