using System;
using HomeApplianceRepairCenter.Delegates;
using HomeApplianceRepairCenter.Enums;
using HomeApplianceRepairCenter.Helpers;
using HomeApplianceRepairCenter.Interfaces;

namespace HomeApplianceRepairCenter.Models
{
    /// <summary>
    /// Abstract base class for every appliance accepted by the repair center.
    /// Implements IRepairable and leaves the cost calculation to each device type.
    /// </summary>
    public abstract class HomeAppliance : IRepairable
    {
        private const decimal LaborRatePerHour = 15m;

        // Static counters: one slot per RepairStatus value
        private static int _totalAppliances;
        private static readonly int[] _statusCounts = new int[Enum.GetValues(typeof(RepairStatus)).Length];

        // Immutable after creation
        private readonly string _serialNumber;
        private readonly string _brand;
        private readonly string _customerName;
        private readonly string _faultDescription;
        private readonly DateTime _receivedDate;
        private readonly DateTime _expectedDeliveryDate;

        // Can change during the repair
        private RepairStatus _status;
        private decimal _partsCost;
        private decimal _laborHours;

        // True once a RepairCenter has accepted this appliance; only then is it counted in the static statistics
        private bool _isCounted;

        /// <summary>Raised every time the repair status changes (completed, delayed, ...).</summary>
        public event ApplianceStatusChangedHandler StatusChanged;

        protected HomeAppliance(string serialNumber, string brand, string customerName,
            string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate)
        {
            if (!ApplianceValidator.IsValidSerialNumber(serialNumber))
                throw new ArgumentException("Serial number must look like RF-123456.", nameof(serialNumber));
            if (!ApplianceValidator.IsValidName(brand))
                throw new ArgumentException("Brand is required.", nameof(brand));
            if (!ApplianceValidator.IsValidName(customerName))
                throw new ArgumentException("Customer name is required.", nameof(customerName));
            if (!ApplianceValidator.IsValidName(faultDescription))
                throw new ArgumentException("Fault description is required.", nameof(faultDescription));
            if (!ApplianceValidator.IsValidReceivedDate(receivedDate))
                throw new ArgumentException("Received date is not valid.", nameof(receivedDate));
            if (!ApplianceValidator.IsValidDeliveryDate(receivedDate, expectedDeliveryDate))
                throw new ArgumentException("Delivery date cannot be before the received date.", nameof(expectedDeliveryDate));

            _serialNumber = serialNumber;
            _brand = brand.Trim();
            _customerName = customerName.Trim();
            _faultDescription = faultDescription.Trim();
            _receivedDate = receivedDate.Date;
            _expectedDeliveryDate = expectedDeliveryDate.Date;
            _status = RepairStatus.Received;
        }

        // ---------- Read-only properties (fixed at creation) ----------
        public string SerialNumber => _serialNumber;
        public string Brand => _brand;
        public string CustomerName => _customerName;
        public string FaultDescription => _faultDescription;
        public DateTime ReceivedDate => _receivedDate;
        public DateTime ExpectedDeliveryDate => _expectedDeliveryDate;

        /// <summary>Human-readable device type (Refrigerator, Washing Machine, ...).</summary>
        public abstract string DeviceType { get; }

        // ---------- Controlled read/write properties ----------
        public RepairStatus Status => _status;

        public bool IsRepaired => _status == RepairStatus.Completed || _status == RepairStatus.Delivered;

        public decimal PartsCost
        {
            get => _partsCost;
            set
            {
                EnsureEditable();
                if (!ApplianceValidator.IsValidCost(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "Parts cost cannot be negative.");
                _partsCost = value;
            }
        }

        public decimal LaborHours
        {
            get => _laborHours;
            set
            {
                EnsureEditable();
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Labor hours cannot be negative.");
                _laborHours = value;
            }
        }

        // ---------- Abstract operation: each device type prices its own repair ----------
        public abstract decimal CalculateRepairCost();

        /// <summary>Parts + labor, shared by all device types.</summary>
        protected decimal CalculateBaseCost()
        {
            return _partsCost + (_laborHours * LaborRatePerHour);
        }

        /// <summary>Prevents changing repair data once the work is finished.</summary>
        protected void EnsureEditable()
        {
            if (IsRepaired)
                throw new InvalidOperationException(
                    $"Appliance {_serialNumber} is already repaired; its data can no longer be modified.");
        }

        // ---------- Repair workflow ----------
        public void StartRepair()
        {
            if (_status != RepairStatus.Received && _status != RepairStatus.Delayed)
                throw new InvalidOperationException($"Cannot start repair while status is {_status}.");
            ChangeStatus(RepairStatus.InProgress);
        }

        public void CompleteRepair()
        {
            if (_status != RepairStatus.InProgress && _status != RepairStatus.Delayed)
                throw new InvalidOperationException($"Cannot complete repair while status is {_status}.");
            ChangeStatus(RepairStatus.Completed);
        }

        public void Deliver()
        {
            if (_status != RepairStatus.Completed)
                throw new InvalidOperationException("Only a completed repair can be delivered.");
            ChangeStatus(RepairStatus.Delivered);
        }

        /// <summary>
        /// Marks the appliance as delayed if it is still unrepaired after the promised date.
        /// Returns true when the status was changed (and the event raised).
        /// </summary>
        public bool CheckDelivery(DateTime today)
        {
            bool stillOpen = _status == RepairStatus.Received || _status == RepairStatus.InProgress;
            if (stillOpen && today.Date > _expectedDeliveryDate)
            {
                ChangeStatus(RepairStatus.Delayed);
                return true;
            }
            return false;
        }

        public string GetStatusSummary()
        {
            return $"[{_serialNumber}] {DeviceType,-15} {_brand,-8} | Customer: {_customerName,-12} | " +
                   $"Status: {_status,-10} | Due: {_expectedDeliveryDate:yyyy-MM-dd} | Cost: {CalculateRepairCost():N2}";
        }

        // ---------- Event raising ----------
        private void ChangeStatus(RepairStatus newStatus)
        {
            RepairStatus oldStatus = _status;
            if (_isCounted)
            {
                _statusCounts[(int)oldStatus]--;
                _statusCounts[(int)newStatus]++;
            }
            _status = newStatus;
            OnStatusChanged(oldStatus, newStatus);
        }

        protected virtual void OnStatusChanged(RepairStatus oldStatus, RepairStatus newStatus)
        {
            StatusChanged?.Invoke(this, oldStatus, newStatus);
        }

        /// <summary>
        /// Called by RepairCenter after the appliance is accepted (not a duplicate),
        /// so rejected objects never affect the static counters.
        /// </summary>
        internal void RegisterInStatistics()
        {
            if (_isCounted) return;
            _isCounted = true;
            _totalAppliances++;
            _statusCounts[(int)_status]++;
        }

        // ---------- Static members ----------
        public static int TotalAppliances => _totalAppliances;
        public static int ReceivedCount => _statusCounts[(int)RepairStatus.Received];
        public static int InProgressCount => _statusCounts[(int)RepairStatus.InProgress];
        public static int DelayedCount => _statusCounts[(int)RepairStatus.Delayed];
        public static int CompletedCount => _statusCounts[(int)RepairStatus.Completed];
        public static int DeliveredCount => _statusCounts[(int)RepairStatus.Delivered];

        /// <summary>Number of appliances currently in the given status.</summary>
        public static int CountByStatus(RepairStatus status)
        {
            return _statusCounts[(int)status];
        }
    }
}
