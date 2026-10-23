using System;

namespace HomeApplianceRepairCenter.Models
{
    /// <summary>Washing machine: cost depends on loading type, drum and water pump.</summary>
    public class WashingMachine : HomeAppliance
    {
        private const decimal FrontLoadServiceFee = 25m;
        private const decimal FrontLoadDrumFee = 150m;
        private const decimal TopLoadDrumFee = 100m;
        private const decimal PumpReplacementFee = 60m;

        private readonly double _loadCapacityKg;
        private readonly bool _isFrontLoad;
        private bool _needsDrumReplacement;
        private bool _needsPumpReplacement;

        public WashingMachine(string serialNumber, string brand, string customerName,
            string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            double loadCapacityKg, bool isFrontLoad)
            : base(serialNumber, brand, customerName, faultDescription, receivedDate, expectedDeliveryDate)
        {
            if (loadCapacityKg <= 0)
                throw new ArgumentOutOfRangeException(nameof(loadCapacityKg), "Load capacity must be positive.");
            _loadCapacityKg = loadCapacityKg;
            _isFrontLoad = isFrontLoad;
        }

        public override string DeviceType => "Washing Machine";

        public double LoadCapacityKg => _loadCapacityKg;
        public bool IsFrontLoad => _isFrontLoad;

        public bool NeedsDrumReplacement
        {
            get => _needsDrumReplacement;
            set { EnsureEditable(); _needsDrumReplacement = value; }
        }

        public bool NeedsPumpReplacement
        {
            get => _needsPumpReplacement;
            set { EnsureEditable(); _needsPumpReplacement = value; }
        }

        public override decimal CalculateRepairCost()
        {
            decimal cost = CalculateBaseCost();
            if (_isFrontLoad) cost += FrontLoadServiceFee; // door seal and bearing access is harder
            if (_needsDrumReplacement) cost += _isFrontLoad ? FrontLoadDrumFee : TopLoadDrumFee;
            if (_needsPumpReplacement) cost += PumpReplacementFee;
            return cost;
        }
    }
}
