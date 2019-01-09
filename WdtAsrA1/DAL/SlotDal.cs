using System;
using System.Collections.Generic;
using System.Linq;
using WdtAsrA1.Model;

namespace WdtAsrA1.DAL
{
    /// <summary>
    /// slots db layer
    /// implements singleton pattern
    /// </summary>
    public class SlotDal : ISlotDal
    {
        

        public static ISlotDal Instance => _instance.Value;

        private static readonly Lazy<ISlotDal> _instance = new Lazy<ISlotDal>(
            () => new SlotDal());



        private SlotDal()
        {
        }

        
        public IEnumerable<Slot> SlotsForDate(DateTime date)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"Start", date.Date},
                {"End", date.AddDays(1).Date}
            };
            var table = DalDbFacade.Instance.GetDataTable("list slots for date", connParams);
            var items = table.Select().Select(x =>
                new Slot
                {
                    BookedInStudentId =  x["BookedInStudentId"] == DBNull.Value
                        ? string.Empty
                        : (string) x["BookedInStudentId"],
                    RoomID = (string) x["RoomID"],
                    StaffID = (string) x["StaffID"],
                    StartTime = (DateTime) x["StartTime"]
                }).ToList();
            return items;
        }
        
        public IEnumerable<Slot> SlotsForStaff(User staff)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"StaffID", staff.UserID},
                {"CurrentTime", DateTime.Now}
            };
            var table = DalDbFacade.Instance.GetDataTable("list slots for staff", connParams);
            var items = table.Select().Select(x =>
                new Slot
                {
                    BookedInStudentId =  x["BookedInStudentId"] == DBNull.Value
                        ? string.Empty
                        : (string) x["BookedInStudentId"],
                    RoomID = (string) x["RoomID"],
                    StaffID = (string) x["StaffID"],
                    StartTime = (DateTime) x["StartTime"]
                }).ToList();
            return items;
        }
        
        public void CreateSlot(string roomId, DateTime startTime, string staffId)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"RoomID", roomId},
                {"StartTime", startTime},
                {"StaffID", staffId}
            };

            DalDbFacade.Instance.ExecuteNonQuery("add new slot", connParams);
        }

        public void DeleteSlot(Slot slot)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"RoomID", slot.RoomID},
                {"StartTime", slot.StartTime}
            };

            DalDbFacade.Instance.ExecuteNonQuery("delete slot", connParams);
        }

        public void BookSlot(Slot slot, User user)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"RoomID", slot.RoomID},
                {"StartTime", slot.StartTime},
                {"StudentID", user.UserID}
            };
            DalDbFacade.Instance.ExecuteNonQuery("book slot", connParams);
        }

        public void UnbookSlot(Slot slot)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"RoomID", slot.RoomID},
                {"StartTime", slot.StartTime}
            };

            DalDbFacade.Instance.ExecuteNonQuery("unbook slot", connParams);
        }
    }
}