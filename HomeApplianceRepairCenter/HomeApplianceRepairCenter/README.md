# Home Appliance Repair Center (C# / OOP)

Console project that manages appliances in a repair center (refrigerators, washing machines, air conditioners).

## Structure
- `Interfaces/IRepairable.cs`: common operations (interface)
- `Models/HomeAppliance.cs`: abstract class with the abstract `CalculateRepairCost()`
- `Models/Refrigerator.cs`, `WashingMachine.cs`, `AirConditioner.cs`: derived classes
- `Services/RepairCenter.cs`: main class holding `List<HomeAppliance>`
- `Delegates/ApplianceStatusChangedHandler.cs`: delegate; event `StatusChanged` in `HomeAppliance`
- `Helpers/ApplianceValidator.cs`: static validation class
- `Enums/RepairStatus.cs`: status values
- `Program.cs`: demo

## Run
`dotnet run` (requires the .NET SDK; change `TargetFramework` in the .csproj if you use another version, e.g. `net6.0`).
