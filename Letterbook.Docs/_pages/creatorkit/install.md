---
title: Install
order: 2
---

CreatorKit is a customizable .NET companion App that you would run alongside your Website which provides the backend for
mailing list subscriptions, User repository and comment features which can be added to your website with CreatorKit's
tailwind components which are loaded from and communicate back directly to your CreatorKit .NET App instance:

<div class="my-12 flex justify-center w-full">
    <div class="flex flex-wrap">
        <svg class="w-40 h-40 text-indigo-600" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M13 23V13h10v10H13Zm1.5-1.5h7v-.8q-.625-.775-1.525-1.238T18 19q-1.075 0-1.975.463T14.5 20.7v.8ZM18 18q.625 0 1.063-.438T19.5 16.5q0-.625-.438-1.063T18 15q-.625 0-1.063.438T16.5 16.5q0 .625.438 1.063T18 18Zm-8.75 4l-.4-3.2q-.325-.125-.613-.3t-.562-.375L4.7 19.375l-2.75-4.75l2.575-1.95Q4.5 12.5 4.5 12.337v-.674q0-.163.025-.338L1.95 9.375l2.75-4.75l2.975 1.25q.275-.2.575-.375t.6-.3l.4-3.2h5.5l.4 3.2q.325.125.613.3t.562.375l2.975-1.25l2.75 4.75L19.925 11H15.4q-.35-1.075-1.25-1.788t-2.1-.712q-1.45 0-2.475 1.025T8.55 12q0 1.2.675 2.1T11 15.35V22H9.25Z"/></svg>
        <svg class="w-40 h-40 mx-8 mt-10 inline-block" style="rotate:200deg" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M16.01 11H4v2h12.01v3L20 12l-3.99-4v3z"/></svg>
        <div class="flex flex-col">
            <svg class="w-40 h-40 text-indigo-600" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><g fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"><path d="M3.338 17A9.996 9.996 0 0 0 12 22a9.996 9.996 0 0 0 8.662-5M3.338 7A9.996 9.996 0 0 1 12 2a9.996 9.996 0 0 1 8.662 5"/><path d="M13 21.95s1.408-1.853 2.295-4.95M13 2.05S14.408 3.902 15.295 7M11 21.95S9.592 20.098 8.705 17M11 2.05S9.592 3.902 8.705 7M9 10l1.5 5l1.5-5l1.5 5l1.5-5M1 10l1.5 5L4 10l1.5 5L7 10m10 0l1.5 5l1.5-5l1.5 5l1.5-5"/></g></svg>
            <svg class="w-32 h-32 text-sky-400" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M12 6c-2.67 0-4.33 1.33-5 4c1-1.33 2.17-1.83 3.5-1.5c.76.19 1.31.74 1.91 1.35c.98 1 2.09 2.15 4.59 2.15c2.67 0 4.33-1.33 5-4c-1 1.33-2.17 1.83-3.5 1.5c-.76-.19-1.3-.74-1.91-1.35C15.61 7.15 14.5 6 12 6m-5 6c-2.67 0-4.33 1.33-5 4c1-1.33 2.17-1.83 3.5-1.5c.76.19 1.3.74 1.91 1.35C8.39 16.85 9.5 18 12 18c2.67 0 4.33-1.33 5-4c-1 1.33-2.17 1.83-3.5 1.5c-.76-.19-1.3-.74-1.91-1.35C10.61 13.15 9.5 12 7 12Z"/></svg>
        </div>
    </div>
</div>

## Get CreatorKit

