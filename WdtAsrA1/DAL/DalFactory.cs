using System;

namespace WdtAsrA1.DAL
{
    /// <summary>
    /// DAL. Returns instances of data Accessors
    /// And performs proxy functions for various sql operations
    /// </summary>
    public static class DalFactory
    {
        private static readonly Lazy<IRoomDal> _roomDal = new Lazy<IRoomDal>(() => DAL.RoomDal.Instance);
        public static IRoomDal RoomDal => _roomDal.Value;
        
        private static readonly Lazy<ISlotDal> _slotDal = new Lazy<ISlotDal>(() => DAL.SlotDal.Instance);
        public static ISlotDal SlotDal => _slotDal.Value;
        
        private static readonly Lazy<IUserDal> _userDal = new Lazy<IUserDal>(() => DAL.UserDal.Instance);
        public static IUserDal UserDal = _userDal.Value;

    }
}