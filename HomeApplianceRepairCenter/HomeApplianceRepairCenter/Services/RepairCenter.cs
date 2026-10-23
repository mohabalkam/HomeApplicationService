using System;
using System.Collections.Generic;
using System.Linq;
using HomeApplianceRepairCenter.Delegates;
using HomeApplianceRepairCenter.Enums;
using HomeApplianceRepairCenter.Models;

namespace HomeApplianceRepairCenter.Services
{
    /// <summary>
    /// Main class: holds the appliances and processes them polymorphically through HomeAppliance.
    /// </summary>
    public class RepairCenter
    {
        private readonly List<HomeAppliance> _appliances = new List<HomeAppliance>();

        /// <summary>Forwards the status-change event of any registered appliance.</summary>
        public event ApplianceStatusChangedHandler ApplianceStatusChanged;

        public IReadOnlyList<HomeAppliance> Appliances => _appliances.AsReadOnly();

        public int Count => _appliances.Count;

        public void AddAppliance(HomeAppliance appliance)
        {
            if (appliance == null)
                throw new ArgumentNullException(nameof(appliance));
            if (FindBySerialNumber(appliance.SerialNumber) != null)
                throw new InvalidOperationException($"Serial number {appliance.SerialNumber} is already registered.");

            _appliances.Add(appliance);
            appliance.RegisterInStatistics();
            appliance.StatusChanged += OnApplianceStatusChanged;
        }

        public HomeAppliance FindBySerialNumber(string serialNumber)
        {
            return _appliances.FirstOrDefault(a => a.SerialNumber == serialNumber);
        }

        /// <summary>Total of all repair costs; each appliance computes its own cost (polymorphism).</summary>
        public decimal CalculateTotalRepairCost()
        {
            decimal total = 0m;
            foreach (HomeAppliance appliance in _appliances)
                total += appliance.CalculateRepairCost();
            return total;
        }

        /// <summary>Revenue only from appliances whose repair is finished.</summary>
        public decimal CalculateEarnedRevenue()
        {
            return GetRepairedAppliances().Sum(a => a.CalculateRepairCost());
        }

        public IEnumerable<HomeAppliance> GetRepairedAppliances()
        {
            return _appliances.Where(a => a.IsRepaired);
        }

        public IEnumerable<HomeAppliance> GetUnderRepairAppliances()
        {
            return _appliances.Where(a => !a.IsRepaired);
        }

        /// <summary>Checks all open appliances against today's date; raises events for delayed ones.</summary>
        public int CheckOverdueDeliveries(DateTime today)
        {
            int delayedNow = 0;
            foreach (HomeAppliance appliance in _appliances)
            {
                if (appliance.CheckDelivery(today))
                    delayedNow++;
            }
            return delayedNow;
        }

        public void DisplayAllStatuses()
        {
            foreach (HomeAppliance appliance in _appliances)
                Console.WriteLine(appliance.GetStatusSummary());
        }

        private void OnApplianceStatusChanged(HomeAppliance appliance, RepairStatus oldStatus, RepairStatus newStatus)
        {
            ApplianceStatusChanged?.Invoke(appliance, oldStatus, newStatus);
        }
    }
}
