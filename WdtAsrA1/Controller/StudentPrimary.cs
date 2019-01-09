using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WdtAsrA1.DAL;
using WdtAsrA1.Model;
using WdtAsrA1.Utils;

namespace WdtAsrA1.Controller
{
    // instance of class is created via static reflection in BaseController
    // ReSharper disable once UnusedMember.Global
    internal class StudentPrimary : MenuControllerAdapter
    {
        public StudentPrimary(BaseController parent) : base(parent)
        {
            MenuHeader = $"({((MainMenuController) Parent).CurrentUserType}) Main Menu";
        }

        protected override void GetInput()
        {
            while (true)
            {
                var maxInput = BuildMenu(out var menu);
                var option = GetInput(menu.ToString(), maxInput);
                if (option == maxInput) Parent.Start(); // go back 
                switch (option)
                {
                    case -1: // empty input, go back
                        Parent.Start();
                        break;
                    case 1:
                        ListUsers('s');
                        break;
                    case 2:
                        StaffAvailability();
                        break;
                    case 3:
                        MakeBooking();
                        break;
                    case 4:
                        CancelBooking();
                        break;
                    default:
                        continue;
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
                                     $"{++counter}. List students{Environment.NewLine}" +
                                     $"{++counter}. Staff availability{Environment.NewLine}" +
                                     $"{++counter}. Make booking{Environment.NewLine}" +
                                     $"{++counter}. Cancel booking{Environment.NewLine}" +
                                     $"{++counter}. Previous Menu{Environment.NewLine}");

            //append messenger
            menu.Append($"{Environment.NewLine}{Message}{Environment.NewLine}");

            return counter;
        }

        private void StaffAvailability()
        {
            var date = GetDate("Enter date for staff availability (dd-mm-yyyy): ");
            ListUsers('e');
            var staff = GetUser('e');
            var staffBookings = DalFacade.SlotDal
                .SlotsForDate(date)
                .ToList()
                .FindAll(slot => slot.StaffID.Equals(staff.UserID) && string.IsNullOrWhiteSpace(slot.BookedInStudentId));
            if (staffBookings.Any())
            {
                const string format = "{0}{1,-8}{2,-16}{3,-16}";
                var slotList =
                    new StringBuilder($"{Environment.NewLine}Staff {staff.UserID} availability on {date:d-MM-yyy}:");
                slotList.Append(
                    string.Format(format,
                        Environment.NewLine,
                        "Room",
                        "Start time",
                        "End time")
                );

                staffBookings.ForEach(slot =>
                    slotList.Append(
                        string.Format(
                            format,
                            Environment.NewLine,
                            slot.RoomID,
                            $"{slot.StartTime:hh:mm tt}",
                            $"{slot.StartTime.AddMinutes(Program.SlotDuration):hh:mm tt}")));

                Message = slotList.ToString();
            }
            else
            {
                Message = $"<No slots available for {staff.UserID}> on {date:d-MM-yyyy}";
            }
        }

        private void MakeBooking()
        {
            var date = GetDate("Enter date for slot (dd-mm-yyyy): ");
            ListUsers('e');
            var staff = GetUser('e');
            ListUsers('s');
            var student = GetUser('s');

            // check if student already has booking for this day
            if (DalFacade.SlotDal
                .SlotsForDate(date)
                .Any(slot => slot.BookedInStudentId.Equals(student.UserID)))
            {
                Message = "Student has already booked slot in this day";
                return;
            }
                
            
            var staffBookings = DalFacade.SlotDal
                .SlotsForDate(date)
                .ToList()
                .FindAll(slot => slot.StaffID.Equals(staff.UserID) && string.IsNullOrWhiteSpace(slot.BookedInStudentId));
            if (staffBookings.Any())
            {
                var slotsView = BuildSlotsList(staffBookings, staff, date);
                var option = GetInput(slotsView.ToString(), staffBookings.Count);
                var candidateSlot = staffBookings[--option];
                DalFacade.SlotDal.BookSlot(candidateSlot, student);

                Message = "Slot booked successfully";
            }
            else
            {
                Message = $"<No slots available for {staff.UserID}> on {date:d-MM-yyyy}";
            }
            
        }
        
        private void CancelBooking()
        {
            var date = GetDate("Enter date for slot (dd-mm-yyyy): ");
            ListUsers('e');
            var staff = GetUser('e');
            var staffBookings = DalFacade.SlotDal
                .SlotsForDate(date)
                .ToList()
                .FindAll(slot => slot.StaffID.Equals(staff.UserID) && !string.IsNullOrWhiteSpace(slot.BookedInStudentId));
            if (staffBookings.Any())
            {
                var slotsView = BuildTakenSlotsList(staffBookings, staff, date);
                var option = GetInput(slotsView.ToString(), staffBookings.Count);
                var candidateSlot = staffBookings[--option];
                DalFacade.SlotDal.UnbookSlot(candidateSlot);
                
                Message = "Slot cancelled successfully";
            }
            else
            {
                Message = $"<No booked slots available for {staff.UserID}> on {date:d-MM-yyyy}";
            }
            
        }

        private StringBuilder BuildSlotsList(List<Slot> staffBookings, User staff, DateTime date)
        {
            const string format = "{0}{1, -3}{2,-8}{3,-16}{4,-16}";
            var slotList =
                new StringBuilder($"{Environment.NewLine}Staff {staff.UserID} availability on {date:d-MM-yyy}:");
            var count = 0;
            slotList.Append(
                string.Format(format,
                    Environment.NewLine,
                    "#",
                    "Room",
                    "Start time",
                    "End time")
            );

            staffBookings.ForEach(slot =>
                slotList.Append(
                    string.Format(
                        format,
                        Environment.NewLine,
                        $"{++count}.",
                        slot.RoomID,
                        $"{slot.StartTime:hh:mm tt}",
                        $"{slot.StartTime.AddMinutes(Program.SlotDuration):hh:mm tt}")));

            return slotList;
        }
        
        private StringBuilder BuildTakenSlotsList(List<Slot> staffBookings, User staff, DateTime date)
        {
            const string format = "{0}{1, -3}{2,-8}{3,-16}{4,-16}{5, -8}";
            var slotList =
                new StringBuilder($"{Environment.NewLine}Staff {staff.UserID} bookings on {date:d-MM-yyy}:");
            var count = 0;
            slotList.Append(
                string.Format(format,
                    Environment.NewLine,
                    "#",
                    "Room",
                    "Start time",
                    "End time",
                    "Student Id")
            );

            staffBookings.ForEach(slot =>
                slotList.Append(
                    string.Format(
                        format,
                        Environment.NewLine,
                        $"{++count}.",
                        slot.RoomID,
                        $"{slot.StartTime:hh:mm tt}",
                        $"{slot.StartTime.AddMinutes(Program.SlotDuration):hh:mm tt}",
                        slot.BookedInStudentId)));

            return slotList;
        }
        
    }
}