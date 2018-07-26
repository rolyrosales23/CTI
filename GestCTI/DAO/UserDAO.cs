using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestCTI.Models;


namespace GestCTI.DAO
{
    public class UserDAO
    {
        public static DBCTIEntities db;

        public static Users VerifyUser(Users user)
        {
            db = new DBCTIEntities();
            var res = db.Users.SingleOrDefault(a=>a.Username == user.Username && a.Password == user.Password);

            return res;
        }

        public static IEnumerable<object> GetAllRolls()
        {
            db = new DBCTIEntities();
            return db.Roles.Select(p => new { IdRoll = p.Id, Description = p.Name});
        }
    }
}