using System.Collections.Generic;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger.Entities
{
    public class VehicleShopNote
    {
        public string Id { get; set; }
        public string VehicleNumber { get; set; }
        public long? CaseId { get; set; }
        public long? AppointmentId { get; set; }
        public long? EstimateId { get; set; }
        public IList<long> TaskIds { get; set; }
        public string Note { get; set; }
        public string NoteDate { get; set; }
        public string NoteType { get; set; }
        public string NoteAudience { get; set; }
        public string NoteUser { get; set; }
        public char? ActionRequired { get; set; }
        public int Version { get; set; }
    }
}
