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
















        List<SearchProviders> myActiveSearchProviders = new List<SearchProviders>();
        /// <summary>
        /// Loads a list of search providers who are synched with Lync 2013 server address book  
        /// </summary>
        public void LoadSearchProviders(LyncClient lyncClient)
        {

            //Did the user sign in to Lync from inside of an organization firewall?
            if (lyncClient.SignInConfiguration.SignedInFromIntranet == true)
            {
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.ExchangeService) == SearchProviderStatusType.SyncSucceeded)
                {
                    myActiveSearchProviders.Add(SearchProviders.ExchangeService);
                }
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.Expert) == SearchProviderStatusType.SyncSucceeded)
                {
                    myActiveSearchProviders.Add(SearchProviders.Expert);
                }
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.GlobalAddressList) == SearchProviderStatusType.SyncSucceeded)
                {
                    myActiveSearchProviders.Add(SearchProviders.GlobalAddressList);
                }
            }
            else
            {
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.ExchangeService) == SearchProviderStatusType.SyncSucceededForExternalOnly)
                {
                    myActiveSearchProviders.Add(SearchProviders.ExchangeService);
                }
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.Expert) == SearchProviderStatusType.SyncSucceededForExternalOnly)
                {
                    myActiveSearchProviders.Add(SearchProviders.Expert);
                }
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.GlobalAddressList) == SearchProviderStatusType.SyncSucceededForExternalOnly)
                {
                    myActiveSearchProviders.Add(SearchProviders.GlobalAddressList);
                }
            }

            //The following search providers are always fully synched and their sync state does not change.
            myActiveSearchProviders.Add(SearchProviders.Default);
            myActiveSearchProviders.Add(SearchProviders.OtherContacts);
            myActiveSearchProviders.Add(SearchProviders.PersonalContacts);
            myActiveSearchProviders.Add(SearchProviders.WindowsAddressBook);
        }




        /// <summary>
        /// Search for a contact or group
        /// </summary>
        /// <param name="searchName">string. Name of contact or group to search for</param>
        /// <param name="numResults">uint. Number of results to return.</param>
        //public void SearchForGroupOrContact(LyncClient lyncClient, string searchName, uint numResults)
        //{
        //    try
        //    {
        //        // Initiate search for entity based on name.

        //        if (lyncClient.State == ClientState.SignedIn)
        //        {
        //            SearchFields searchFields = lyncClient.ContactManager.GetSearchFields();
        //            object[] asyncState = { lyncClient.ContactManager, searchName };
        //            foreach (var myActiveSearchProvider in myActiveSearchProviders)
        //            {
        //                lyncClient.ContactManager.BeginSearch(searchName
        //                    , myActiveSearchProvider
        //                    , searchFields
        //                    , SearchOptions.Default
        //                    , numResults
        //                    , (ar) =>
        //                    {
        //                        SearchResults searchResults = lyncClient.ContactManager.EndSearch(ar);
        //                        if (searchResults.Contacts.Count > 0)
        //                        {
        //                            Console.WriteLine(
        //                              searchResults.Contacts.Count.ToString() +
        //                              " found");

        //                            foreach (Contact contact in searchResults.Contacts)
        //                            {
        //                                Console.WriteLine(
        //                                contact.GetContactInformation(ContactInformationType.DisplayName).ToString());
        //                            }
        //                        }
        //                    }
        //                    , asyncState);
        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        //TODO handle exceptions
        //        //MessageBox.Show("Error:    " + ex.Message); 
        //    }
        //}
    }
}
