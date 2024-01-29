// See https://aka.ms/new-console-template for more information
//========================================================== 
// Student Number : S10243523
// Student Name : Akul Arun
//==========================================================
using Microsoft.VisualBasic;
using S10243523_Stage_2;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

List<Room> rooms_list = new();
List<Guest> guest_list = new();
List<Guest> cloned_guest_list = new();
TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
Console.WriteLine("Welcome to ICT Hotel");
// Creating Menu
int DisplayMenu()
{
    Console.WriteLine("-------------MENU--------------");
    Console.WriteLine("[1] List all guests");
    Console.WriteLine("[2] List all available rooms");
    Console.WriteLine("[3] Register guest");
    Console.WriteLine("[4] Check-in guest");
    Console.WriteLine("[5] Display stay details of a guest");
    Console.WriteLine("[6] Extend stay");
    Console.WriteLine("[7] Display monthly charged amounts breakdown & total charged amounts for the year");
    Console.WriteLine("[8] Check-out guest");
    Console.WriteLine("[9] Display Past Reviews");
    Console.WriteLine("[0] Exit");
    Console.WriteLine("-------------------------------");
    while (true) // Validating option number
    {
        try
        {
            Console.Write("Enter option: ");
            int useroption = Convert.ToInt32(Console.ReadLine());
            return useroption;
        }
        catch (FormatException)
        {
            Console.WriteLine("Please enter a number");
            Console.WriteLine();
        }
    }
}


//Basic Feature 1

// Making stay object of a guest by using guest passport number
Stay SearchStay(string p_num)
{
    using (StreamReader SR = new StreamReader("Stays.csv"))
    {
        string? s1 = SR.ReadLine();
        while ((s1 = SR.ReadLine()) != null)
        {
            string[]? eachline = s1.Split(',');
            if (eachline[1] == p_num)
            {
                Stay currstay = new Stay(Convert.ToDateTime(eachline[3]), Convert.ToDateTime(eachline[4])); //fourth and fifth columns check in date and check out dates
                return currstay;
            }
        }
        return null;
    }
}
InitialiseRoomData();

