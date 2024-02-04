using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickingGame.Models
{
    public class Json_Template
    {
        public short Factor { get; set; }
        public short Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
    }
    public class Json_Template2
    {
        public int Coins { get; set; }
        public int Premium_Coins { get; set; }
        public string Name { get; set; }
        public string Active_boost { get; set; }
        public int Clicks { get; set; }

        public string Password { get; set; }
    }
}
