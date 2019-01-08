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

        private void ListStaff()
        {
            var staff = DalProxy.StaffMenu.StaffUsers.ToList();
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
            Message = string.Empty;
            var room = GetRoom();
            var date = GetDate();

            try
            {
                var slots = room == null
                    ? DalProxy.MainMenu.Slots(date).ToList()
                    : DalProxy.StaffMenu.Slots(date, room).ToList();

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


        private Room GetRoom(bool allowEmpty = true)
        {
            var header = new StringBuilder($"{Environment.NewLine}--- All rooms---");

            DalProxy
                .MainMenu
                .Rooms
                .ToList()
                .ForEach(r => header.Append($"{Environment.NewLine}{r.RoomID}"));
            Console.WriteLine(header);
            Console.WriteLine(Message);
            string roomId;
            while (true)
            {
                var prompt = allowEmpty ? "Select Room ID (or empty input for all roms): " : "Type Room Name: ";

                Console.Write(prompt);
                roomId = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(roomId) && allowEmpty) return null;
                if (string.IsNullOrWhiteSpace(roomId) && !allowEmpty) continue;
                break;
            }

            try
            {
                return DalProxy.MainMenu.Rooms.First(room => room.RoomID.Equals(roomId.ToUpper()));
            }
            catch (Exception)
            {
                Message = "Incorrect input, try again";
                return GetRoom();
            }
        }


        private User GetStaff()
        {
            Console.WriteLine(Message);
            Console.Write("Enter Staff ID: ");
            string staffId;
            while (true)
            {
                staffId = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(staffId)) break;
            }

            try
            {
                return DalProxy.StaffMenu.StaffUsers.First(staff => staff.UserID.Equals(staffId.ToLower()));
            }
            catch (Exception)
            {
                Message = "Incorrect input, try again";
                return GetStaff();
            }
        }

        private void CreateSlot()
        {
            var room = GetRoom(allowEmpty: false);
            var date = GetDate();
            var timeDateTime = GetTime();
            var dateCombined = date.Date.Add(timeDateTime.TimeOfDay);

            // each room maximum 2 slots per day
            var roomBookings = DalProxy.StaffMenu
                .Slots(dateCombined, room)
                .ToList()
                .FindAll(slot => slot.RoomID.Equals(room.RoomID) && slot.StartTime.Date.Equals(dateCombined.Date));
            if (roomBookings.Count >= Program.DailyRoomBookings)
            {
                Message = "This room reached Maximum daily capacity";
                return;
            }

            // also check if room already booked at this time
            var roomIsBooked = DalProxy.StaffMenu
                .Slots(dateCombined, room)
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
            var staffBookings = DalProxy.MainMenu
                .Slots(dateCombined)
                .ToList()
                .FindAll(slot => slot.StaffID.Equals(staff.UserID));
            if (staffBookings.Count >= Program.DailyStaffBookings)
            {
                Message = "Staff Member is overbooked for this day";
                return;
            }

            DalProxy.StaffMenu.CreateSlot(room.RoomID, dateCombined, staff.UserID);
            Message = "New slot added";
        }


        private static DateTime GetTime(string prompt = "Time (hh am/pm): ")
        {
            var enAu = new CultureInfo("en-AU");
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                var parsedInputTime = DateTime.TryParseExact(input, "h tt", enAu,
                    DateTimeStyles.None, out var dateValue);

                if (parsedInputTime)
                {
                    if (dateValue.Hour >= Program.WorkingHoursStart && dateValue.Hour < Program.WorkingHoursEnd)
                    {
                        if (dateValue.Hour > DateTime.Now.Hour) return dateValue;
                        Console.WriteLine("Select time in future");
                        Console.WriteLine();
                    }
                    else
                    {
                        var startTime = Program.WorkingHoursStart.ToString().PadLeft(2, '0');
                        DateTime.TryParseExact(startTime, "HH", enAu,
                            DateTimeStyles.None, out var start);
                        DateTime.TryParseExact(Program.WorkingHoursEnd.ToString(), "HH", enAu,
                            DateTimeStyles.None, out var end);
                        Console.WriteLine($"Select slot time from working hours of {start:h:mm tt} to {end:h:mm tt}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    Console.WriteLine();
                }
            }
        }


        private void RemoveSlot()
        {
            throw new NotImplementedException();
        }
    }
}