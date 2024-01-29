using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10243523_Stage_2
{
    [Serializable]
    internal class Guest
    {
        public string Name { get; set; } = "Guest Name";

        public string PassportNum { get; set; } = "Guess Passport Number";

        public Stay HotelStay { get; set; } = new Stay();

        public Membership Member { get; set; } = new Membership();

        public bool IsCheckedIn { get; set; } = false;

        public Guest() { }
        public Guest(string n, string p_num, Stay h_stay, Membership m)
        {
            Name = n;
            PassportNum = p_num;
            HotelStay = h_stay;
            Member = m;
        }
        public Guest(Guest guest)
        {
            Name = guest.Name;
            PassportNum = guest.PassportNum;
            HotelStay = new Stay(guest.HotelStay);
        }
        public override string ToString()
        {
            return ($"{Name,-8} {PassportNum,-15} {Member.Status,-19} {Member.Points,-18}");
        }
    }
}

