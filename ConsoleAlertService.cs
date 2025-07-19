using System;
using System.Collections.Generic;
using System.Linq;

namespace FireDetectionApp
{
    public class ConsoleAlertService : IAlertService
    {
        public void Alert(IEnumerable<Detection> detections)
        {
            int count = detections.Count();
            if (count > 0)
                Console.WriteLine($"🔥 Fire detected ({count} region{(count>1?"s":"")}) at {DateTime.Now:HH:mm:ss}");
        }
    }
}
