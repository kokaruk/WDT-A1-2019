using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using WdtAsrA1.DAL;
using WdtAsrA1.Model;
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


        private void RoomAvailability()
        {
            Message = string.Empty;
            var room = GetRoom();
            var date = GetDate();
            try
            {
                var slots = room == null
                    ? DalFactory.SlotDal.SlotsForDate(date).ToList()
                    : DalFactory.SlotDal.SlotsForDate(date).ToList()
                        .FindAll(slot => slot.StartTime.Date.Equals(date.Date));
                if (slots.Any())
                {
                    var slotsListOutput = new StringBuilder();
                    slotsListOutput.SlotsListOutput(slots);
                    Message = slotsListOutput.ToString();
                }
                else
                {
                    Message = "All slots are empty";
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("<No Rooms>");
            }
        }


        private void CreateSlot()
        {
            var room = GetRoom(allowEmpty: false);
            var date = GetDate();
            if (date.Date.Equals(DateTime.Now.Date) && DateTime.Now.Hour >= Program.WorkingHoursEnd)
            {
                Message = "School closed today already";
                return;
            }

            var timeDateTime = GetTime(date);
            var dateCombined = date.Date.Add(timeDateTime.TimeOfDay);

            // each room maximum 2 slots per day
            var roomBookings = DalFactory.SlotDal
                .SlotsForDate(dateCombined)
                .ToList()
                .FindAll(slot => slot.RoomID.Equals(room.RoomID) && slot.StartTime.Date.Equals(dateCombined.Date));
            if (roomBookings.Count >= Program.DailyRoomBookings)
            {
                Message = "This room reached Maximum daily capacity";
                return;
            }

            // also check if room already booked at this time
            var roomIsBooked = DalFactory.SlotDal
                .SlotsForDate(dateCombined)
                .Any(slot => slot.RoomID.Equals(room.RoomID)
                             && slot.StartTime.Date == dateCombined.Date
                             && slot.StartTime.Hour == dateCombined.Hour);
            if (roomIsBooked)
            {
                Message = "This slot already exists";
                return;
            }

            ListStaff();
            var staff = GetStaff();

            // check constraints
            // staff can have max 4 slots a day
            var staffBookings = DalFactory.SlotDal
                .SlotsForDate(dateCombined)
                .ToList()
                .FindAll(slot => slot.StaffID.Equals(staff.UserID));
            if (staffBookings.Count >= Program.DailyStaffBookings)
            {
                Message = "Staff Member is overbooked for this day";
                return;
            }

            DalFactory.SlotDal.CreateSlot(room.RoomID, dateCombined, staff.UserID);
            Message = "New slot added";
        }


        private void RemoveSlot()
        {
            ListStaff();
            const string message = "No upcoming slots for staff member";
            var staff = GetStaff();
            try
            {
                var slots = DalFactory.SlotDal
                    .SlotsForStaff(staff).ToList();

                if (slots.Any())
                {
                    var slotsView = BuilSlotsList(slots);
                    var option = GetInput(slotsView.ToString(), slotsView.Length);
                    var candidateSlot = slots[--option];
                    if (string.IsNullOrWhiteSpace(candidateSlot.BookedInStudentId))
                    {
                        DalFactory.SlotDal.DeleteSlot(candidateSlot);
                        Message = "Slot removed successfully";
                    }
                    else
                    {
                        Message = "Can't delete booked slot";
                    }
                }
                else
                {
                    Message = message;
                }
            }
            catch (SqlException)
            {
                Message = message;
            }
        }

        

        private StringBuilder BuilSlotsList(List<Slot> slots)
        {
            var slotsView = new StringBuilder();
            slotsView.Append($"{Environment.NewLine} --- List slots ---");

            slotsView.Append(
                $"{Environment.NewLine}{"#",-3}{"Room",-7}{"Start time",-22}{"End time",-16}Bookings");
            var rowNum = 0;
            slots
                .ForEach(s =>
                    slotsView.Append(
                        $"{Environment.NewLine}{++rowNum,-3}{s.RoomID,-7}{$"{s.StartTime:dd-MM-yyyy hh:mm tt}",-22}{$"{s.StartTime.AddMinutes(Program.SlotDuration):hh:mm tt}",-16}{s.BookedInStudentId}"));

            return slotsView;
        }
    }
}