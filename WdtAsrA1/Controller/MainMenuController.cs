using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WdtAsrA1.Model;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WdtAsrA1.DAL;
using WdtAsrA1.Utils;


namespace WdtAsrA1.Controller
{
    internal class MainMenuController : BaseController
    {
        internal UserType CurrentUserType { get; private set; }
        private Lazy<BaseController> _primaryController;
        private BaseController PrimaryBaseController => _primaryController.Value;


        internal override void Start()
        {
            CurrentUserType = UserSelect();
            _primaryController = new Lazy<BaseController>(BuildPrimaryController(this));
            PrimaryBaseController.Start();
        }

        /// <summary>
        /// login method
        /// potentially possible to inject real login from console
        /// </summary>
        /// <returns>by default returns fake user</returns>
        private UserType UserSelect()
        {
            Debug.Assert(Program.PrototypeMode);

            Console.Clear();
            const string greetingHeader = "Welcome to Appointment Scheduling and Reservation System";
            Console.WriteLine(greetingHeader);
            Console.WriteLine(greetingHeader.MenuHeaderPad());

            // optional params
            var extraOptions = new List<string> {"List rooms", "List slots"};

            while (true)
            {
                var userLogon = SelectUserMenu(extraOptions);
                var maxInput =
                    Enum.GetNames(typeof(UserType)).Length + extraOptions.Count; // extra two options for listings 
                var option = GetInput(userLogon.ToString(), ++maxInput);
                while (option < 0)
                {
                    option = GetInput(maxInput: ++maxInput);
                }
                if (option == maxInput) Environment.Exit(0);

                switch (option)
                {
                    
                    case 1:
                        ListAllRooms();
                        break;
                    case 2:
                        ListSlots();
                        break;
                    default:
                        var selectedUserNumber = option - extraOptions.Count; // reverse compensation fro first two options
                        return (UserType)selectedUserNumber;
                }
            }
        }

        /// <summary>
        /// simply display all room names
        /// </summary>
        private static void ListAllRooms()
        {
            try
            {
                var header = new StringBuilder($"{Environment.NewLine}--- All rooms---");

                DalProxy
                    .MainMenu
                    .Rooms
                    .ToList()
                    .ForEach(r =>
                        header.Append($"{Environment.NewLine}{r.RoomID}")
                    );
                Console.WriteLine(header);
                Console.WriteLine();
            }
            catch (SqlException)
            {
                Console.WriteLine("<No Rooms>");
                Console.WriteLine();
            }
            
        }


        private static void ListSlots()
        {
            Console.WriteLine();
            var date = GetDate("Enter date for slots (dd-mm-yyyy):");

            try
            {
                var slots = DalProxy
                    .MainMenu
                    .Slots(date)
                    .ToList();


                if (slots.Any())
                {
                    var slotsList = new StringBuilder($"{Environment.NewLine} --- List slots ---");

                    slotsList.Append(
                        $"{Environment.NewLine}{"Room name",-11}{"Start time",-16}{"End time",-16}{"Staff ID",-14}Bookings");


                    slots
                        .ForEach(s =>
                            slotsList.Append(
                                $"{Environment.NewLine}{s.RoomID,-11}{$"{s.StartTime:HH:mm}",-16}{$"{s.StartTime.AddHours(1):HH:mm}",-16}{s.StaffID,-14}{s.BookedInStudentId}")
                        );

                    Console.WriteLine(slotsList);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("<no slots>");
                    Console.WriteLine();
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("<no slots>");
                Console.WriteLine();
            }
            
        }


        /// <summary>
        /// build user type select menu
        /// </summary>
        /// <returns>user logon requests</returns>
        private static StringBuilder SelectUserMenu(List<string> extraOptions)
        {
            const string greetingHeader = "Main Menu";

            // essentially a hacky way to override console design limitation 

            var primaryMenu = new StringBuilder(greetingHeader);
            primaryMenu.Append($"{Environment.NewLine}{greetingHeader.MenuHeaderPad()}");

            var count = 0;

            extraOptions
                .ForEach(opt => primaryMenu.Append($"{Environment.NewLine}{++count}. {opt}"));

            Enum.GetNames(typeof(UserType))
                .ToList()
                .ForEach(userType => primaryMenu.Append($"{Environment.NewLine}{++count}. {userType} menu"));
            
            primaryMenu.Append($"{Environment.NewLine}{++count}. Quit{Environment.NewLine}");
            
            return primaryMenu;
        }
    }
}