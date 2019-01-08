using System;
using System.Collections.Generic;
using WdtAsrA1.Model;

namespace WdtAsrA1.DAL
{
    public interface IStaffDal
    {
        IEnumerable<User> StaffUsers { get; }
        IEnumerable<Slot> Slots(DateTime date, Room room);
        void CreateSlot(string RoomID, DateTime StartTime, string StaffID);
    }
}