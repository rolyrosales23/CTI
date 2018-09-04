using GestCTI.Core.Util;
using GestCTI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace GestCTI.Core.Service
{
    public class Sinchronize
    {
        public static void loadSkills() {
            DBCTIEntities db = new DBCTIEntities();

            String result = ServiceCoreHttp.SkillList().Result;
            JObject json = JObject.Parse(result);
            JArray Group_Number = (JArray)json["Group_Number"];
            JArray Group_Name = (JArray)json["Group_Name"];
            JArray Group_Extension = (JArray)json["Group_Extension"];
            
            for (int i = 0; i < Group_Number.Count; i++) {
                Skills skill = new Skills();
                skill.Value = Group_Number[i].ToString();
                skill.Description = Group_Name[i].ToString();
                skill.extension = Group_Extension[i].ToString();
                db.Skills.Add(skill);
                db.SaveChanges();
            }
        }

    }
}