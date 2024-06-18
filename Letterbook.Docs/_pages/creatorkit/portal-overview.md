---
title: Overview
order: 6
group: Portal
---

All information captured by CreatorKit's components can be managed from your CreatorKit's instance portal at:

<div class="not-prose">
    <h3 class="text-4xl text-center text-indigo-800 font-semibold pb-3"><span class="text-gray-300">https://localhost:5003</span>/portal/</h3>
</div>

Signing in with an Admin User will take you to the dashboard showing your Website activity: 

![](https://servicestack.net/img/pages/creatorkit/portal.png)

## Mailing List Admin

The first menu section is for managing your contact mailing lists including creating and sending emails and email campaigns
to mailing list contacts,

### Contacts

Mailing List Contacts can either be added via the [JoinMailingList](creatorkit/components#joinmailinglist) component
on your website or using the Contacts Admin UI:

![](https://servicestack.net/img/pages/creatorkit/portal-contacts.png)

### Archive

When you want to clear your workspace of sent emails you can archive them which moves them to a separate Database ensuring
the current working database is always snappy and clear of clutter. 

![](https://servicestack.net/img/pages/creatorkit/portal-archive.png)

## Posts Admin

The **Manage Posts** section is for managing and moderating your website's post comments with
most menu items manages data in different Tables using [AutoQueryGrid](https://docs.servicestack.net/vue/gallery/autoquerygrid)
and custom [AutoForm](https://docs.servicestack.net/vue/gallery/autoform) components.
