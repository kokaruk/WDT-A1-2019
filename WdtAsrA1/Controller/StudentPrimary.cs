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
    }
}