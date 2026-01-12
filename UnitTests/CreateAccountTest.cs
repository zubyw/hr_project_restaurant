using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project.DataAccess;
using Project.DataModels;
using System.Linq;
using Project.Logic;

namespace UnitTests;
[TestClass]
public sealed class CreateAccount
{

    private static UsersAccess _userAccess = new UsersAccess();
    private static UsersLogic _userLogic = new UsersLogic();

    [DataTestMethod]
    
    [DataRow("kevin", "Jan", "0634212344", "Kevin@kevin.nl", "kevinjan", "customer")] 
    [DataRow("Casper", "Jan", "0634212345", "casper@kevin.nl", "casperjan", "customer")] 
    [DataRow("hendrick", "Jan", "0634212346", "hendrick@hendrick.nl", "hendrickjan", "customer")] 
    public void CreateAccountValidCredentials(string f, string l, string ph,  string m, string p, string r)
    {
        // arrange
        UserModel TestUser = new UserModel
        {
            FirstName = f,
            LastName = l,
            PhoneNumber = ph,
            EmailAddress = m,
            Password = p,
            Roles = r
        };
        _userLogic.CreateUser(TestUser);

        // act 
        UserModel result = _userLogic.CheckLogin(m, p);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(m, result.EmailAddress);
        Assert.AreEqual(p, result.Password);
    }

    [DataTestMethod]
    [DataRow("","","","kevin@kevin.nl","", "customer")]
    [DataRow("","krul","","","", "customer")]
    [DataRow("capvin","","0634093711","","", "customer")]
    public void CreateAccountInvalidCredentials(string f, string l, string ph,  string m, string p, string r)
    {
        // arrange
        UserModel TestUser = new UserModel
        {
            FirstName = f,
            LastName = l,
            PhoneNumber = ph,
            EmailAddress = m,
            Password = p,
            Roles = r
        };
        _userLogic.CreateUser(TestUser);
        // act 
        UserModel? result = _userLogic.CheckLogin(m, p);

        // assert
        Assert.IsNull(result);
    }
}
