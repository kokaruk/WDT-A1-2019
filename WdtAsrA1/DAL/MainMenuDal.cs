using System;
using System.Collections.Generic;
using System.Linq;
using WdtAsrA1.Model;

namespace WdtAsrA1.DAL
{
    /// <summary>
    /// main menu data accessor
    /// implements singleton pattern
    /// </summary>
    public class MainMenuDal : IMainMenuDal
    {
        private IEnumerable<Room> _rooms;

        public IEnumerable<Room> Rooms
        {
            get
            {
                // ReSharper disable once InvertIf
                if (_rooms == null || !_rooms.Any())
                {
                    var table = _dbFacade.GetDataTable("all rooms");
                    _rooms = table.Select().Select(x =>
                        new Room {RoomID = (string) x["RoomID"]}
                    ).ToList();
                }

                return _rooms;
            }
        }

        public IEnumerable<Slot> Slots(DateTime date)
        {
            var connParams = new Dictionary<string, dynamic>
            {
                {"Start", date.Date},
                {"End", date.AddDays(1).Date}
            };
            var table = _dbFacade.GetDataTable("list slots for date", connParams);
            var items = table.Select().Select(x =>
                new Slot
                {
                    BookedInStudentId = (string) x["BookedInStudentId"],
                    RoomID = (string) x["RoomID"],
                    StaffID = (string) x["StaffID"],
                    StartTime = (DateTime) x["StartTime"]
                }).ToList();
            return items;
        }

        private static readonly Lazy<IMainMenuDal> _instance = new Lazy<IMainMenuDal>(
            () => new MainMenuDal());

        public static readonly IMainMenuDal Instance = _instance.Value;

        private readonly IDalDbFacade _dbFacade = DalDbFacade.Instance;

        // private constructor
        private MainMenuDal()
        {
        }
    }
}