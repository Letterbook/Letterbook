---
title: Integrations
order: 5
---

We recommend your website have pages for the following `info.txt` collection variables:

```txt
MailPreferences   {{WebsiteBaseUrl}}/mail-preferences
Unsubscribe       {{WebsiteBaseUrl}}/mail-preferences
Privacy           {{WebsiteBaseUrl}}/privacy
Contact           {{WebsiteBaseUrl}}/#contact
SignupConfirmed   {{WebsiteBaseUrl}}/signup-confirmed
```

You're also free to change the URLs in `info.txt` to reference existing pages on your website where they exist.

The `info.SignupConfirmed` URL is redirected to after a contact verifies their email address.

## Example

For reference here are example pages Razor SSG uses for this URLs:

| Page                                  | Source Code                                                                                                             |
|---------------------------------------|-------------------------------------------------------------------------------------------------------------------------|
| [/signup-confirmed](signup-confirmed) | [/_pages/signup-confirmed.md](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/_pages/signup-confirmed.md) |
| [/mail-preferences](mail-preferences) | [/_pages/mail-preferences.md](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/_pages/mail-preferences.md) |
| [/privacy](privacy)                   | [/_pages/privacy.md](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/_pages/privacy.md)                   |
| [/community-rules](community-rules)   | [/_pages/community-rules.md](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/_pages/community-rules.md)   |
