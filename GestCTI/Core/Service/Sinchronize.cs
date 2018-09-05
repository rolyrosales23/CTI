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
using GestCTI.Util;

namespace GestCTI.Core.Service
{
    public class Sinchronize
    {
        private static void loadSkills() {
            DBCTIEntities db = new DBCTIEntities();

            String result = ServiceCoreHttp.SkillList().Result;
            JObject json = JObject.Parse(result);
            JArray Group_Number = (JArray)json["Group_Number"];
            JArray Group_Name = (JArray)json["Group_Name"];
            JArray Group_Extension = (JArray)json["Group_Extension"];

            List<Skills> actual = db.Skills.ToList();
            for (int i = 0; i < Group_Number.Count; i++) {
                String number = Group_Number[i].ToString();
                String name = Group_Name[i].ToString();
                String extension = Group_Extension[i].ToString();

                Skills skill = db.Skills.FirstOrDefault(s => s.Value == number);
                if (skill == null)
                {
                    skill = new Skills();
                    skill.Value = number;
                    skill.Description = name;
                    skill.Extension = extension;
                    db.Skills.Add(skill);
                }
                else {
                    skill.Description = name;
                    skill.Extension = extension;
                    actual.RemoveAll(s => s.Value == skill.Value);
                }
            }

            for (int i = 0; i < actual.Count(); i++)
                db.Skills.Remove(actual[i]);

            db.SaveChanges();
        }

        private static void loadAgents() {
            DBCTIEntities db = new DBCTIEntities();

            String result = ServiceCoreHttp.AgentList().Result;
            JObject json = JObject.Parse(result);
            JArray Login_ID = (JArray)json["Login_ID"];
            JArray Name = (JArray)json["Name"];

            List<Users> actual = db.Users.Where(u => u.Role == "agent").ToList();
            for (int i = 0; i < Login_ID.Count; i++)
            {
                String username = Login_ID[i].ToString();
                String firstname = Name[i].ToString();

                Users user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    user = new Users();
                    user.Username = username;
                    user.FirstName = firstname;
                    user.Active = true;
                    user.Role = "agent";
                    user.Password = Seguridad.EncryptMD5( getAgentPassword(username) );
                    //user.IdLocation = null;
                    //user.IdCompany = null;
                    db.Users.Add(user);
                }
                else
                {
                    user.FirstName = firstname;
                    user.Password = Seguridad.EncryptMD5(getAgentPassword(username));
                    user.Active = true;
                    actual.RemoveAll(u => u.Username == user.Username);
                }
            }

            for (int i = 0; i < actual.Count(); i++)
                actual[i].Active = false;

            db.SaveChanges();
        }

        private static void loadAgentsSkills() {
            DBCTIEntities db = new DBCTIEntities();
            List<String> agents = db.Users.Where(u => u.Role == "agent" && u.Active).Select(u => u.Username).ToList<String>();

            String result = ServiceCoreHttp.AgentGetSkills(agents).Result;
            JArray lista = JArray.Parse(result);

            List<UserSkill> actual = db.UserSkill.Where(us => us.Users.Active).ToList();
            for (int i = 0; i < lista.Count; i++)
            {
                String username = lista[i]["Login_ID"].ToString();
                JArray skillNumber = (JArray)lista[i]["SN"];
                JArray skillLevel = (JArray)lista[i]["SL"];

                Users user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user == null) continue;

                for (int j = 0; j < skillNumber.Count; j++) {
                    String sn = skillNumber[j].ToString();
                    String sl = skillLevel[j].ToString();

                    Skills skill = db.Skills.FirstOrDefault(s => s.Value == sn);
                    if (skill == null) continue;
                    UserSkill userSkill = db.UserSkill.FirstOrDefault(us => us.IdUser == user.Id && us.IdSkill == skill.Id);
                    if (userSkill == null)
                    {
                        userSkill = new UserSkill();
                        userSkill.IdSkill = skill.Id; 
                        userSkill.IdUser = user.Id;
                        userSkill.Level = Int32.Parse(sl);
                        db.UserSkill.Add(userSkill);
                    }
                    else
                    {
                        userSkill.Level = Int32.Parse(sl);
                        actual.RemoveAll(us => us.IdUser == user.Id && us.IdSkill == skill.Id);
                    }
                }
            }

            for (int i = 0; i < actual.Count(); i++)
                db.UserSkill.Remove(actual[i]);

            db.SaveChanges();
        }

        private static String getAgentPassword(String username) {
            String result = ServiceCoreHttp.AgentDetail(username).Result;
            JObject json = JObject.Parse(result);
            return json["Password"].ToString();
        }

        public static void sincronizar() {
            loadSkills();
            loadAgents();
            loadAgentsSkills();
        }
    }
}