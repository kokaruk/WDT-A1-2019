using System;

namespace WdtAsrA1.DAL
{
    /// <summary>
    /// DAL. Returns instances of data Accessors
    /// And performs proxy functions for various sql operations
    /// </summary>
    public static class DalProxy
    {
        private static readonly Lazy<IMainMenuDal> _mainMenu = new Lazy<IMainMenuDal>(() => MainMenuDal.Instance);
        public static IMainMenuDal MainMenu => _mainMenu.Value;
    }
}