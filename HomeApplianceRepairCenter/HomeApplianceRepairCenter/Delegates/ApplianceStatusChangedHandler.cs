using HomeApplianceRepairCenter.Enums;
using HomeApplianceRepairCenter.Models;

namespace HomeApplianceRepairCenter.Delegates
{
    /// <summary>
    /// Delegate used to notify subscribers that an appliance changed its repair status
    /// (for example: repair completed, or delivery delayed).
    /// </summary>
    public delegate void ApplianceStatusChangedHandler(
        HomeAppliance appliance, RepairStatus oldStatus, RepairStatus newStatus);
}
