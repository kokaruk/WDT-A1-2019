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
        IEnumerable<Slot> SlotsForDate(DateTime date);
        
        /// <summary>
        /// get slots for staff member
        /// </summary>
        /// <param name="staff">staff of slot</param>
        /// <returns>collection of slots</returns>
        IEnumerable<Slot> SlotsForStaff(User staff);
        
        void CreateSlot(string roomId, DateTime startTime, string staffId);

        void DeleteSlot(Slot slot);
    }
}