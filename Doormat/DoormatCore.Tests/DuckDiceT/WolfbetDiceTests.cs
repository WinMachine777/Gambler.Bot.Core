﻿using DoormatCore.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoormatCore.Tests.DuckDiceT
{
    public class DuckDiceDiceTests:DiceTests, IClassFixture<DuckDiceFixture>
    {
        public DuckDiceDiceTests(DuckDiceFixture site):base(site.site)
        {
            
        }
    }
}
