using HomeApplianceRepairCenter.Enums;

namespace HomeApplianceRepairCenter.Interfaces
{
    /// <summary>
    /// Core operations shared by every appliance handled by the repair center.
    /// </summary>
    public interface IRepairable
    {
        RepairStatus Status { get; }

        /// <summary>True once the repair has been completed (or the device already delivered).</summary>
        bool IsRepaired { get; }

        void StartRepair();
        void CompleteRepair();
        void Deliver();

        decimal CalculateRepairCost();
        string GetStatusSummary();
    }
}
