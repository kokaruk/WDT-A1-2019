using System.Collections.Generic;
using WdtAsrA1.Model;

namespace WdtAsrA1.DAL
{
    public interface IStaffDal
    {
        IEnumerable<User> Users { get; }
        
    }
}