using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10243523_Stage_2
{
    [Serializable]
    internal class Stay
    {
        public DateTime CheckinDate { get; set; }

        public DateTime CheckoutDate { get; set; }


        public List<Room> Roomlist { get; set; } = new List<Room>();
        public Stay() { }
        public Stay(DateTime checkind, DateTime checkoutd)
        {
            this.CheckinDate = checkind;
            this.CheckoutDate = checkoutd;
        }
        public Stay(Stay stay)
        {
            CheckinDate= stay.CheckinDate;
            CheckoutDate= stay.CheckoutDate;
            
        }
        public void AddRoom(Room new_room)
        {
            Roomlist.Add(new_room);
        }
        public double CalculateTotal()
        {
            int days_stayed = CheckoutDate.Subtract(CheckinDate).Days;
            double total_cost = 0;
            foreach (Room room in Roomlist)
            {
                total_cost += room.CalculateCharge();
            }
            return total_cost * days_stayed;
        }
        public override string ToString()
        {
            return this.CheckinDate + "\t" + this.CheckoutDate;
        }
    }
}
