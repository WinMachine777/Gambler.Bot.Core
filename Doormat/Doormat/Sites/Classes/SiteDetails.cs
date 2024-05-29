﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Gambler.Bot.Core.Sites.Classes
{
    public class SiteDetails
    {
        [Key]
        public string name { get; set; }
        public decimal edge { get; set; }
        public decimal maxroll { get; set; }
        public bool cantip { get; set; }
        public bool tipusingname { get; set; }
        public bool canwithdraw { get; set; }
        public bool canresetseed { get; set; }
        public bool caninvest { get; set; }
        public string siteurl { get; set; }
        public string[] Currencies { get; set; }
        public string[] Games { get; set; }

    }
}
