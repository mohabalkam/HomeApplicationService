using System;
using System.Text;
using HomeApplianceRepairCenter.Enums;
using HomeApplianceRepairCenter.Models;
using HomeApplianceRepairCenter.Services;

namespace HomeApplianceRepairCenter
{
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            DateTime today = DateTime.Today;

            var center = new RepairCenter();
            center.ApplianceStatusChanged += OnApplianceStatusChanged; // subscribe to notifications

            // Three different appliance types stored in one list of the base type
            center.AddAppliance(new Refrigerator("RF-100234", "Samsung", "Ahmad", "Not cooling",
                today.AddDays(-5), today.AddDays(5), 450)
            { NeedsCompressorReplacement = true, PartsCost = 95m, LaborHours = 3m });

            center.AddAppliance(new WashingMachine("WM-200871", "LG", "Sara", "Drum noise",
                today.AddDays(-12), today.AddDays(-2), 8, true)
            { NeedsDrumReplacement = true, PartsCost = 120m, LaborHours = 4m });

            center.AddAppliance(new AirConditioner("AC-300455", "Gree", "Omar", "Gas leak",
                today.AddDays(-3), today.AddDays(4), 18000, true)
            { NeedsGasRefill = true, RequiresOnSiteVisit = true, PartsCost = 30m, LaborHours = 2.5m });

            Console.WriteLine("=== Appliances received ===");
            center.DisplayAllStatuses();

            Console.WriteLine("\n=== Working on appliances ===");
            center.FindBySerialNumber("RF-100234").StartRepair();
            center.FindBySerialNumber("AC-300455").StartRepair();
            center.FindBySerialNumber("RF-100234").CompleteRepair();

            Console.WriteLine("\n=== Checking late deliveries ===");
            int delayed = center.CheckOverdueDeliveries(today);
            Console.WriteLine($"Appliances newly delayed: {delayed}");

            center.FindBySerialNumber("WM-200871").StartRepair();
            center.FindBySerialNumber("WM-200871").CompleteRepair();
            center.FindBySerialNumber("RF-100234").Deliver();

            Console.WriteLine("\n=== Current state ===");
            center.DisplayAllStatuses();

            Console.WriteLine("\n=== Repaired appliances ===");
            foreach (HomeAppliance a in center.GetRepairedAppliances())
                Console.WriteLine($"{a.SerialNumber} - {a.DeviceType} ({a.Status})");

            Console.WriteLine("\n=== Still under repair ===");
            foreach (HomeAppliance a in center.GetUnderRepairAppliances())
                Console.WriteLine($"{a.SerialNumber} - {a.DeviceType} ({a.Status})");

            Console.WriteLine("\n=== Totals ===");
            Console.WriteLine($"Total of all repair costs: {center.CalculateTotalRepairCost():N2}");
            Console.WriteLine($"Revenue from finished repairs: {center.CalculateEarnedRevenue():N2}");
            Console.WriteLine($"Total appliances: {HomeAppliance.TotalAppliances}");
            Console.WriteLine($"In progress: {HomeAppliance.InProgressCount}, Completed: {HomeAppliance.CompletedCount}, " +
                              $"Delivered: {HomeAppliance.DeliveredCount}, Delayed: {HomeAppliance.DelayedCount}");

            Console.WriteLine("\n=== Validation and encapsulation checks ===");
            TryAction("Invalid serial number", () =>
                new Refrigerator("BAD", "Samsung", "Ali", "Noise", today, today.AddDays(2), 300));
            TryAction("Editing a finished repair", () =>
                center.FindBySerialNumber("WM-200871").PartsCost = 500m);
        }

        // Event handler: reacts to completed and delayed repairs
        private static void OnApplianceStatusChanged(HomeAppliance appliance, RepairStatus oldStatus, RepairStatus newStatus)
        {
            string message = newStatus switch
            {
                RepairStatus.Completed => "repair completed, the customer can be called.",
                RepairStatus.Delayed => "delivery date missed, the customer must be informed!",
                _ => $"status changed from {oldStatus} to {newStatus}."
            };
            Console.WriteLine($"[NOTIFICATION] {appliance.DeviceType} {appliance.SerialNumber}: {message}");
        }

        private static void TryAction(string title, Action action)
        {
            try
            {
                action();
                Console.WriteLine($"{title}: no error (unexpected)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{title}: blocked -> {ex.Message}");
            }
        }
    }
}
