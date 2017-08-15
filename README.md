# Easy AAD User and Groups Manager

An ASP.NET Core 1.0 website with Google Material Design, Azure Active Directory Graph API and Azure Autoamtion Runbooks for managing Azure Active Directory in an easy controlled way.

## Updates

* 01-8-2017 Initial release to GitHub
* 15-8-2017 Update ReadMe

## The idea

The front office or HR controls the onboarding of new employees. This easy AAD User and groups manager app supports the onboarding and changing roles of employees. Without the need of getting in to the technical, and complex, Azure portal by HR people.

The App seperates responsibilities during the creation of AAD users and Groups in two areas. The front office triggers the creation or editing of a user and assinges groups via the App. the backoffice, AAD administrators, controls the steps and details needed for the creation of the user or group.

Solution roles:
![alt text](/Docs/Images/Resps.jpg "Solution")

## Cloud Resources

The Web App uses several Cloud resources.

* Azure Web App, for the end user.
  * Create, edit, delete users and groups.
  * Triggers Azure Automation runbooks.
  * Collect user and group state via AAD Graph API.

* Azure Automation Runbook, maintained by administrators.
  * Edit, Create, Delete users or groups.
  * Send notification mail with temproraly password.
  * Logs activtiy to Azure Blob Storage.

* Azure Blob Storage.
  * Stores application and runbook logdata.
  * Stores temproraly user and group data.

* Azure Active Directory
  * Holds users and groups
  * Secures Web App and Azure Automation Runbook.

![Image Cloud resources](/Docs/Images/Solution.png "Cloud resources")

All Azure Resources are created via ARM templates except AAD and its settings. These settings are the regitration of the WebApp and the inital group configuration. See ![blog post jurgen, hard to setup aad].

The ARM templates follow the organization of Core and Application resources. See ![blog post Clemens]. The ARM Tempaltes can be found in the ARM folder.

## UI

[Google Material Design Lite](https://getmdl.io/) is used for the UI.

### Menu

![alt text](/Docs/Images/gmd-menu.png "Solution")

### Create user

![alt text](/Docs/Images/gmd-newuser.png "Solution")
The create user screen constructs the mail adres from firstname, suffix, lastname via a simple Javascript function, change this if you want something else. There should be at least one department group in AAD for the creation of a user.

### Edit groupmembership

![alt text](/Docs/Images/gmd-editgroups.png "Solution")
The groupmembership editing screen collects all AAD groups with prefix 'Dep', 'Az' or 'App'. See file [AadCollectieFromAad.cs](/AadUserCreation/Data/AadCollectieFromAad.cs)

## Implementation

The Web Application is implemented with ASP Net Core in Visual Studio 2017 Preview.
The solution is organized in Controllers, Business Services and Data Services with the default Dependency Injectinion of ASP Net Core 1.0.

### Authentication

Authentication of users is done with Azure Active Directory.

```cs
app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                ClientId = Configuration["Authentication:AzureAd:ClientId"],
                Authority = Configuration["Authentication:AzureAd:AADInstance"] + Configuration["Authentication:AzureAd:TenantId"],
                CallbackPath = Configuration["Authentication:AzureAd:CallbackPath"]
            });
```

Within Azure Active Directory users are restricted by application group who can use the App.

### App Settings

The Application Settings are holding the authentication settings, webhooks urls and storage account connection string. Default ASP Net Core appsetings.{environment}.json files are used for the different stages. An entry in the gitignore takes care no configuration settings are stored in Githuib.

### ARM Templates

The provisioning of the Cloud Resources is seperated in Core Cloud resources and Application Cloud resousorces. Core are the App Service Plan, Storage Account, Azure Automation Runbooks. The application resource is the WebApp. Core resources need to be deployed first.

## Getting Started

Three steps needs to be done before starting the WebApp.

* Deploy Core Azure Resource ARM Templates, and capture the configuration settings.
* Create or select Azure Active Directory.
* Add Application to Azure Active Directory, one entry for localhost on for deployed WebApp.

Run Application.

## Contribute

Pull requests serve as the primary mechanism by which contributions are proposed and accepted. We recommend creating a [topic branch](https://www.git-scm.com/book/en/v2/Git-Branching-Branching-Workflows#Topic-Branches) and sending a pull request to the master branch from the topic branch. For additional guidance, read through the [GitHub Flow Guide](https://guides.github.com/introduction/flow/).

Be prepared to address feedback on your pull request and iterate if necessary.