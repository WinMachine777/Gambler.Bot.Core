﻿using Gambler.Bot.Common.Games.Dice;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gambler.Bot.Common.Helpers
{
    public class SiteDetails
    {
        [Key]
        public string name { get; set; }
        public string Name { get=>name; set=>name=value; }
        public decimal edge { get; set; }
        public decimal maxroll { get; set; }
        public bool cantip { get; set; }
        public bool tipusingname { get; set; }
        public bool canwithdraw { get; set; }
        public bool canresetseed { get; set; }
        public bool caninvest { get; set; }
        public bool canbank { get; set; }
        public string siteurl { get; set; }
        public string[] Currencies { get; set; }
        public string[] Games { get; set; }
        public bool NonceBased { get; set; }
        public Dictionary<string,IGameConfig> GameSettings { get; set; }

    }
}
