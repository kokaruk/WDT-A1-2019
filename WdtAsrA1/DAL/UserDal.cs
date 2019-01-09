using System;
using System.Collections.Generic;
using System.Linq;
using WdtAsrA1.Model;

namespace WdtAsrA1.DAL
{
    class UserDal : IUserDal
    {
        
        private static readonly Lazy<IUserDal> _instance = new Lazy<IUserDal>(() => new UserDal());
        public static readonly IUserDal Instance = _instance.Value;

        private UserDal()
        {
        }
        
        private IEnumerable<User> _users;
        public IEnumerable<User> StaffUsers
        {
            get
            {
                // ReSharper disable once InvertIf
                if (_users == null || !_users.Any())
                {
                    var table = DalDbFacade.Instance.GetDataTable("list staff");
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
    }
}