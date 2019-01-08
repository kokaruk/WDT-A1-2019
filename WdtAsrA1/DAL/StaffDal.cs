using System;
using System.Collections.Generic;
using System.Linq;
using WdtAsrA1.Model;

namespace WdtAsrA1.DAL
{
    /// <summary>
    /// staff menu DAL
    /// implements singleton pattern
    /// </summary>
    public class StaffDal : IStaffDal
    {
        private readonly IDalDbFacade _dbFacade = DalDbFacade.Instance;

        private IEnumerable<User> _users;

        public IEnumerable<User> Users
        {
            get
            {
                // ReSharper disable once InvertIf
                if (_users == null || !_users.Any())
                {
                    var table = _dbFacade.GetDataTable("list staff");
                    _users = table.Select().Select(x =>
                        new User
                        {
                            UserID = (string) x["UserID"],
                            Name = (string) x["Name"],
                            Email = (string) x["Email"]
                        }).ToList();
                }

                return _users;
            }
        }

        private static readonly Lazy<IStaffDal> _instance = new Lazy<IStaffDal>(
            () => new StaffDal());

        public static IStaffDal Instance => _instance.Value;

        private StaffDal()
        {
        }
    }
}