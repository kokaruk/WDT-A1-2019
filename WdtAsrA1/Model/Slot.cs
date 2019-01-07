using System;

namespace WdtAsrA1.Model
{
    public class Slot
    {
        public string RoomID { get; set; }
        public DateTime StartTime { get; set; }
        public string StaffID { get; set; }
        public string BookedInStudentId { get; set; }
    }
}