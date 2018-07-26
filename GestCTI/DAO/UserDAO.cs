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
            var res = db.Users.SingleOrDefault(a=>a.User == user.User && a.Password == user.Password);

            return res;
        }

        public static IEnumerable<object> GetAllRolls()
        {
            db = new DBCTIEntities();
            return db.Rolls.Select(p => new { IdRoll = p.IdRoll, Description = p.Description});
        }
    }
}