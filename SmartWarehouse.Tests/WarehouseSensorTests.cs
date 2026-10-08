using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartWarehouse.Core;

namespace SmartWarehouse.Tests
{
    [TestClass]
    public class WarehouseSensorTests
    {
        [TestMethod]
        public void Constructor_SetsInitialState()
        {
            // Arrange
            string sensorId = "SENSOR-001";
            string locationTag = "Cold-Vault-01";

            // Act
            WarehouseSensor sensor = new WarehouseSensor(sensorId, locationTag);

            // Assert
            Assert.AreEqual(sensorId, sensor.SensorId);
            Assert.AreEqual(locationTag, sensor.LocationTag);
            Assert.AreEqual(0.0, sensor.CurrentTemperature);
            Assert.IsFalse(sensor.IsActive);
            Assert.IsFalse(sensor.IsAlertTriggered);
            Assert.AreEqual(4.0, sensor.CriticalThresholdCelsius);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_InvalidSensorId_ThrowsArgumentException(string sensorId)
        {
            // Arrange
            string locationTag = "Cold-Vault-01";

            // Act
            Action act = () => new WarehouseSensor(sensorId, locationTag);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_InvalidLocationTag_ThrowsArgumentException(string locationTag)
        {
            // Arrange
            string sensorId = "SENSOR-001";

            // Act
            Action act = () => new WarehouseSensor(sensorId, locationTag);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [TestMethod]
        public void Activate_SetsSensorToActive()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");

            // Act
            sensor.Activate();
            sensor.Activate(); // Call Activate again to test idempotency

            // Assert
            Assert.IsTrue(sensor.IsActive);
        }

        [TestMethod]
        public void Deactivate_SetsSensorInactiveAndClearsAlert()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
            sensor.Activate();
            sensor.RecordReading(5.0); // Trigger alert

            // Act
            sensor.Deactivate();

            // Assert
            Assert.IsFalse(sensor.IsActive);
            Assert.IsFalse(sensor.IsAlertTriggered);
        }

        [TestMethod]
        public void RecordReading_WhenInactive_ThrowsInvalidOperationException()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");

            // Act
            Action act = () => sensor.RecordReading(5.0);

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }

        [TestMethod]
        public void RecordReading_AtMinimumTemperature_Succeeds()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
            sensor.Activate();

            // Act
            sensor.RecordReading(-50.0); // Minimum temperature

            // Assert
            Assert.AreEqual(-50.0, sensor.CurrentTemperature);
        }

        [TestMethod]
        public void RecordReading_BelowMinimumTemperature_Succeeds()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
            sensor.Activate();

            // Act
            Action act = () => sensor.RecordReading(-50.1); // Below minimum temperature

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(act);
        }

        [TestMethod]
        public void RecordReading_AtMaximumTemperature_Succeeds()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
            sensor.Activate();

            // Act
            sensor.RecordReading(80.0); // Maximum temperature

            // Assert
            Assert.AreEqual(80.0, sensor.CurrentTemperature);
        }

        [TestMethod]
        public void RecordReading_AboveMaximumTemperature_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
            sensor.Activate();

            // Act
            Action act = () => sensor.RecordReading(80.1); // Above maximum temperature

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(act);
        }

        [TestMethod]
        [DataRow(3.9, false)]
        [DataRow(4.0, true)]
        [DataRow(4.1, true)]
        public void RecordReading_UpdatesAlertBasedOnThreshold(double temperature, bool expectedAlert)
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
            sensor.Activate();
            
            // Act
            sensor.RecordReading(temperature);
            
            // Assert
            Assert.AreEqual(expectedAlert, sensor.IsAlertTriggered);
        }

        [TestMethod]
        public void UpdateThreshold_RecalculatesAlert()
        {
            // Arrange
            WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
            sensor.Activate();
            sensor.RecordReading(5.0); // Trigger alert

            // Act
            sensor.UpdateThreshold(8.0); // Raise threshold above current temperature

            // Assert
            Assert.AreEqual(8.0, sensor.CriticalThresholdCelsius);
            Assert.IsFalse(sensor.IsAlertTriggered); // Alert should be cleared
        }
    }
}
