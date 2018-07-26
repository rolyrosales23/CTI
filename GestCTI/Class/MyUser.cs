using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI
{
    public class MyUser
    {
        int idUser;
        string user;
        int idRoll;
        string roll;
        string firstName;
        string lastName;
        int idState;
        string state;
        string password;

        public string User { get => user; set => user = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string Roll { get => roll; set => roll = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string State { get => state; set => state = value; }
        public int IdUser { get => idUser; set => idUser = value; }
        public int IdRoll { get => idRoll; set => idRoll = value; }
        public int IdState { get => idState; set => idState = value; }
        public string Password { get => password; set => password = value; }
    }
}