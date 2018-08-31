﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestCTI.Controllers
{
    public class ScriptsController : Controller
    {
        // GET: Scripts
        public ActionResult Display(String id, String data)
        {
            if (data != null && data != "null")
            {
                JObject json = JObject.Parse(data);

                IEnumerator<KeyValuePair<string, JToken>> e = json.GetEnumerator();
                while (e.MoveNext())
                {
                    ViewData.Add(e.Current.Key, e.Current.Value);
                }
            }

            return View(id);
        }
    }
}