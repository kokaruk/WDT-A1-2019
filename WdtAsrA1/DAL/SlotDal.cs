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

        
        public IEnumerable<Slot> Slots(DateTime date)
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
        
        public void CreateSlot(string RoomID, DateTime StartTime, string StaffID)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"RoomID", RoomID},
                {"StartTime", StartTime},
                {"StaffID", StaffID}
            };

            DalDbFacade.Instance.ExecuteNonQuery("add new slot", connParams);
        }
    }
}