To better be able to keep up-to-date with future CreatorKit improvements we recommend
[forking CreatorKit](https://github.com/NetCoreApps/CreatorKit/fork) so you can easily apply future changes
to your customized forks:

<div class="my-12 text-center">
    <button type="button" title="Fork CreatorKit" class="rounded-md bg-white px-3.5 py-2.5 text-2xl font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50">
        <svg class="w-10 h-10 inline-block" xmlns="http://www.w3.org/2000/svg" width="256" height="256" viewBox="0 0 256 256"><path fill="currentColor" d="M224 64a32 32 0 1 0-40 31v9a16 16 0 0 1-16 16H88a16 16 0 0 1-16-16v-9a32 32 0 1 0-16 0v9a32 32 0 0 0 32 32h32v25a32 32 0 1 0 16 0v-25h32a32 32 0 0 0 32-32v-9a32.06 32.06 0 0 0 24-31ZM48 64a16 16 0 1 1 16 16a16 16 0 0 1-16-16Zm96 128a16 16 0 1 1-16-16a16 16 0 0 1 16 16Zm48-112a16 16 0 1 1 16-16a16 16 0 0 1-16 16Z"/></svg>
        <span class="pr-1">CreatorKit</span>
    </button>
</div>

Or if you're happy to take CreatorKit's current feature-set as it is, download the .zip to launch a local instance of
CreatorKit:

<div class="not-prose mt-12 flex justify-center">
<a class="hover:no-underline" href="https://github.com/NetCoreApps/CreatorKit/archive/refs/heads/main.zip">
    <div class="bg-white dark:bg-gray-800 px-4 py-4 mr-4 mb-4 rounded-lg shadow-lg text-center items-center justify-center hover:shadow-2xl dark:border-2 dark:border-pink-600 dark:hover:border-blue-600 dark:border-2 dark:border-pink-600 dark:hover:border-blue-600" style="min-width: 150px;">
        <div class="text-center font-extrabold flex items-center justify-center mb-2">
            <div class="text-4xl text-blue-400 my-3">
                <svg class="w-12 h-12 text-indigo-600" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M13 23V13h10v10H13Zm1.5-1.5h7v-.8q-.625-.775-1.525-1.238T18 19q-1.075 0-1.975.463T14.5 20.7v.8ZM18 18q.625 0 1.063-.438T19.5 16.5q0-.625-.438-1.063T18 15q-.625 0-1.063.438T16.5 16.5q0 .625.438 1.063T18 18Zm-8.75 4l-.4-3.2q-.325-.125-.613-.3t-.562-.375L4.7 19.375l-2.75-4.75l2.575-1.95Q4.5 12.5 4.5 12.337v-.674q0-.163.025-.338L1.95 9.375l2.75-4.75l2.975 1.25q.275-.2.575-.375t.6-.3l.4-3.2h5.5l.4 3.2q.325.125.613.3t.562.375l2.975-1.25l2.75 4.75L19.925 11H15.4q-.35-1.075-1.25-1.788t-2.1-.712q-1.45 0-2.475 1.025T8.55 12q0 1.2.675 2.1T11 15.35V22H9.25Z"/></svg>
            </div>
        </div>
        <div class="archive-name px-4 pb-2 text-blue-600 dark:text-indigo-400">CreatorKit.zip</div>
    </div>
</a>
</div>

## Extending CreatorKit

To minimize disruption when upgrading to future versions of CreatorKit we recommend adding any new Services to 
[CreatorKit.Extensions](https://github.com/NetCoreApps/CreatorKit/tree/main/CreatorKit.Extensions) and their DTOs
in [CreatorKit.Extensions.ServiceModel](https://github.com/NetCoreApps/CreatorKit/tree/main/CreatorKit.Extensions.ServiceModel):

<div data-component="FileLayout" data-props="{
    files: {
        CreatorKit: { 
            'CreatorKit.Extensions':  { _: ['CustomEmailRunServices.cs','CustomEmailServices.cs','CustomRendererServices.cs'] },
            'CreatorKit.Extensions.ServiceModel': { _: ['MarkdownEmail.cs','NewsletterMailRun.cs','RenderNewsletter.cs'] } 
        }
    }
}"></div>

These folders will be limited to optional extras which can added to or removed as needed where it will be isolated from 
the core set of functionality maintained in the other CreatorKit's folders. 

Any custom AppHost or IOC dependencies your Services require can be added to 
[Configure.Extensions.cs](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit/Configure.Extensions.cs).

### Before you Run

We need to initialize CreatorKit's database which we can populate with our preferred App Users, Mailing Lists and Subscribers
by modifying the CSV files in `/Migrations/seed`:

<div data-component="FileLayout" data-props="{
    files: {
        Migrations: { 
            seed:  { _: ['mailinglists.csv','subscribers.csv','users.csv'] },
            _:     ['Migration1000.cs','Migration1001.cs'] 
        }
    }
}"></div>


## Mailing Lists

You can define all Mailing Lists you wish to send and contacts can subscribe to in **mailinglists.csv**:

#### mailinglists.csv

```csv
Name,Description
None,None
TestGroup,Test Group
MonthlyNewsletter,Monthly Newsletter
BlogPostReleases,New Blog Posts
VideoReleases,New Videos
ProductReleases,New Product Releases
YearlyUpdates,Yearly Updates
```

When the database is first created this list will be used to generate the
[MailingList.cs](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit.ServiceModel/Types/MailingList.cs) Enum, e.g:

```csharp
[Flags]
public enum MailingList
{
    None = 0,
    [Description("Test Group")]
    TestGroup         = 1 << 0,     //1
    [Description("Monthly Newsletter")]
    MonthlyNewsletter = 1 << 1,     //2
    [Description("New Blog Posts")]
    BlogPostReleases  = 1 << 2,     //4
    [Description("New Videos")]
    VideoReleases     = 1 << 3,     //8
    [Description("New Product Releases")]
    ProductReleases   = 1 << 4,     //16
    [Description("Yearly Updates")]
    YearlyUpdates     = 1 << 5,     //32
}
```

This is a `[Flags]` enum with each value increasing by a power of 2 allowing a single integer value to capture
all the mailing lists contacts are subscribed to.

#### subscribers.csv

Add any mailing subscribers you wish to be included by default, it's a good idea to include all Website developer emails
here so they can test sending emails to themselves:

```csv
Email,FirstName,LastName,MailingLists
test@subscriber.com,Test,Subscriber,3
```

[Mailing Lists](creatorkit/customize#mailing-lists) is a flag enums where the integer values is a sub of all Mailing Lists
you want them subscribed to, e.g. use `3` to  subscribe to both the `TestGroup (1)` and `MonthlyNewsletter (2)` Mailing Lists.

#### users.csv

Add any App Users you want your CreatorKit App to include by default, at a minimum you'll need an `Admin` user which is
required to access the Portal to be able to use CreatorKit:

```csv
Id,Email,FirstName,LastName,Roles
1,admin@email.com,Admin,User,"[Admin]"
2,test@user.com,Test,User,
```

Once your happy with your seed data run the included [OrmLite DB Migrations](https://docs.servicestack.net/ormlite/db-migrations) with:

<div class="not-prose text-base"><div data-component="ShellCommand" data-props="{ text:'npm run migrate' }"></div></div>

Which will create the CreatorKit SQLite databases with your seed Users and Mailing List subscribers included.

Should you need to recreate the database, you can delete the `App_Data/*.sqlite` databases then rerun
`npm run migrate` to recreate the databases with your updated `*.csv` seed data.

### What's included

The full .NET Source code is included with CreatorKit enabling unlimited customizations. It's a stand-alone download
which doesn't require any external dependencies to run initially, although some features require configuration:

#### SMTP Server

You'll need to configure an SMTP Server to enable sending Emails by adding it to your **appsettings.json**, e.g:

```json
{
  "smtp": {
    "UserName" : "SmtpUsername",
    "Password" : "SmtpPassword",
    "Host" : "smtp.example.org",
    "Port" : 587,
    "From" : "noreply@example.org",
    "FromName" : "My Organization",
    "Bcc": "optional.backup@example.org"
  }
}
```

If you don't have an existing SMTP Server we recommend using [Amazon SES](https://aws.amazon.com/ses/) as a cost effective
way to avoid managing your own SMTP Servers.

#### OAuth Providers

By default CreatorKit is configured to allow Sign In's for authenticated post comments from Facebook, Google, Microsoft
OAuth Providers during development on its `https://localhost:5002`.

You'll need to configure OAuth Apps for your production host in order to support OAuth Sign Ins at deployment:

- Create App for Facebook at https://developers.facebook.com/apps
- Create App for Google at https://console.developers.google.com/apis/credentials
- Create App for Microsoft at https://apps.dev.microsoft.com

You can Add/Remove to this from the list of [supported OAuth Providers](https://docs.servicestack.net/auth#oauth-providers).

### RDBMS

CreatorKit by default is configured to use an embedded SQLite database which can be optionally configured to replicate
backups to AWS S3 or Cloudflare R2 using [Litestream](https://docs.servicestack.net/ormlite/litestream).

This is setup to be used with Cloudflare R2 by default which can be configured from the [.deploy/litestream-template.yml](https://github.com/NetCoreApps/CreatorKit/blob/main/.deploy/litestream-template.yml) file:

```yml
access-key-id: ${R2_ACCESS_KEY_ID}
secret-access-key: ${R2_SECRET_ACCESS_KEY}

dbs:
  - path: /data/db.sqlite
    replicas:
      - type: s3
        bucket: ${R2_BUCKET}
        path: db.sqlite
        region: auto
        endpoint: ${R2_ENDPOINT}
```

By adding the matching GitHub Action Secrets to your repository, this file will be populated and deployed to your own Linux server via SSH.
This provides a realtime backup to your R2 bucket for minimal cost, enabling point in time recovery of data if you run into issues.

Alternatively [Configure.Db.cs](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit/Configure.Db.cs) can
be changed to use preferred [RDBMS supported by OrmLite](https://docs.servicestack.net/ormlite/installation).

### App Settings

The **PublicBaseUrl** and **BaseUrl** properties in `appsettings.json` should be updated with the URL where your
CreatorKit instance is deployed to and replace **WebsiteBaseUrl** with the website you want to use CreatorKit emails
to be addressed from:

```json
{
  "AppData": {
    "PublicBaseUrl":  "https://creatorkit.netcore.io",
    "BaseUrl":        "https://creatorkit.netcore.io",
    "WebsiteBaseUrl": "https://razor-ssg.web-templates.io"
  }
}
```

### CORS

Any additional Website URLs that utilize CreatorKit's components should be included in the CORS **allowOriginWhitelist**
to allow CORS requests from that website:

```json
{
  "CorsFeature": {
    "allowOriginWhitelist": [
      "http://localhost:5000",
      "http://localhost:8080"
    ]
  }
}
```

### Customize

After configuring CreatorKit to run with your preferred Environment, you'll want to customize it to your Organization
or Personal Brand:
