using System;
using System.Collections.Generic;
using WdtAsrA1.Model;

namespace WdtAsrA1.DAL
{
    public interface IMainMenuDal
    {
        IEnumerable<Room> Rooms { get; }
        IEnumerable<Slot> Slots(DateTime date);
    }
}