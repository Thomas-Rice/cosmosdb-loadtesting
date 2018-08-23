namespace loadtesting
{
    public class CalendarEntry
    {
        public bool IsActive { get; set; }
        public string Warehouse { get; set; }
        public int AvailableFrom { get; set; }
        public int AvailableTo { get; set; }
        public string DeliveryZone { get; set; }
        public string Carrier { get; set; }
        public string CarrierService { get; set; }
        public string AllocationProvider { get; set; }
        public string ServiceLevel { get; set; }
        public int AllocationPreference { get; set; }
        public string[] ViabilityRules { get; set; }
        public string[] AdjustmentRules { get; set; }
        public string AllocationAccount { get; set; }
        public object SolutionId { get; set; }
    }
}