public class UsersLogic
{
    private UsersAccess _usersAccess = new UsersAccess();

    public bool CreateUser(string firstName, string lastName, string phoneNumber, string emailAddress, string password, string roles)
    {
        try
        {
            // Check if user already exists
            var existingUser = _usersAccess.GetByEmail(emailAddress);
            if (existingUser != null)
            {
                return false; // User already exists
            }

            var newUser = new UserModel
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                EmailAddress = emailAddress,
                Password = password,
                Roles = roles
            };

            _usersAccess.Write(newUser);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public UserModel? GetUserByEmail(string emailAddress)
    {
        try
        {
            return _usersAccess.GetByEmail(emailAddress);
        }
        catch
        {
            return null;
        }
    }

    public UserModel? GetUserById(int id)
    {
        try
        {
            return _usersAccess.GetById(id);
        }
        catch
        {
            return null;
        }
    }

    public List<UserModel> GetAllUsers()
    {
        try
        {
            return _usersAccess.GetAll();
        }
        catch
        {
            return new List<UserModel>();
        }
    }

    public List<UserModel> GetUsersByRole(string role)
    {
        try
        {
            return _usersAccess.GetByRole(role);
        }
        catch
        {
            return new List<UserModel>();
        }
    }

    public bool UpdateUser(UserModel user)
    {
        try
        {
            _usersAccess.Update(user);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteUser(int userId)
    {
        try
        {
            _usersAccess.DeleteById(userId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsValidEmail(string email)
    {
        return email.Contains("@") && email.Contains(".");
    }

    public bool IsValidPhoneNumber(string phoneNumber)
    {
        return phoneNumber.Length >= 10 && phoneNumber.All(char.IsDigit);
    }

    public UserModel? CheckLogin(string email, string password)
    {
        try
        {
            var user = _usersAccess.GetByEmail(email);
            if (user != null && user.Password == password)
            {
                return user;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}
