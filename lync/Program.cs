using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Lync.Model;

namespace lync
{
    class Program
    {
        // COBOL HAS RUINED ME
        static string name;
        static int scode;
        static string line3;

        static void Main(string[] args)
        {
            name = "---";
            scode = 1;
            Console.Write("Enter search key : ");
            string mycontact = Console.ReadLine().ToString();
            Console.Write("Enter check frequency (secs): ");
            int freq;
            Int32.TryParse(Console.ReadLine(), out freq);
            freq = freq * 1000;
            if (freq == 0){ freq = 180000; }

            //Console.WriteLine(name);
            /*
             * 
             * 
             * * Invalid (-1)
             * None (0) – Do not use this enumerator. This flag indicates that the contact state is unspecified.
             * Free (3500) – A flag indicating that the contact is available
             * FreeIdle (5000) – Contact is free but inactive
             * Busy (6500) – A flag indicating that the contact is busy and inactive
             * BusyIdle (7500) – Contact is busy but inactive
             * DoNotDisturb (9500) – A flag indicating that the contact does not want to be disturbed
             * TemporarilyAway (12500) – A flag indicating that the contact is temporarily away
             * Away (15500) – A flag indicating that the contact is away
             * Offline (18500) – A flag indicating that the contact is signed out.
             * 
             * 
             */
            //Console.ReadLine();

            //Console.WriteLine("the searchforcontacts outputs");
            //Console.WriteLine(name);
            // Console.WriteLine(scode);
            //Console.WriteLine(line3);
            // Console.WriteLine("-------");

            while (true)
            {
                SearchForContacts(mycontact);
                //string output = Console.ReadLine().ToString();

                string message = string.Format(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                message += string.Format(",");
                message += string.Format(name);
                message += string.Format(",");
                String stringcode;
                stringcode = ConvertCode(scode);

                message += string.Format(stringcode);

                message += Environment.NewLine;

                //Console.WriteLine(output);

                string path = "..\\Log\\lynclog.txt";
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(message);
                    writer.Close();
                }
                 //Console.WriteLine(output)
                ;
                System.Threading.Thread.Sleep(freq);
            }

        }




        public static void SearchForContacts(string searchKey)
        {

            List < ContactInformationType > ContactInformationList = new List<ContactInformationType>();
            //ContactInformationList.Add(ContactInformationType.Activity);
            ContactInformationList.Add(ContactInformationType.Availability);
           // ContactInformationList.Add(ContactInformationType.CapabilityString);
           
            ContactSubscription contactSubscription = LyncClient.GetClient().ContactManager.CreateSubscription();
            
            
            Console.WriteLine(
                "Searching for contacts on " +
                searchKey);

            name = searchKey;

            LyncClient.GetClient().ContactManager.BeginSearch(
                searchKey,
                (ar) =>
                {
                    
                    SearchResults searchResults = LyncClient.GetClient().ContactManager.EndSearch(ar);
                    if (searchResults.Contacts.Count > 0)
                    {
                        Console.WriteLine(
                            searchResults.Contacts.Count.ToString() +
                            " found");

                        foreach (Contact contact in searchResults.Contacts)
                        {
                            contactSubscription.AddContact(contact);
                            contactSubscription.Subscribe(ContactSubscriptionRefreshRate.High, ContactInformationList);
                           Console.WriteLine(
                                contact.GetContactInformation(ContactInformationType.DisplayName).ToString()+"   "+ contact.GetContactInformation(ContactInformationType.Availability).ToString());
                            scode = (int)contact.GetContactInformation(ContactInformationType.Availability);
                            // next update, make this int and pass int code rather than string .ToString()
                        }
                    }
                },
                null);

        }

        public static String ConvertCode(int code)
        {
            String sgcode;
            Console.WriteLine(code.ToString());
            switch (code)
            {
                case 3500:
                    sgcode = "Free";
                    break;
                case 5000:
                    sgcode = "FreeIdle";
                    break;
                case 6500:
                    sgcode = "Busy";
                    break;
                case 7500:
                    sgcode = "BusyIdle";
                    break;
                case 9500:
                    sgcode = "DoNotDisturb";
                    break;
                case 12500:
                    sgcode = "TempAway";
                    break;
                case 15500:
                    sgcode = "Away";
                    break;
                case 18500:
                    sgcode = "Offline";
                    break;
                default:
                    sgcode = "---";
                    break;
            }

            return sgcode;
        }


    }
}
