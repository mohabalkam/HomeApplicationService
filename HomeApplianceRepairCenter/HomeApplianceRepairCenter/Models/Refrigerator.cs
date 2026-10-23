using System;

namespace HomeApplianceRepairCenter.Models
{
    /// <summary>Refrigerator: cost depends on capacity, compressor and gas refill.</summary>
    public class Refrigerator : HomeAppliance
    {
        private const decimal CompressorReplacementFee = 120m;
        private const decimal GasRefillFee = 45m;
        private const decimal LargeCapacityFee = 30m;
        private const int LargeCapacityLiters = 500;

        private readonly int _capacityLiters;
        private bool _needsCompressorReplacement;
        private bool _needsGasRefill;

        public Refrigerator(string serialNumber, string brand, string customerName,
            string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            int capacityLiters)
            : base(serialNumber, brand, customerName, faultDescription, receivedDate, expectedDeliveryDate)
        {
            if (capacityLiters <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacityLiters), "Capacity must be positive.");
            _capacityLiters = capacityLiters;
        }

        public override string DeviceType => "Refrigerator";

        public int CapacityLiters => _capacityLiters;

        public bool NeedsCompressorReplacement
        {
            get => _needsCompressorReplacement;
            set { EnsureEditable(); _needsCompressorReplacement = value; }
        }

        public bool NeedsGasRefill
        {
            get => _needsGasRefill;
            set { EnsureEditable(); _needsGasRefill = value; }
        }

        public override decimal CalculateRepairCost()
        {
            decimal cost = CalculateBaseCost();
            if (_needsCompressorReplacement) cost += CompressorReplacementFee;
            if (_needsGasRefill) cost += GasRefillFee;
            if (_capacityLiters > LargeCapacityLiters) cost += LargeCapacityFee; // heavier to move and open
            return cost;
        }
    }
}
