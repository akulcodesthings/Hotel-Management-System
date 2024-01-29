using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10243523_Stage_2
{
    [Serializable]
    internal class Membership
    {
        public string Status { get; set; } = "Ordinary";
        public int Points { get; set; } = 0;

        public Membership() { }
        public Membership(string s, int p)
        {
            Status = s;
            Points = p;
        }
        public void Earnpoints(double amount)
        {
            int point = Convert.ToInt32(amount) / 10;
            Points += point;
            if (Status == "Ordinary" || Status == "Silver")
            {
                if (Points >= 100 && Points < 200)
                {
                    Status = "Silver";
                    Console.WriteLine("You have been promoted to silver.");
                }
                else if (Points >= 200)
                {
                    Status = "Gold";
                    Console.WriteLine("You have been promoted to gold.");
                }
            }
        }
        public bool RedeemPoints(int i)
        {
            if (Status == "Silver" || Status == "Gold")
            {
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return Status + "\t" + Points;
        }
    }
}

