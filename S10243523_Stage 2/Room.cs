using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10243523_Stage_2
{
    [Serializable]
    internal abstract class Room
    {
        public int RoomNumber { get; set; }


        public string BedConfiguration { get; set; } = "Bed Configuration";


        public double DailyRate { get; set; }

        public bool IsAvail { get; set; } = true;

        public Room() { }
        public Room(int rn, string bc, double dr, bool ia)
        {
            this.RoomNumber = rn;
            this.BedConfiguration = bc;
            this.DailyRate = dr;
            this.IsAvail = ia;
        }
        public abstract double CalculateCharge();

        public override string ToString()
        {
            return $"{RoomNumber,-15} {BedConfiguration,-20} {DailyRate,-15}";
        }
    }
}
