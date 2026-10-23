namespace HomeApplianceRepairCenter.Enums
{
    /// <summary>
    /// Life-cycle of an appliance inside the repair center.
    /// Values are sequential because they are used as array indexes for the static counters.
    /// </summary>
    public enum RepairStatus
    {
        Received = 0,   // Appliance arrived, repair not started
        InProgress = 1, // Technician is working on it
        Delayed = 2,    // Expected delivery date passed before the repair finished
        Completed = 3,  // Repair finished, waiting for the customer
        Delivered = 4   // Handed back to the customer
    }
}
