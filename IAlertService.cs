using System.Collections.Generic;

namespace FireDetectionApp
{
    public interface IAlertService
    {
        void Alert(IEnumerable<Detection> detections);
    }
}
