---
title: Posts
order: 9
group: Portal
---

The **Manage Posts** section provides editable [AutoQuery Grid components](https://docs.servicestack.net/vue/gallery/autoquerygrid) 
to manage all the RDBMS tables used to implement CreatorKit's comment system like **Threads** which manages the `Thread` 
table which supports adding Thread comments to every unique URL:

![](https://servicestack.net/img/pages/creatorkit/portal-threads.png)

## Moderation

Most of the time will be spent either reading through and deleting bad comments and responding to reported comments 
which includes special behavior to filter reports to only show reports that have yet to be moderated.

The **Update Comment Report** includes different moderation options you can choose to perform based on the severity of
a comment ranging from flagging a comment which marks the comment as flagged and hides the comment content, deleting
the comment which also deletes any replies, temporarily banning the user for a day, a week, a month to permanently
banning the user until they're explicitly unbanned:

![](https://servicestack.net/img/pages/creatorkit/portal-report.png)

Thread Users are managed with the [User Admin Feature](https://docs.servicestack.net/admin-ui-users) UI who can be
banned for any duration up to **Ban Until Date** or permanently banned by Locking the User Account:

![](https://servicestack.net/img/pages/creatorkit/admin-users.png)
