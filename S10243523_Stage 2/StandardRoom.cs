using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10243523_Stage_2
{
    [Serializable]
    internal class StandardRoom : Room
    {

        public bool RequireWifi { get; set; } = false;

        public bool RequireBreakfast { get; set; } = false;

        public StandardRoom() : base() { }
        public StandardRoom(int rn, string bc, double dr, bool ia) : base(rn, bc, dr, ia)
        {
            this.RoomNumber = rn;
            this.BedConfiguration = bc;
            this.DailyRate = dr;
            this.IsAvail = ia;
        }
        public override double CalculateCharge()
        {
            int wifirate = 0;
            int breakfastrate = 0;
            if (this.RequireWifi == true)
            {
                wifirate = 10;
            }
            if (this.RequireBreakfast == true)
            {
                breakfastrate = 20;
            }
            double dailycost = this.DailyRate + wifirate + breakfastrate;
            return dailycost;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
