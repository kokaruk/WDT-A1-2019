using System;
using System.Globalization;
using WdtAsrA1.Utils;

namespace WdtAsrA1.Controller
{
    /// <summary>
    /// Base abstract controller, inherited by all Controllers
    /// </summary>
    internal abstract class BaseController
    {
        /// <summary>
        /// message field. required for output of error messages
        /// </summary>
        private string _message = string.Empty;
        protected string Message { 
            get
            {
                var tmp = _message;
                _message = string.Empty;
                return tmp;
            } 
            set => _message = $"{Environment.NewLine}{value}{Environment.NewLine}"; 
        }

        /// <summary>
        /// primary controller factory based on selected user via reflection 
        /// </summary>
        /// <param name="mainMenuController">contains instances of logged on user and namespace string</param>
        /// <returns>instance of Primary Controller or login controller if error is thrown</returns>
        internal static BaseController BuildPrimaryController(MainMenuController mainMenuController)
        {
            try
            {
                var baseNamespaceName = mainMenuController.GetType().Namespace;
                var controllerTypeName =
                    $"{baseNamespaceName}.{mainMenuController.CurrentUserType.ToString()}Primary";
                // use reflection to create instance 
                var controllerType = Type.GetType(controllerTypeName, true);
                var instance = (BaseController) Activator.CreateInstance(controllerType, mainMenuController);
                return instance;
            }
            catch (TypeLoadException)
            {
                return mainMenuController;
            }
        }

        /// <summary>
        /// get user input from menu and max value
        /// </summary>
        /// <param name="menu">menu prompt string</param>
        /// <param name="maxInput">max allowed value</param>
        /// <param name="prompt">Optional param for input prompt, can be overriden from calling function</param>
        /// <param name="allowTextInput">Optional flag param allowing text input (for next / previous pagination)</param>
        /// <returns></returns>
        internal static int GetInput(string menu = "", int maxInput = 99, string prompt = "Enter an option: ",
            bool allowTextInput = false)
        {
            Console.WriteLine(menu);
            
            while (true)
            {
                
                Console.Write(prompt);
                var input = Console.ReadLine();
                // if allowing text input, return negative values 
                // positive values are for selection of numeric options
                if (allowTextInput)
                {
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (input)
                    {
                        case "n":
                        case "N":
                            return -2;
                        case "r":
                        case "R":
                            return -3;
                    }
                }

                // return negative one, requesting function to handle as go up one level
                if (string.Empty.Equals(input)) return -1;
                if (int.TryParse(input, out var option) && option.IsWithinMaxValue(maxInput)) return option;
                Console.WriteLine("Invalid Input");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// get user input and translate to date
        /// </summary>
        /// <returns></returns>
        internal static DateTime GetDate(string prompt = "Type Date")
        {
            var enAu = new CultureInfo("en-AU");
            
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (DateTime.TryParseExact(input, "dd-mm-yyyy", enAu,
                    DateTimeStyles.None, out var dateValue)) return dateValue;
                Console.WriteLine("Invalid Input");
                Console.WriteLine();
            }      
        }
        
        
        /// <summary>
        /// abstract Start method
        /// </summary>
        internal abstract void Start();
    }
}