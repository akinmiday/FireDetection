using System;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace FireDetectionApp
{
    public class OnnxFireDetector : IDetector
    {
        private readonly InferenceSession _session;

        public OnnxFireDetector(string modelPath)
        {
            var opts = new SessionOptions();
            opts.EnableMemoryPattern = true;
            opts.EnableCpuMemArena   = true;
            _session = new InferenceSession(modelPath, opts);
        }

        public float[] Run(DenseTensor<float> input, out int numDet)
        {
            // Build the array of inputs (NamedOnnxValue is NOT disposable here)
            var inputs = new[] 
            { 
                NamedOnnxValue.CreateFromTensor("images", input) 
            };

            // Only 'results' needs disposing
            using var results = _session.Run(inputs);

            // Extract the single output tensor
            var outputTensor = results.First().AsTensor<float>();
            numDet = outputTensor.Dimensions[1];
            return outputTensor.ToArray();
        }

        public void Dispose() => _session.Dispose();
    }
}
