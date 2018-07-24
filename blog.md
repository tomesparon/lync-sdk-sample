AUGUST 3, 2016 BY KASUN BALASOORIYA
Using lync sdk 2013 to get contact attributes.
Lync Banner

Hi!

In this post i’m going to explain how to use the lync sdk 2013 to query your contacts in lyncs and get the attribute of a contact. I have the whole project in this git repo.

I will try to explain some of the code behind the application. First about the pre-requisites.

What you need :

Lync 2013 ||  skype for business 2015/16
lync sdk 2013 (can be downloaded from : https://www.microsoft.com/en-us/download/details.aspx?id=36824)
Then let’s go through the inner workings of the project.

I used the above sdk to query my contact list with a given email address and return the current availability of that person. I hope this blog-post will save the time someone have to spend on reading a whole bunch of documentation and meddling with the methods provided by the sdk.

I have written the method SearchForContacts to search for the contacts  of the currently signed in lync user.  The list ContactInformationList contains the attributes of we are going to check. Later in the code we will be subscribing to the list of contacts returned by our query.

When we do, we can prioritize the attributes of the contact we are most interested in since if you go through the attributes you will notice that there are quite a number of attributes available. (For my app I’m only interested in the contact’s Availability property )

For us to use the methods from the lync sdk for this module the Microsoft.Lync.Model.dll reference should be added. The .dlls can be found at C:\Program Files (x86)\Microsoft Office\Office15\LyncSDK\Assemblies\Desktop folder.

 
```csharp
List < ContactInformationType > ContactInformationList = new List();
//ContactInformationList.Add(ContactInformationType.Activity);
//ContactInformationList.Add(ContactInformationType.EmailAddresses);
ContactInformationList.Add(ContactInformationType.Availability);
// ContactInformationList.Add(ContactInformationType.CapabilityString);
```
Then we can create a subscription using the contact manager.
```csharp
ContactSubscription contactSubscription = 
                    LyncClient.GetClient().ContactManager.CreateSubscription();
```
Once that is done we can use the following lambda expression to search for contacts for a search string we enter in the console.

 
```csharp
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
Console.WriteLine(searchResults.Contacts.Count.ToString() +" found");
```
Next we can loop through the contacts returned and we can use the  ContactSubscription  object created earlier to subscribe to each contacts’ attributes we are planning to use. Inside the contactSubscription.Subscribe method we are setting the refresh rate to high and we are passing the contact attribute list we need.

 
```csharp
foreach (Contact contact in searchResults.Contacts)
{
contactSubscription.AddContact(contact);
contactSubscription.Subscribe(ContactSubscriptionRefreshRate.High, ContactInformationList);
Console.WriteLine(
contact.GetContactInformation(ContactInformationType.DisplayName).ToString()+" "+ 
                          contact.GetContactInformation(ContactInformationType.Availability).ToString()); }
                          }
                   },
        null);

  }
```

A side not on the Availability codes that are returned from the above method call.

* Invalid (-1)
* None (0) – Do not use this enumerator. This flag indicates that the contact state is unspecified.
* Free (3500) – A flag indicating that the contact is available
* FreeIdle (5000) – Contact is free but inactive
* Busy (6500) – A flag indicating that the contact is busy and inactive
* BusyIdle (7500) – Contact is busy but inactive
* DoNotDisturb (9500) – A flag indicating that the contact does not want to be disturbed
* TemporarilyAway (12500) – A flag indicating that the contact is temporarily away
* Away (15500) – A flag indicating that the contact is away
* Offline (18500) – A flag indicating that the contact is signed out.


Apart from the above methods used if you want to uniquely identify a certain user,  the  GetContactByUri under ContactManager can be used.
```csharp
Contact foundContact= LyncClient.GetClient().ContactManager.GetContactByUri(emailAddress);
```

This will return a contact object and as usual the contact attributes can be accessed by a code like
```csharp
string resultString  = foundContact.GetContactInformation(ContactInformationType.Availability).ToString();
```
Hope this helps!

