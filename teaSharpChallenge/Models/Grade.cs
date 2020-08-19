using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace teaSharpChallenge.Models
{
    public class Grade
    {
        public int courseID { get; set; }
        public string title { get; set; }
        public int credits { get; set; }

        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public decimal? grade { get; set; }
    }
}
