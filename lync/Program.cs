using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Lync.Model;

namespace lync
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Write("Enter search key : ");
            SearchForContacts(Console.ReadLine().ToString());
            //Console.WriteLine(name);
            Console.ReadLine();
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
                        } 
                    }
                },
                null);
        }




    }
}
