using System.Collections.Generic;
using WdtAsrA1.Model;

namespace WdtAsrA1.DAL
{
    public interface IRoomDal
    {
        IEnumerable<Room> Rooms { get; }
        
    }
}