
namespace SagaDB.Item
{
    public class ActorTimerItem
    {
        uint itemID, startTime, endTime;

        public uint ItemID { get { return this.itemID; } set { this.itemID = value; } }
        public string ItemName { get; set; }
        public uint StartTime { get { return this.startTime; } set { this.startTime = value; } }
        public uint EndTime { get { return this.endTime; } set { this.endTime = value; } }
        public string BuffCodeName { get; set; }
        public string BuffName { get; set; }
        public string BuffValues { get; set; }
        public bool DurationType { get; set; }
        public uint Duration { get; set; }
        public bool LogoutCount { get; set; }
    }
}
