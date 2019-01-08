using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using WdtAsrA1.DAL;
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
            while (true)
            {
                var maxInput = BuildMenu(out var menu);
                var option = GetInput(menu.ToString());
                if (option == maxInput) Parent.Start(); // go back 
                switch (option)
                {
                    case -1: // empty input, go back
                        Parent.Start();
                        break;
                    case 1:
                        ListStaff();
                        break;
                    case 2:
                        RoomAvailability();
                        break;
                    case 3:
                        CreateSlot();
                        break;
                    case 4:
                        RemoveSlot();
                        break;
                    default:
                        return;
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
                                     $"{++counter}. Previous Menu{Environment.NewLine}");

            //append messenger
            menu.Append($"{Environment.NewLine}{Message}{Environment.NewLine}");

            return counter;
        }

        private void ListStaff()
        {
            var staff = DalProxy.StaffMenu.Users.ToList();
            if (staff.Any())
            {
                var staffList = new StringBuilder($"{Environment.NewLine} --- all staff ---");
                staffList.Append(
                    $"{Environment.NewLine}{"User Id",-11}{"Name",-11}Email");
                staff
                    .ForEach(user =>
                        staffList.Append(
                            $"{Environment.NewLine}{user.UserID,-11}{user.Name,-11}{user.Email}")
                    );
                Message = staffList.ToString();
            }
            else
            {
                Message = "<no staff found";
            }
        }

        private void RoomAvailability()
        {
            
        }

        private void CreateSlot()
        {
            throw new NotImplementedException();
        }

        private void RemoveSlot()
        {
            throw new NotImplementedException();
        }
        

    }
}