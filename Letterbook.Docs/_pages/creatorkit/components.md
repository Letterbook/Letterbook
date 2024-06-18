---
title: Components
order: 4
---

After launching your customized CreatorKit instance, you can start integrating its features into your existing websites, 
or if you're also in need of a fast, beautiful website we highly recommend the [Razor SSG](https://razor-ssg.web-templates.io/posts/razor-ssg)
template which is already configured to include CreatorKit's components.

The components are included using a declarative progressive markup so that it doesn't affect the behavior of the website
if the CreatorKit is down or unresponsive.

## Enabling CreatorKit Components

To utilize CreatorKit's Components in your website you'll need to initialize the components you want to use by embedding
this script at the bottom of your page, e.g. in [Footer.cshtml](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/Pages/Shared/Footer.cshtml):

```html
<script type="module">
@{ var components = HostContext.DebugMode 
     ? "https://localhost:5003/mjs/components" 
     : "https://creatorkit.netcore.io/mjs/components"; }

import { mail } from '@components/mail.mjs'
import { post } from '@components/post.mjs'

mail('[data-mail]')
post('[data-post]')
</script>
```

Where `mail()` will scan the document for declarative `data-mail` for any Mailing List components to create, likewise `post()`
does the same for any Thread/Post components.

The `@components` URL lets you load Components from your `localhost:5001` instance during development and your public CreatorKit
instance in production which you'll need to replace `creatorkit.netcore.io` to use. 

## Post Voting and Comments

You can enable voting for individual posts or pages with and Thread comments by including the `PostComments` component with:

```html
<div data-post="PostComments""></div>
```

Which when loaded will render a thread like icon where users can up vote posts or pages and either Sign In/Sign Up
buttons for unauthenticated users or a comment box for Signed in Users:

<div data-post="PostComments" data-props="{ commentLink: null }" class="not-prose text-base mb-12"></div>

#### PostComments Properties

The available PostComments properties for customizing its behavior include:

```ts
defineProps<{
    hide?: "threadLikes"|"threadLikes"[]
    commentLink?: { href: string, label: string }
}>()
```

### Component Properties

Any component properties can be either declared inline using `data-props`, e.g:

```html
<div data-post="PostComments" data-props="{
    hide: 'threadLikes', 
    commentLink: { 
        href: '/community-rules',
        label: 'read the community rules'
    } 
}"></div>
```

<div data-post="PostComments" data-props="{
    hide: 'threadLikes', 
    commentLink: { 
        href: '/community-rules',
        label: 'read the community rules'
    } 
}" class="not-prose text-base mb-20"></div>

Where it will hide the Thread Like icon and include a link to your `/community-rules` page inside each comment box.

Alternatively properties can instead be populated in the `mail()` and `post()` initialize functions: 

```html
<script type="module">
@{ var components = HostContext.DebugMode
     ? "https://localhost:5001/mjs/components"
     : "https://creatorkit.netcore.io/mjs/components"; }

mail('[data-mail]', { 
    mailingLists:['MonthlyNewsletter'] 
})

post('[data-post]', {
    commentLink: { 
        href: '/community-rules',
        label: 'read the community rules'
    } 
})
</script>
```

## Mailing List Components

### JoinMailingList

The `JoinMailingList` component can be added anywhere you want to accept Mailing List subscriptions on your website, e.g:

```html
<div data-mail="JoinMailingList" data-props="{ submitLabel:'Join our newsletter' }"></div>
```

<div class="my-20 flex justify-center">
    <div data-mail="JoinMailingList" data-props="{ submitLabel:'Join our newsletter' }"></div>
</div>

Which you can style as needed as this template wraps in a 
[Newsletter.cshtml](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/Pages/Shared/Newsletter.cshtml)
Tailwind component that's displayed on the [Home Page](/).

#### JoinMailingList Properties

Which allows for the following customizations:

```ts
defineProps<{
    //= MonthlyNewsletter
    mailingLists?: "TestGroup"     | "MonthlyNewsletter" | "BlogPostReleases" |
                   "VideoReleases" | "ProductReleases"   | "YearlyUpdates" 
    placeholder?: string    //= Enter your email
    submitLabel?: string    //= Subscribe
    thanksHeading?: string  //= Thanks for signing up!
    thanksMessage?: string  //= To complete sign up, look for the verification...
    thanksIcon?: { svg?:string, uri?:string, alt?:string, cls?:string }
}>
```

### MailPreferences

The `MailPreferences` component manages a users Mailing List subscriptions which you can be linked in your Email footers
for users wishing to manage or unsubscribe from mailing list emails. 

It can be include in any HTML or Markdown page as Razor SSG does in its 
[mail-preferences.md](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/_pages/mail-preferences.md):

```html
<div data-mail="MailPreferences"></div>
```

Where if it's unable to locate the user will ask the user for their email:

<div class="my-20" data-mail="MailPreferences"></div>

Alternatively the page can jump directly to a contacts Mailing Lists by including a `?ref` query string parameter
of the Contact's External Ref, e.g: `/mail-preferences?ref={{ExternalRef}}`

You can also add `&unsubscribe=1` to optimize the page for users wishing to Unsubscribe where it will also display
an **Unsubscribe** button to subscribe to all mailing lists.

#### MailPreferences Properties

Most of the copy used in the `MailPreferences` component can be overridden with:

```ts
defineProps<{
    emailPrompt?: string            //= Enter your email to manage your email...
    submitEmailLabel?: string       //= Submit
    updatedHeading?: string         //= Updated!
    updatedMessage?: string         //= Your email preferences have been saved.
    unsubscribePrompt?: string      //= Unsubscribe from all future email...
    unsubscribeHeading?: string     //= Updated!
    unsubscribeMessage?: string     //= You've been unsubscribed from all email...
    submitLabel?: string            //= Save Changes
    submitUnsubscribeLabel?: string //= Unsubscribe
}>()
```

## Tailwind Styles

CreatorKit's components are styled with tailwind classes which will also need to be included in your website. 
For Tailwind projects we recommend copying a concatenation of all Components from 
[/CreatorKit/wwwroot/tailwind/all.components.txt](https://raw.githubusercontent.com/NetCoreApps/CreatorKit/main/CreatorKit/wwwroot/tailwind/all.components.txt) 
and include it in your project where the tailwind CLI can find it so any classes used are included in your 
App's Tailwind **.css** bundle.

In Razor SSG projects this is already being copied in its [postinstall.js](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/postinstall.js) 

If you're not using Tailwind, websites will need to reference your CreatorKit's instance Tailwind .css bundle instead, e.g:

```html
<link rel="stylesheet" href="https://creatorkit.netcore.io/css/app.css">
```