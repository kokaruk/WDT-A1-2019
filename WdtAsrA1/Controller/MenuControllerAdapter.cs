using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using WdtAsrA1.DAL;
using WdtAsrA1.Model;
using WdtAsrA1.Utils;

namespace WdtAsrA1.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    internal abstract class MenuControllerAdapter : BaseController, IMenuController
    {
        public string MenuHeader { get; set; }
        public BaseController Parent { get; }
        private List<MenuControllerAdapter> Children { get; } = new List<MenuControllerAdapter>();

        protected MenuControllerAdapter(BaseController parent)
        {
            Parent = parent;
        }

        internal override void Start()
        {
            Console.Clear();
            GetInput();
        }

        /// <summary>
        /// primary get input to select child sub controller
        /// </summary>
        protected virtual void GetInput()
        {
            var maxInput = BuildMenu(out var menu);
            var option = GetInput(menu.ToString(), maxInput);
            if (option == maxInput || option == -1)
            {
                if (Parent.GetType() == typeof(MainMenuController) && !Program.PrototypeMode)
                {
                    if (option == -1)
                    {
                        Console.Clear();
                        // ReSharper disable once TailRecursiveCall
                        GetInput();
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Parent.Start();
                }
            }
            else
            {
                Children[--option].Start();
            }
        }

        /// <summary>
        /// menu string builder from collection of child controllers
        /// </summary>
        /// <param name="menu"> string builder for menu contents</param>
        /// <returns></returns>
        public int BuildMenu(out StringBuilder menu)
        {
            menu = new StringBuilder(MenuHeader);
            menu.Append($"{Environment.NewLine}{MenuHeader.MenuHeaderPad()}");
            var maxValue = Children.Count + 1;
            if (Children.Count > 0)
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    menu.Append($"{Environment.NewLine}{i + 1}. {Children[i].MenuHeader}");
                }
            }

            menu.Append(Parent.GetType() == typeof(MainMenuController) && !Program.PrototypeMode
                ? $"{Environment.NewLine}{maxValue}. Quit{Environment.NewLine}"
                : $"{Environment.NewLine}{maxValue}. Return to Main Menu{Environment.NewLine}");
            return maxValue;
        }

        /// <summary>
        /// list staff.
        /// sett message to list of staff
        /// </summary>
        protected void ListUsers(char userIdDescriptor)
        {
            var userType = userIdDescriptor == 'e' ? "Staff" : "Students";
            var users = DalFactory.UserDal.GetAllUsers().ToList()
                .FindAll(user => user.UserID.StartsWith(userIdDescriptor));
            if (users.Any())
            {
                const string format = "{0}{1,-11}{2,-11}{3}";

                var usersList = new StringBuilder($"{Environment.NewLine} --- {userType} list  ---");
                usersList.Append(
                    string.Format(format,
                        Environment.NewLine,
                        "Id",
                        "Name",
                        "Email"
                    ));
                users
                    .ForEach(user =>
                        usersList.Append(
                            string.Format(format,
                                Environment.NewLine,
                                user.UserID,
                                user.Name,
                                user.Email)));
                Message = usersList.ToString();
            }
            else
            {
                Message = $"<no {userType} found>";
            }
        }


        /// <summary>
        /// factory method to make a room 
        /// </summary>
        /// <param name="allowEmpty">a hacky way to determine that getting all roomId dependant list</param>
        /// <returns>return room object</returns>
        protected Room GetRoom(bool allowEmpty = true)
        {
            var header = new StringBuilder($"{Environment.NewLine}--- All rooms---");

            DalFactory
                .RoomDal
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
                return DalFactory.RoomDal.Rooms.First(room => room.RoomID.Equals(roomId.ToUpper()));
            }
            catch (Exception)
            {
                Message = "Incorrect input, try again";
                return GetRoom();
            }
        }

        /// <summary>
        /// factory method to make staff object based on id input string
        /// </summary>
        /// <returns>returns staff object</returns>
        protected User GetUser(char userIdDescriptor)
        {
            Console.WriteLine(Message);
            var prompt = userIdDescriptor == 'e' ? "Enter Staff ID: " : "Enter Student ID: ";
            Console.Write(prompt);
            string userId;
            while (true)
            {
                userId = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(userId)) break;
            }

            try
            {
                return DalFactory.UserDal.GetAllUsers().First(user =>
                    user.UserID.Equals(userId.ToLower()) && user.UserID.StartsWith(userIdDescriptor));
            }
            catch (Exception)
            {
                Message = "Incorrect input, try again";
                return GetUser(userIdDescriptor);
            }
        }

        /// <summary>
        /// get time in allowed business hours for a spe
        /// </summary>
        /// <param name="date">date for which time slot is thought</param>
        /// <param name="prompt">overridable user prompt message</param>
        /// <returns></returns>
        protected static DateTime GetTime(DateTime date, string prompt = "Time (hh am/pm): ")
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
                    // check if within allowed working time
                    if (dateValue.Hour >= Program.WorkingHoursStart && dateValue.Hour < Program.WorkingHoursEnd)
                    {
                        // if date today check time is in future
                        if (date.Date.Equals(DateTime.Now.Date) && dateValue.Hour < DateTime.Now.Hour)
                        {
                            Console.WriteLine("Select time in future");
                            Console.WriteLine();
                        }
                        else
                        {
                            return dateValue;
                        }
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
    }
}