using System;
using System.Text;
using WdtAsrA1.Utils;

namespace WdtAsrA1.Controller
{
    // instance of class is created via static reflection in BaseController
    // ReSharper disable once UnusedMember.Global
    internal class StaffPrimary : MenuControllerAdapter
    {
        public StaffPrimary(BaseController parent) : base(parent)
        {
            MenuHeader = $"({((MainMenuController) Parent).CurrentUserType}) Main Menu";
        }

        protected override void GetInput()
        {
            var maxInput = BuildMenu(out var menu);
            
            while (true)
            {
                var option = GetInput(menu.ToString());
                if (option == maxInput || option == -1) Parent.Start();// go back 
                switch (option)
                {
                    case 1:
                        
                        break;
                    default:
                        break;
                }
            }
        }

        private new int BuildMenu(out StringBuilder menu)
        {
            Console.Clear();
            var counter = 0;

            menu = new StringBuilder(MenuHeader +
                                     $"{Environment.NewLine}{MenuHeader.MenuHeaderPad()}" +
                                     Environment.NewLine +
                                     $"{++counter}. List staff{Environment.NewLine}" +
                                     $"{++counter}. Room availability{Environment.NewLine}" +
                                     $"{++counter}. Create slot{Environment.NewLine}" +
                                     $"{++counter}. Remove slot{Environment.NewLine}" +
                                     $"{++counter}. Exit{Environment.NewLine}");

            //append messenger
            menu.Append($"{Environment.NewLine}{Message}{Environment.NewLine}"); 

            return counter;
        }
    }
}