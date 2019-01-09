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
    public class RoomDal : IRoomDal
    {
        private IEnumerable<Room> _rooms;
        public IEnumerable<Room> Rooms
        {
            get
            {
                // ReSharper disable once InvertIf
                if (_rooms == null || !_rooms.Any())
                {
                    var table = DalDbFacade.Instance.GetDataTable("all rooms");
                    _rooms = table.Select().Select(x =>
                        new Room {RoomID = (string) x["RoomID"]}
                    ).ToList();
                }

                return _rooms;
            }
        }
        
        private static readonly Lazy<IRoomDal> _instance = new Lazy<IRoomDal>(
            () => new RoomDal());
        public static readonly IRoomDal Instance = _instance.Value;

        // private constructor
        private RoomDal()
        {
        }
    }
}