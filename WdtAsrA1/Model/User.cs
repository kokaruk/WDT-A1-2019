namespace WdtAsrA1.Model
{
    public class User
    {
        private string Name { get; }
        public UserType UserType { get; protected set; }
        protected User(string name)
        {
            Name = name;
        }
    }

    public class Staff : User
    {
        public Staff(string name) : base(name)
        {
            UserType = UserType.Staff;
        }
    }

    public class Student : User
    {
        public Student(string name) : base(name)
        {
            UserType = UserType.Student;
        }
    }
}