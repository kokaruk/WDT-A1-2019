using System;
using System.Collections.Generic;
using WdtAsrA1.Model;

namespace WdtAsrA1.DAL
{
    public interface ISlotDal
    {
        /// <summary>
        /// get slots for a date
        /// </summary>
        /// <param name="date">date of slot</param>
        /// <returns>collection of slots</returns>
        IEnumerable<Slot> Slots(DateTime date);
        
        void CreateSlot(string RoomID, DateTime StartTime, string StaffID);
    }
}