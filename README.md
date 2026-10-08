# SmartWarehouse

A multi-project .NET 10 solution for modeling and unit testing an industrial IoT warehouse environmental sensor.

## Projects

- `SmartWarehouse.Core` — Domain library containing the `WarehouseSensor` entity and its validation and lifecycle rules.
- `SmartWarehouse.Tests` — MSTest project containing the unit tests for `WarehouseSensor`.

## Testing

The solution includes comprehensive unit tests covering:

- Constructor validation and initial state
- Sensor activation and deactivation
- Temperature reading validation
- Temperature boundary values
- Alert threshold behavior
- Dynamic threshold updates