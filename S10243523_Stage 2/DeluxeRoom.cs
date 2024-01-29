using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10243523_Stage_2
{
    [Serializable]
    internal class DeluxeRoom : Room
    {
        public bool AdditionalBed { get; set; } = false;
        public DeluxeRoom() : base() { }
        public DeluxeRoom(int rn, string bc, double dr, bool ia) : base(rn, bc, dr, ia)
        {
            this.RoomNumber = rn;
            this.BedConfiguration = bc;
            this.DailyRate = dr;
            this.IsAvail = ia;
        }
        public override double CalculateCharge()
        {
            int bedrate = 0;
            if (this.AdditionalBed == true)
            {
                bedrate = 25;
            }
            return this.DailyRate + bedrate;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
