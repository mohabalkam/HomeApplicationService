using System;

namespace HomeApplianceRepairCenter.Models
{
    /// <summary>Air conditioner: cost depends on BTU capacity, inverter board, gas and on-site visit.</summary>
    public class AirConditioner : HomeAppliance
    {
        private const decimal StandardGasRefillFee = 60m;
        private const decimal LargeGasRefillFee = 90m;
        private const int LargeUnitBtu = 24000;
        private const decimal InverterBoardFee = 80m;
        private const decimal OnSiteVisitFee = 40m;

        private readonly int _capacityBtu;
        private readonly bool _isInverter;
        private bool _needsGasRefill;
        private bool _requiresOnSiteVisit;

        public AirConditioner(string serialNumber, string brand, string customerName,
            string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            int capacityBtu, bool isInverter)
            : base(serialNumber, brand, customerName, faultDescription, receivedDate, expectedDeliveryDate)
        {
            if (capacityBtu <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacityBtu), "Capacity must be positive.");
            _capacityBtu = capacityBtu;
            _isInverter = isInverter;
        }

        public override string DeviceType => "Air Conditioner";

        public int CapacityBtu => _capacityBtu;
        public bool IsInverter => _isInverter;

        public bool NeedsGasRefill
        {
            get => _needsGasRefill;
            set { EnsureEditable(); _needsGasRefill = value; }
        }

        public bool RequiresOnSiteVisit
        {
            get => _requiresOnSiteVisit;
            set { EnsureEditable(); _requiresOnSiteVisit = value; }
        }

        public override decimal CalculateRepairCost()
        {
            decimal cost = CalculateBaseCost();
            if (_needsGasRefill)
                cost += _capacityBtu >= LargeUnitBtu ? LargeGasRefillFee : StandardGasRefillFee;
            if (_isInverter) cost += InverterBoardFee;
            if (_requiresOnSiteVisit) cost += OnSiteVisitFee;
            return cost;
        }
    }
}
