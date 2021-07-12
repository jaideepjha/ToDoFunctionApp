using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ToDOFunctionApp.Models
{
    public class Item
    {
        public string id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public bool completed { get; set; }

    }
}
