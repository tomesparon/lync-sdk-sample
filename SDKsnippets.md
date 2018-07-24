```csharp
    /// <summary>
    /// Subscribes to contacts.
    /// </summary>
    /// <param name="group">List of Lync contacts.</param>
    void SubscribeToContacts(List<Contact> contacts)
    {
        Console.WriteLine("Total Contacts: " + contacts.Count.ToString());

        foreach (var contact in contacts)
            contact.ContactInformationChanged += new EventHandler<ContactInformationChangedEventArgs>(Contact_ContactInformationChanged);

        contactSubscription = contactManager.CreateSubscription();

        //Choose the types of presence changes to listen for
        var contactInformationTypes = new List<ContactInformationType>() { ContactInformationType.Availability };

        contactSubscription.Subscribe(ContactSubscriptionRefreshRate.High, contactInformationTypes);

        //contactSubscription.AddContacts(contacts);
        foreach (var item in contacts)
        {
            Console.WriteLine(item.Uri);
            contactSubscription.AddContact(item);

            Console.WriteLine(item.GetContactInformation(ContactInformationType.Availability).ToString());
        }
    }
```

Powershell example:
David W Knight • 6 years ago   http://www.ravichaganti.com/blog/finding-lync-contact-availability-using-powershell/

This made me wonder about the potential for logging lync status changes on a list of particular contacts. I put together this function that creates event subscriptions for adding status changes to a log file. You run the function, and the logging will continue to occur in the background as long as your powershell session is open. To have it run in the background, I created a task to start at logon that runs powershell.exe with -windowstyle hidden -noexit and runs this function to continually capture a log in the background. It's missing the embedded help comments, which I haven't gotten around to adding yet. I used an advanced function template I put together - there are BEGIN and END blocks which aren't used here yet, but I like to make the advanced functions as fully featured as possible, so I'll probably go back and tweak it to accept input from the pipeline, etc., and the BEGIN and END blocks come in handy sometimes, so I just include them by default for potential future use.
Sample usage:
Start-LogLyncAvailability -contacts 'joe@mycompany.com','fred@mycompany.com' -logfile c:\temp\LyncAvailability.log
Enjoy...
```powershell
function Start-LogLyncAvailability
{
[CmdletBinding(
SupportsShouldProcess=$false,
ConfirmImpact="None",
DefaultParameterSetName="")]
param(
[Parameter(
Position=0,
ValueFromPipeLine=$false,
Mandatory=$true,
ValueFromPipelineByPropertyName=$false,
ValueFromRemainingArguments=$false,
HelpMessage="The email addresses of the contacts to monitor",
ParameterSetName=""
)]
[string[]]$contacts,

[Parameter(
Position=1,
ValueFromPipeLine=$false,
Mandatory=$true,
ValueFromPipelineByPropertyName=$false,
ValueFromRemainingArguments=$false,
HelpMessage="The file to write the log to",
ParameterSetName=""
)]
[string]$LogFile,

[Parameter(
Position=2,
ValueFromPipeLine=$false,
Mandatory=$false,
ValueFromPipelineByPropertyName=$false,
ValueFromRemainingArguments=$false,
#HelpMessage="The path to the Lync 2010 SDK dll files",
ParameterSetName=""
)]
[string]$lyncSDKPath="C:\Program Files (x86)\Microscoft Lync\SDK\Assemblies\Desktop"
)

# Use Write-Debug, Write-Verbose, Write-Error, and Write-Warning
# Use Write-Output to write objects to the pipeline
BEGIN
{
write-debug "In the BEGIN script block."
}

PROCESS
{
if ((Get-EventSubscriber | where {$_.sourceidentifier -like "LogLyncAvailability-*"}).count -gt 0)
{ write-host "Currently logging Lync availability. Please run Stop-LogLyncAvailability and then re-run this command." ; return }

$lyncSDKPath = $(dir $lyncSDKPath | select -first 1).directory.FullName
try {Import-Module "$lyncSDKPath\Microsoft.Lync.Model.dll"}
catch { "Failed to import the Microsoft.Lync.Model.dll" ; return }

try { $client = [Microsoft.Lync.Model.LyncClient]::GetClient() }
catch { "Error attaching to the Lync client. Probably it is not running and/or not installed." ; return }

$self = $client.Self

Add-Content $Logfile "$((get-date).DateTime) - beginning log of lync availability changes for $contacts."
write-host "$((get-date).DateTime) - beginning log of lync availability changes for $contacts."
Write-Host "Logging to $((dir $Logfile).fullname)"
Write-Host "Note that the logging will continue only as long as this powershell session exists."
Write-Host "The 'Stop-LogLyncAvailability' command can be used to stop the lync status logging."

# creating the $LyncAvailabilityLogFile variable as a read-only variable in the global scope, as it needs to be available to the event action scriptblock
set-Variable -name LyncAvailabilityLogfile -Option readonly -Value "$((dir $logfile).fullname)" -force -scope global

foreach ($email in $contacts)
{
$contact = $client.ContactManager.GetContactByUri($email)

$out = Register-ObjectEvent -InputObject $contact `
-EventName "ContactInformationChanged" `
-SourceIdentifier "LogLyncAvailability-$email" `
-action {
$changedinfo = $event.sourceeventargs.changedcontactinformation
if ($changedinfo -contains "Availability")
{
Add-Content $LyncAvailabilityLogfile "$((get-date).DateTime) - $(($sender.uri -split ':')[1]) is now $([Microsoft.Lync.Model.ContactAvailability]$sender.getcontactinformation("Availability"))"
}
}
} # END foreach

} # END process block

END
{
write-debug "In the END script block."
}
}
function Stop-LogLyncAvailability
{
Get-EventSubscriber | where {$_.sourceidentifier -like "LogLyncAvailability-*"} | Unregister-Event
}
```


Other interesting https://github.com/zloeber/Powershell/tree/master/Lync

other sdk sample:
PresenceIndicator

Populates a list on a page with three PresenceIndicator instances. The user can hover a mouse pointer over any of the presence indicators to display a contact card for a user. The Source property for each control instance is set by the sample application.

WPF sample location: %PROGRAMFILES(X86)%\Microsoft Office 2013\LyncSDK\Samples
\microsamples.zip\PresenceIndicatorDesktop\PresenceIndicatorDesktop.sln

Silverlight sample location: %PROGRAMFILES(X86)%\Microsoft Office 2013\LyncSDK\Samples
\microsamples.zip\PresenceIndicatorDesktop\PresenceIndicatorSilverlight.sln
