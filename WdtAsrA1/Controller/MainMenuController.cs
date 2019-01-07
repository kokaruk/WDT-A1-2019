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
        internal User LoggedOnUser { get; private set; }
        private Lazy<BaseController> _primaryController;
        private BaseController PrimaryBaseController => _primaryController.Value;


        internal override void Start()
        {
            LoggedOnUser = Login();
            _primaryController = new Lazy<BaseController>(BuildPrimaryController(this));
            PrimaryBaseController.Start();
        }

        /// <summary>
        /// login method
        /// potentially possible to inject real login from console
        /// </summary>
        /// <returns>by default returns fake user</returns>
        private User Login()
        {
            Debug.Assert(Program.PrototypeMode);

            Console.Clear();
            const string greetingHeader = "Welcome to Appointment Scheduling and Reservation System";
            Console.WriteLine(greetingHeader);
            Console.WriteLine(greetingHeader.MenuHeaderPad());

            // optional params
            List<string> extraOptions = new List<string> {"List rooms", "List slots"};

            while (true)
            {
                var userLogon = SelectUserMenu(extraOptions);
                var maxInput =
                    Enum.GetNames(typeof(UserType)).Length + extraOptions.Count; // extra two options for listings
                userLogon.Append($"{Environment.NewLine}{++maxInput}. Quit{Environment.NewLine}");
                var option = GetInput(userLogon.ToString(), maxInput);
                if (option < 0) continue;
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
                        option -= extraOptions.Count; // reverse compensation fro first two options
                        var user = UserFactory.MakeUserFromInt(--option);
                        return user;
                }
            }
        }

        /// <summary>
        /// simply display all room names
        /// </summary>
        private static void ListAllRooms()
        {
            var roomsDisplay = new StringBuilder($"{Environment.NewLine}--- All rooms---");

            DalProxy
                .MainMenu
                .Rooms
                .ToList()
                .ForEach(r =>
                    roomsDisplay.Append($"{Environment.NewLine}{r.RoomID}")
                    );
            Console.WriteLine(roomsDisplay);
        }


        private static void ListSlots()
        {
            Console.WriteLine();
            var date = GetDate("Enter date for slots (dd-mm-yyyy):");
            Console.WriteLine(date.Date);
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
            return primaryMenu;
        }
    }
}