//Adding guest data to guest list
void InitialiseGuestData()
{
    using (StreamReader sr = new StreamReader("Guests.csv"))
    {
        string? s = sr.ReadLine();
        while ((s = sr.ReadLine()) != null)
        {
            string?[] eachlinearray = s.Split(',');
            Membership currentmembership = new Membership(eachlinearray[2], Convert.ToInt32(eachlinearray[3]));
            Stay currentstay = SearchStay(eachlinearray[1]);            // Using SearchStay method to create the stay object using guest passport number
            Guest currentGuest = new Guest(eachlinearray[0], eachlinearray[1], currentstay, currentmembership); // Stay and membership object needed for guest object
            guest_list.Add(currentGuest); 
        }
    }
    foreach (Guest Guest in guest_list)
    {
        using (StreamReader sr6 = new StreamReader("Stays.csv"))
        {
            List<int> roomnumlist = new List<int>(); // To store the room numbers of the rooms the guest stayed in.
            int roomnum1 = 0; // To store first room number found
            int roomnum2 = 0; // To store second room number found
            string? s6 = sr6.ReadLine();
            while ((s6 = sr6.ReadLine()) != null)
            {
                string?[] eachrow = s6.Split(',');
                if (Guest.PassportNum == eachrow[1]) // Matching row to passport number
                {
                    if (eachrow[2] == "TRUE")
                    {
                        Guest.IsCheckedIn = true;
                    }
                    else
                    {
                        Guest.IsCheckedIn = false;
                    }
                    for (int i = 0; i < eachrow.Length; i++)
                    {
                        if (int.TryParse(eachrow[i], out int id))
                        {
                            foreach (Room r in rooms_list)
                            {
                                if (r.RoomNumber == id)
                                {
                                    if (r is StandardRoom)
                                    {
                                        StandardRoom s_r = new StandardRoom(r.RoomNumber, r.BedConfiguration, r.DailyRate, r.IsAvail);
                                        s_r.RequireWifi = Convert.ToBoolean(eachrow[i + 1]);
                                        s_r.RequireBreakfast = Convert.ToBoolean(eachrow[i + 2]);
                                        Guest.HotelStay.AddRoom(s_r);
                                    }
                                    else
                                    {
                                        DeluxeRoom d_r = new DeluxeRoom(r.RoomNumber, r.BedConfiguration, r.DailyRate, r.IsAvail);
                                        d_r.AdditionalBed = Convert.ToBoolean(eachrow[i + 3]);
                                        Guest.HotelStay.AddRoom(d_r);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }
}
InitialiseGuestData();
using (MemoryStream stream = new MemoryStream())
{
    BinaryFormatter formatter = new BinaryFormatter();
    formatter.Serialize(stream, guest_list);
    stream.Position = 0;
    cloned_guest_list = (List<Guest>)formatter.Deserialize(stream);
}

void DisplayGuest(List<Guest> guest_list)
{
    using (StreamReader sr = new StreamReader("Guests.csv"))
    {
        string? s = sr.ReadLine();
        if (s != null)
        {
            string[] heading = s.Split(',');
            Console.WriteLine("{0,-7}  {1,-15} {2,-19} {3,-18} {4}", heading[0], heading[1], heading[2], heading[3], "Check-in Status");
        }

        foreach (Guest guest in guest_list)
        {
            Console.WriteLine($"{guest.ToString()} {guest.IsCheckedIn,-10}");
        }
    }
}
//Basic Feature 2
void AvailRoom(List<Room> rooms_list)
{
    DateTime now = DateTime.Now;
    foreach (Guest guest in guest_list)
    {
        Stay stay = guest.HotelStay;
        List<Room> stay_roomlist = stay.Roomlist;
        foreach (Room room in rooms_list)
        {
            foreach (Room r in stay_roomlist)
            {
                if (room == r)
                {
                    if (now <= guest.HotelStay.CheckinDate || now >= guest.HotelStay.CheckoutDate)
                    {
                        r.IsAvail = true;
                    }
                    else
                    {
                        r.IsAvail = false;
                    }
                }
                else
                {
                    room.IsAvail = true;

                }
            }
        }
    }
}
void RoomIsAvail(DateTime check_in_date, DateTime check_out_date)    //checks if room is available based on user input check in and out date
{

    foreach (Guest guest in guest_list)
    {
        Stay stay = guest.HotelStay;
        List<Room> stay_roomlist = stay.Roomlist;
        foreach (Room r in stay_roomlist)      // runs through the roomlist for each room that a stay object has
        {
            if ((check_out_date <= guest.HotelStay.CheckinDate && check_in_date <= guest.HotelStay.CheckinDate) || (check_in_date >= guest.HotelStay.CheckoutDate && check_out_date >= guest.HotelStay.CheckoutDate))
            {                                //if user check in and out date is not in the date range of existing guest who checked in                                                                                                                                                                                                                   
                r.IsAvail = true;            //set room attribute IsAvail to true
            }
            else                             // else set to false 
            {
                r.IsAvail = false;
            }
        }                                    //for those rooms not assigned to a stay object by default IsAvail will be true  
    }
}
void InitialiseRoomData()
{
    using (StreamReader? sr = new("Rooms.csv"))
    {
        string? s = sr.ReadLine();
        while ((s = sr.ReadLine()) != null)
        {
            string[] data = s.Split(",");
            string room_type = data[0];
            Room? room;
            if (room_type == "Standard")
            {
                room = new StandardRoom(Convert.ToInt32(data[1]), data[2], Convert.ToDouble(data[3]), true);
            }
            else
            {
                room = new DeluxeRoom(Convert.ToInt32(data[1]), data[2], Convert.ToDouble(data[3]), true);
            }
            rooms_list.Add(room);
        }
    }
}

void DisplayRoom(List<Room> rooms_list)
{
    Console.WriteLine($"{"RoomType",-15} {"Room Number",-15} {"Bed Configuration",-20} {"DailyRate"}");
    foreach (Room r in rooms_list)
    {
        if (r.IsAvail)
        {
            if (r is StandardRoom standardRoom)
            {
                Console.WriteLine($"{"Standard",-16}" + standardRoom.ToString());
            }
            else if (r is DeluxeRoom deluxeRoom)
            {
                Console.WriteLine($"{"Deluxe",-16}" + deluxeRoom.ToString());
            }
        }
    }
}

//Basic Feature 3
void RegisterGuest(List<Guest> guest_list, List<Guest> cloned_guest_list)
{
    Console.WriteLine("Registered Guests");
    Console.WriteLine("-----------------");
    DisplayGuest(guest_list);
    string? new_guest_name = "";
    while (true)           // Validating user input for new guest name
    {
        Console.Write("\nEnter the name of the new guest: ");
        new_guest_name = Console.ReadLine();
        new_guest_name = textInfo.ToTitleCase(new_guest_name.ToLower());
        if (new_guest_name == "")
        {
            Console.WriteLine("Your name cannot be empty");
        }
        else if (new_guest_name.Any(char.IsDigit))
        {
            Console.WriteLine("Your name cannot have numbers ");
        }
        else
        {
            break;
        }
    }
    string? new_guest_pnum = "";
    while (true)        // Validating user input for new guest passport number
    {
        bool status = true;
        Console.Write("Enter the Passport number of the new guest: ");
        new_guest_pnum = Console.ReadLine().ToUpper();
        if (new_guest_pnum.Length == 0)
        {
            Console.WriteLine("Please provide your passport number, do not leave it empty.");
        }
        else if (new_guest_pnum.Length == 9)
        {
            if (Char.IsLetter(new_guest_pnum[0]) && Char.IsLetter(new_guest_pnum[new_guest_pnum.Length - 1]))
            {
                foreach (Guest guest in guest_list)
                {
                    if (new_guest_pnum == guest.PassportNum)
                    {
                        status = false;
                        break;
                    }
                }
                if (status == false)
                {
                    Console.WriteLine("Guest is already registered.Please enter a unique passport number.");
                }
                else
                {
                    break;
                }
            }
            else
            {
                Console.WriteLine("First and last character of passport number must be letters");
            }
        }
        else
        {
            Console.WriteLine("Passport number must have 9 characters");
        }
    }
    Membership new_member = new Membership("Ordinary", 0);
    Stay stay = new Stay();
    Guest? new_guest = new(new_guest_name, new_guest_pnum, stay, new_member);
    guest_list.Add(new_guest);
    Guest cloned_registered_guest = new();
    using (MemoryStream stream = new MemoryStream())
    {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, new_guest);
        stream.Position = 0;
        cloned_registered_guest = (Guest)formatter.Deserialize(stream);
    }
    cloned_guest_list.Add(cloned_registered_guest);
    string new_guest_data = new_guest_name + "," + new_guest_pnum + "," + new_member.Status + "," + new_member.Points;
    string new_stay_data = new_guest_name + "," + new_guest_pnum + "," + new_guest.IsCheckedIn + "," + new_guest.HotelStay.CheckinDate.ToShortDateString() + "," + new_guest.HotelStay.CheckoutDate.ToShortDateString() + ",,,,,,,,,";
    using (StreamWriter sw1 = new StreamWriter("Guests.csv", true))
    {
        sw1.WriteLine($"{new_guest_data}");
    }
    using (StreamWriter sw2 = new StreamWriter("Stays.csv", true))
    {
        sw2.WriteLine($"{new_stay_data}");
    }
    Console.WriteLine($"{new_guest_name} has been registered!");
}

//Basic Feature 4
void CheckInGuest(List<Room> rooms_list, List<Guest> guest_list, List<Guest> cloned_guest_list) // check in a guest to hotel
{
    DisplayGuest(guest_list);
    string guest_name = "";
    while (true)
    {
        Console.Write("Select a guest name to check in: ");
        guest_name = Console.ReadLine();
        guest_name = textInfo.ToTitleCase(guest_name.ToLower());
        bool status = false;
        bool check_in_status = false;
        foreach (Guest g in guest_list)         //checks if guest is found in guest list and if check in status is true
        {                                       //if both are true then proceed
            if (guest_name == g.Name)
            {
                Guest guest1 = g;
                status = true;
                if (guest1.IsCheckedIn == false)
                {
                    check_in_status = true;
                    break;
                }
                else
                {
                    Console.WriteLine("This person is already checked in");
                }
            }
        }
        if (check_in_status == true && status == true)
        {
            break;
        }
        else if (status == false)
        {
            Console.WriteLine("Guest not found.Please try again");
        }
    }
    Guest guest = new();
    while (true)
    {
        bool status = false;
        try
        {
            Console.Write("Enter the passport number: ");
            string p_num = Console.ReadLine().ToUpper();
            foreach (Guest g in guest_list)         //check if passport number is found and if passport number matches the person name
            {                                      // if both are true then proceed
                if (p_num == g.PassportNum)
                {
                    guest = g;
                    status = true;
                    break;
                }
            }
            if (status == false)
            {
                Console.WriteLine("Passport number not found.Please try again");
            }
            else
            {
                if (guest.Name == guest_name)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Passport number does not match with the person name");
                }
            }
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    Console.WriteLine(guest_name + " selected ");
    DateTime check_in_date;
    while (true)
    {
        try                                      //check if date is in proper format 
        {
            Console.Write("Enter check in date: ");
            check_in_date = Convert.ToDateTime(Console.ReadLine());
            if (check_in_date < DateTime.Now)
            {
                Console.WriteLine("The date you entered has already passed.Please enter a valid date.");
            }
            else
                break;
        }
        catch (FormatException)
        {
            Console.WriteLine("Date must be in the form of dd/mm/yyyy");
        }
    }
    DateTime check_out_date;
    while (true)
    {
        try                         //check if check out date is after check in date
        {
            Console.Write("Enter check out date: ");
            check_out_date = Convert.ToDateTime(Console.ReadLine());
            if (check_out_date < check_in_date)
            {
                Console.WriteLine("The check_out_date cannot be earlier than the check_in_date.");
            }
            else
                break;
        }
        catch (FormatException)
        {
            Console.WriteLine("Date must be in the form of dd/mm/yyyy");
        }
    }
    RoomIsAvail(check_in_date, check_out_date);
    Console.WriteLine($"\nAvailable Rooms From {check_in_date.ToShortDateString()} To {check_out_date.ToShortDateString()}:");
    Console.WriteLine("------------------------------------");
    DisplayRoom(rooms_list);
    guest.HotelStay.CheckinDate = check_in_date;
    guest.HotelStay.CheckoutDate = check_out_date;
    if (guest.IsCheckedIn == false)
    {
        guest.HotelStay.Roomlist.Clear();
    }
    void SelectRoom()
    {
        int room_no;
        while (true)
        {
            try                                  //check if user input correct format
            {
                bool status = false;
                Console.Write("Select a room to stay in: ");
                room_no = Convert.ToInt32(Console.ReadLine());
                foreach (Room r in rooms_list)         //check if room number is in room_list
                {
                    if (room_no == r.RoomNumber)
                    {
                        status = true;
                        break;
                    }
                }
                if (status == false)
                {
                    Console.WriteLine("Room not found.");
                }
                else
                {
                    break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Please enter a room number");
            }
        }
        foreach (Room r in rooms_list)
        {
            if (room_no == r.RoomNumber)
            {
                if (r is StandardRoom standardRoom)
                {
                    string wifi = "";
                    string breakfast = "";
                    while (true)
                    {
                        try     //validation for y/n responses
                        {
                            Console.Write("Do you require wifi? (Costs $10 per day) [Y/N] : ");
                            wifi = Console.ReadLine().ToUpper();
                            if ((wifi[0] == 'Y' && wifi.Length == 1) || (wifi[0] == 'N' && wifi.Length == 1))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Please input 'Y' or 'N'");
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Please do not leave it empty");
                        }
                    }
                    while (true)
                    {
                        try
                        {
                            Console.Write("Do you require breakfast? (Costs $20 per day) [Y/N] : ");
                            breakfast = Console.ReadLine().ToUpper();
                            if ((breakfast[0] == 'Y' && breakfast.Length == 1) || (breakfast[0] == 'N' && breakfast.Length == 1))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Please input 'Y' or 'N'");
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Please do not leave it empty");
                        }
                    }
                    if (wifi == "Y")
                    {
                        standardRoom.RequireWifi = true;
                    }
                    else
                    {
                        standardRoom.RequireWifi = false;
                    }
                    if (breakfast == "Y")
                    {
                        standardRoom.RequireBreakfast = true;
                    }
                    else
                    {
                        standardRoom.RequireBreakfast = false;
                    }
                    standardRoom.IsAvail = false;
                    guest.HotelStay.AddRoom(standardRoom);
                    break;
                }
                else if (r is DeluxeRoom deluxeRoom)
                {
                    string bed = "";
                    while (true)
                    {
                        try
                        {
                            Console.Write("Do you require additional bed? (Costs $25 per day) [Y/N]: ");
                            bed = Console.ReadLine().ToUpper();
                            if ((bed[0] == 'Y' && bed.Length == 1) || (bed[0] == 'N' && bed.Length == 1))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Please input 'Y' or 'N'");
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Please do not leave it empty");
                        }
                    }
                    if (bed == "Y")
                    {
                        deluxeRoom.AdditionalBed = true;
                    }
                    else
                    {
                        deluxeRoom.AdditionalBed = false;
                    }
                    deluxeRoom.IsAvail = false;
                    guest.HotelStay.AddRoom(deluxeRoom);   // add room to guest stay object               
                }
            }
        }
    }
    SelectRoom();
    string option = "";
    while (true)
    {
        try
        {
            Console.Write("Do you want to select another room? (will cost extra) [Y/N]: ");
            option = Console.ReadLine().ToUpper();
            if ((option[0] == 'Y' && option.Length == 1) || (option[0] == 'N' && option.Length == 1))
            {
                break;
            }
            else
            {
                Console.WriteLine("Please input 'Y' or 'N'");
            }
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Please do not leave it empty");
        }
    }
    if (option == "Y")
    {
        SelectRoom();
    }
    guest.IsCheckedIn = true; //set guest ischecked in to true 
    Guest cloned_chkedin_guest = new Guest();
    using (MemoryStream stream = new MemoryStream())
    {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, guest);
        stream.Position = 0;
        cloned_chkedin_guest = (Guest)formatter.Deserialize(stream);
    }
    cloned_guest_list.Add(cloned_chkedin_guest);
    Console.WriteLine(guest.Name + " is successfully checked in.");
}

//Basic Feature 5
void DisplayStayDetails(List<Guest> guest_list)
{
    Console.WriteLine("Guest Details");
    Console.WriteLine("-------------");
    DisplayGuest(guest_list);
    string guest_name = "";
    string guest_passportnum = "";
    Guest selected_guest = new();
    // Getting guest name
    while (true)
    {
        bool status = false;
        bool check_in_status = false;
        Console.Write("Enter the name of the guest: ");
        guest_name = Console.ReadLine();
        guest_name = textInfo.ToTitleCase(guest_name.ToLower());
        foreach (Guest g in guest_list)
        {
            if (g.Name == guest_name)
            {
                status = true;
                if (g.IsCheckedIn == true)
                {
                    check_in_status = true;
                    selected_guest = g;
                    break;
                }
                else
                {
                    Console.WriteLine("Please check in first"); // Find out guest is checked in
                }
            }
        }
        if (status == true && check_in_status == true)
        {
            break;
        }
        else if (status == false)
        {
            Console.WriteLine("Guest not found.Please try again");
        }
    }
    // Getting passport number of guest
    while (true)
    {
        Guest guest = new();
        Console.Write("Enter the Passport number of the guest: ");
        guest_passportnum = Console.ReadLine().ToUpper();
        bool status = false;
        foreach (Guest g in guest_list)
        {
            if (g.PassportNum == guest_passportnum)
            {
                status = true;
                guest = g;
                break;

            }
        }
        if (status == true)
        {
            if (guest.Name == guest_name)
            {
                break;
            }
            else
            {
                Console.WriteLine("Passport number does not match with person name"); // Finding if guest has the same passport number given
            }
        }
        else
        {
            Console.WriteLine("Passport number not found.Please try again");
        }
    }
    Console.WriteLine($"\nBelow are the details of the stay of {selected_guest.Name}");
    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("\nCheck-In Date:");
    Console.WriteLine(selected_guest.HotelStay.CheckinDate.ToShortDateString()); // Check in date of guest
    Console.WriteLine("\nCheck-Out Date:");
    Console.WriteLine(selected_guest.HotelStay.CheckoutDate.ToShortDateString()); // Check out date of guest
    Console.WriteLine($"\nDuration of {selected_guest.Name}'s stay:");
    Console.WriteLine($"{selected_guest.HotelStay.CheckoutDate.Subtract(selected_guest.HotelStay.CheckinDate).Days} Days"); // Number of days stayed
    Console.WriteLine($"\n{selected_guest.Name}'s rooms");
    Console.WriteLine("-------------");
    //Displaying the rooms the guest stayed in
    foreach (Room room in selected_guest.HotelStay.Roomlist)
    {
        int counter = 0;
        if (room is StandardRoom sroom) //Downcasting to identify the standard rooms
        {
            if (counter == 0)
            {
                Console.WriteLine("\nStandard room");
                Console.WriteLine("-------------");
                Console.WriteLine($"{"Room Number",-14} {"Bed Configuration",-19} {"Daily Rate",-14} {"Wifi",-12} {"Breakfast"}");
            }
            Console.WriteLine($"{sroom.RoomNumber,-14} {sroom.BedConfiguration,-19} {sroom.DailyRate,-14} {sroom.RequireWifi,-12} {sroom.RequireBreakfast}");
        }
        else if (room is DeluxeRoom droom) //Downcasting to identify the deluxe rooms
        {
            if (counter == 0)
            {
                Console.WriteLine("\nDeluxe room");
                Console.WriteLine("-----------");
                Console.WriteLine($"{"Room Number",-14} {"Bed Configuration",-19} {"Daily Rate",-14} {"Extra Bed"}");
            }
            Console.WriteLine($"{droom.RoomNumber,-14} {droom.BedConfiguration,-19} {droom.DailyRate,-14} {droom.AdditionalBed}");
        }
        counter++;
    }
}

//Basic feature 6
void ExtendStay(List<Guest> guest_list)
{
    int roomnum1 = 0;
    int roomnum2 = 0;
    DisplayGuest(guest_list);
    string? guest_name = "";
    Guest guest = new();
    while (true)
    {
        bool status = false;
        Console.Write("Select a guest to extend stay: ");
        guest_name = Console.ReadLine();
        guest_name = textInfo.ToTitleCase(guest_name.ToLower());
        foreach (Guest g in guest_list)
        {
            if (g.Name == guest_name)
            {
                status = true;
                guest = g;
                break;
            }
        }
        if (status == false)
        {
            Console.WriteLine("Guest not found.Please try again.");
        }
        else
        {
            if (guest.IsCheckedIn == true)
            {
                roomnum1 = 0;
                roomnum2 = 0;
                if (guest.HotelStay.Roomlist.Count == 1)
                {
                    roomnum1 = guest.HotelStay.Roomlist[0].RoomNumber;
                }
                else if (guest.HotelStay.Roomlist.Count == 2)
                {
                    roomnum1 = guest.HotelStay.Roomlist[0].RoomNumber;
                    roomnum2 = guest.HotelStay.Roomlist[1].RoomNumber;
                }
            }
            else
            {
                Console.WriteLine("Please check in first");
            }
            break;
        }
    }
    int extend_days;
    while (true)
    {
        bool extend_status = false;
        try
        {
            Console.Write("Enter how many days you want to extend your days: ");
            extend_days = Convert.ToInt32(Console.ReadLine());
            Stay stay = guest.HotelStay;
            DateTime new_chk_out = stay.CheckoutDate = stay.CheckoutDate.AddDays(extend_days);
            foreach (Guest g in guest_list)
            {
                if (guest.HotelStay.CheckinDate <= g.HotelStay.CheckinDate && new_chk_out <= g.HotelStay.CheckinDate || guest.HotelStay.CheckinDate >= g.HotelStay.CheckoutDate && new_chk_out >= g.HotelStay.CheckoutDate)
                {
                    extend_status = true;
                    break;
                }
                else
                {
                    foreach (Room room in g.HotelStay.Roomlist)
                    {
                        if (g.Name != guest.Name)
                        {
                            if (room.RoomNumber == roomnum1)
                            {
                                Console.WriteLine($"Room {roomnum1} has already been booked before your new check out date.");
                                Console.WriteLine($"Please extend your stay till before {g.HotelStay.CheckinDate.ToShortDateString()}.");
                                extend_status = false;
                                break;
                            }
                            else if (room.RoomNumber == roomnum2)
                            {
                                Console.WriteLine($"Room {roomnum2} has already been booked before your new check out date.");
                                Console.WriteLine($"Please extend your stay till before {g.HotelStay.CheckinDate.ToShortDateString()}.");
                                extend_status = false;
                                break;
                            }
                        }

                    }
                }
                if (!extend_status)
                {
                    break;
                }
            }
            if (extend_status == true)
            {
                Console.WriteLine($"The new check-out date is {stay.CheckoutDate.ToShortDateString()}");
                break;
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Please enter an integer.");
        }
    }
}
//Advanced Feature 1
void MonthlyBreakdown(List<Guest> cloned_guest_list)
{
    //Store Monthly Charge of each month
    //Dictionary to store the index of the month and the monthly charge
    Dictionary<int, double> monthlyCharge_dict = new Dictionary<int, double>();
    monthlyCharge_dict.Add(0, 0); // Storing monthly charges for january
    monthlyCharge_dict.Add(1, 0);
    monthlyCharge_dict.Add(2, 0);
    monthlyCharge_dict.Add(3, 0);
    monthlyCharge_dict.Add(4, 0);
    monthlyCharge_dict.Add(5, 0);
    monthlyCharge_dict.Add(6, 0);
    monthlyCharge_dict.Add(7, 0);
    monthlyCharge_dict.Add(8, 0);
    monthlyCharge_dict.Add(9, 0);
    monthlyCharge_dict.Add(10, 0);
    monthlyCharge_dict.Add(11, 0); // Storing monthly charges for December
    // Getting year value
    int year = 0;
    while (true)
    {
        try
        {
            Console.Write("Please enter a year to show the monthly breakdowns and total charged amounts for: ");
            year = Convert.ToInt32(Console.ReadLine());
            if (year.ToString().Length != 4)
            {
                Console.WriteLine("The year can only be made of 4 numbers.");
            }
            else
            {
                break;
            }
        }
        catch(FormatException)
        {
            Console.WriteLine("The year can only be made up of numbers");
        }
        catch(OverflowException)
        {
            Console.WriteLine("Please do not spam numbers");
        }
    }
    
    Console.WriteLine();
    double year_cost = 0;
    // Looping through guest list with past check ins 
    foreach (Guest g in cloned_guest_list)
    {
        if (g.HotelStay.CheckoutDate.Year == year) // Finding if the year guest checked out equals to input year
        {
            int guest_monthnumber = g.HotelStay.CheckoutDate.Month;
            foreach (KeyValuePair<int, double> kvp in monthlyCharge_dict)
            {
                if (kvp.Key == guest_monthnumber - 1) //Finding the month the guest checked out
                {
                    monthlyCharge_dict[kvp.Key] += g.HotelStay.CalculateTotal(); //Finding the value and adding to the value linked to the key of the dict
                }
            }
        }
    }
    foreach (double charge in monthlyCharge_dict.Values)
    {
        year_cost += charge; // Calculating year cost by looping through all values in dict
    }
    List<string> month_list = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                                                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    Console.WriteLine($"\nMonthly Breakdown and Revenue for {year}");
    Console.WriteLine(" ----------------------");
    for (int i = 0; i < month_list.Count; i++)
    {
        foreach (KeyValuePair<int, double> KVP in monthlyCharge_dict)
        {
            if (KVP.Key == i)
            {
                Console.WriteLine($"| {month_list[i]} {year}:\t${KVP.Value}\t|"); //Displaying all the monthly revenue
            }
        }
    }
    Console.WriteLine(" ----------------------");
    Console.WriteLine($"| Total Cost: \t${year_cost}\t|"); // Displaying year revenue
    Console.WriteLine(" ----------------------");
}

//Advanced Feature 2
void CheckOutGuest(List<Guest> guest_list)
{
    DisplayGuest(guest_list);
    Guest guest = new();
    string guest_name = "";
    while (true)
    {
        Console.Write("Select a guest to check out: ");
        guest_name = Console.ReadLine();
        guest_name = textInfo.ToTitleCase(guest_name.ToLower());
        bool status = false;
        bool check_in_status = false;
        foreach (Guest g in guest_list)
        {
            if (g.Name == guest_name)
            {
                guest = g;
                status = true;
                if (guest.IsCheckedIn == true)
                {
                    check_in_status = true;
                    break;
                }
                else
                {
                    Console.WriteLine("You are not checked in and cannot check out.");
                }
            }
        }
        if (check_in_status == true && status == true)
        {
            break;
        }
        else if (status == false)
        {
            Console.WriteLine("Guest not found.Please try again");
        }
    }
    string p_num = "";
    while (true)
    {
        bool status = false;
        try
        {
            Console.Write("Enter the passport number: ");
            p_num = Console.ReadLine().ToUpper();
            foreach (Guest g in guest_list)
            {
                if (p_num == g.PassportNum)
                {
                    guest = g;
                    status = true;
                    break;
                }
            }

            if (status == false)
            {
                Console.WriteLine("Passport number not found.Please try again.");
            }
            else
            {
                if (guest.Name == guest_name)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Passport number does not match with the person name.");
                }
            }
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    double total = 0;
    Stay guest_stay = guest.HotelStay;
    foreach(Guest g in guest_list)
    {
        if(g.PassportNum == p_num)
        {
            total += g.HotelStay.CalculateTotal();
        }
    }
    Console.WriteLine();
    Console.WriteLine(" -------------------------- ");
    Console.WriteLine("| Your total bill is $" + total + "  |");
    Console.WriteLine(" -------------------------- ");
    Membership membership = guest.Member;
    Console.WriteLine($"| Guest Status: {membership.Status}      |");
    Console.WriteLine($"| Guest Points: {membership.Points}        |");
    Console.WriteLine(" -------------------------- ");
    if (membership.Status == "Silver" || membership.Status == "Gold")
    {
        Console.WriteLine("You can redeem points.");
        while (true)
        {
            try
            {
                Console.Write("How much points do you want to redeem to pay for your bill: ");
                int points = Convert.ToInt32(Console.ReadLine());
                bool redeem_points = membership.RedeemPoints(points);
                if (redeem_points == true)
                {
                    total -= points;
                    break;
                }
                else
                {
                    Console.WriteLine("You do not have that much points.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Please enter a number.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Please do not spam numbers.");
            }
        }
    }
    else
    {
        Console.WriteLine("You cannot redeem points due to your membership status");
    }
    Console.WriteLine("Your final bill is " + total);
    Console.WriteLine("Press any key to make payment");
    Console.ReadKey();
    Console.WriteLine("Payment successful.");
    membership.Earnpoints(total);
    Console.WriteLine($"The number of points you have now is {guest.Member.Points}");
    guest.IsCheckedIn = false;
    Console.WriteLine($"{guest.Name} has been successfully checked out.");
    foreach(Room r in guest.HotelStay.Roomlist)
    {
        r.IsAvail = true;
    }
    guest.HotelStay.Roomlist.Clear();
    Console.Write("Would you like to give us a review? [Y/N]: ");
    string user_review = Console.ReadLine().ToUpper();
    if (user_review == "N")
    {
        Console.WriteLine("Hope you enjoyed your stay with us!");
        return;
    }
    else
    {
        Console.Write("Please rate the room(s) on a scale from 1 to 5: ");
        int roomrating = Convert.ToInt32(Console.ReadLine());
        Console.Write("Please rate the service of our staff from 1 to 5: ");
        int servicerating = Convert.ToInt32(Console.ReadLine());
        Console.Write("Please rate our facilities from 1 to 5: ");
        int facilitiesrating = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Thank you for giving us a review. We hope to see you again!");
        string new_review_data = guest.Name+","+guest.PassportNum+","+roomrating+","+servicerating+","+ facilitiesrating;
        using (StreamWriter sw = new StreamWriter("Reviews.csv", true))
        {
            string reviews = $"{guest.Name},{guest.PassportNum},{roomrating},{servicerating},{facilitiesrating}";
            sw.WriteLine(reviews);
        }
    }
}
void DisplayReviews()
{
    using (StreamReader sr = new StreamReader("Reviews.csv"))
    {
        string? s = sr.ReadLine();
        if (s != null)
        {
            string[] heading = s.Split(',');
            Console.WriteLine("{0,-7}  {1,-15} {2,-19} {3,-18} {4}", heading[0], heading[1], heading[2], heading[3], heading[4]);
        }
        while ((s = sr.ReadLine()) != null)
        {
            string[] eachrow = s.Split(',');
            Console.WriteLine("{0,-7}  {1,-15} {2,-19} {3,-18} {4}", eachrow[0], eachrow[1], eachrow[2], eachrow[3], eachrow[4]);
        }
    }
}
//Calling Methods
int option = DisplayMenu();
while (option != 0)
{
    if (option == 1)
    {
        Console.WriteLine();
        DisplayGuest(guest_list);
    }
    else if (option == 2)
    {
        Console.WriteLine();
        AvailRoom(rooms_list);
        DisplayRoom(rooms_list);
    }
    else if (option == 3)
    {
        Console.WriteLine();
        RegisterGuest(guest_list,cloned_guest_list);
    }
    else if (option == 4)
    {
        Console.WriteLine();
        CheckInGuest(rooms_list, guest_list,cloned_guest_list);
    }
    else if (option == 5)
    {
        Console.WriteLine();
        DisplayStayDetails(guest_list);
    }
    else if (option == 6)
    {
        Console.WriteLine();
        ExtendStay(guest_list);
    }
    else if (option == 7)
    {
        Console.WriteLine();
        MonthlyBreakdown(cloned_guest_list);
    }
    else if (option == 8)
    {
        Console.WriteLine();
        CheckOutGuest(guest_list);
    }
    else if(option == 9)
    {
        Console.WriteLine();
        DisplayReviews();
    }
    else
    {
        Console.WriteLine();
        Console.WriteLine("Invalid input! Please enter a valid number.");
    }
    Console.WriteLine();
    option = DisplayMenu();
}
Console.WriteLine("Bye!");

