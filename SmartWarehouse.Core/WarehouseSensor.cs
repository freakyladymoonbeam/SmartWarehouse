using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWarehouse.Core
{
    public class WarehouseSensor
    {
        public string SensorId { get; private set; }
        public string LocationTag { get; set; }
        public double CurrentTemperature { get; set; }
        public bool IsActive { get; set; }
        public bool IsAlertTriggered { get; private set; }
        public double CriticalThresholdCelsius { get; set; }

        public WarehouseSensor(string sensorId, string locationTag, double criticalThreshold = 4.0)
        {
            if (string.IsNullOrWhiteSpace(sensorId))
            {
                throw new ArgumentException("Sensor ID cannot be blank.");
            }

            if (string.IsNullOrWhiteSpace(locationTag))
            {
                throw new ArgumentException("Location tag cannot be blank.");
            }

            SensorId = sensorId;
            LocationTag = locationTag;
            CurrentTemperature = 0.0;
            IsActive = false;
            IsAlertTriggered = false;
            CriticalThresholdCelsius = criticalThreshold;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
            IsAlertTriggered = false;
        }

        public void RecordReading(double newTemperature)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Sensor must be active to record a reading.");
            }

            if (newTemperature < -50.0 || newTemperature > 80.0)
            {
                throw new ArgumentOutOfRangeException(nameof(newTemperature), "Temperature must be between -50.0°C and 80.0°C.");
            }

            CurrentTemperature = newTemperature;
            IsAlertTriggered = newTemperature >= CriticalThresholdCelsius;
        }

        public void UpdateThreshold(double newThreshold)
        {
            if (newThreshold < -30.0 || newThreshold > 50.0)
            {
                throw new ArgumentOutOfRangeException(nameof(newThreshold), "Threshold must be between -30.0°C and 50.0°C.");
            }

            CriticalThresholdCelsius = newThreshold;
            IsAlertTriggered = CurrentTemperature >= CriticalThresholdCelsius;
        }
    }
}
