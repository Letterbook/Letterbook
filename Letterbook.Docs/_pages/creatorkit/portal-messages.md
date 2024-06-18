---
title: Messages
order: 7
group: Portal
---

### Sending Single plain-text Emails

**Messages** lets you craft and send emails to a single contact which can be sent immediately or saved as a draft so
you can review the HTML rendered email and send later.

![](https://servicestack.net/img/pages/creatorkit/portal-messages.png)

It also lists all available emails that can be sent which are any APIs that inherit the `CreateEmailBase` base class
which contains the minimum contact fields required in each email:

```csharp
public abstract class CreateEmailBase
{
    [ValidateNotEmpty]
    [Input(Type="EmailInput")]
    public string Email { get; set; }
    [ValidateNotEmpty]
    [FieldCss(Field = "col-span-6 lg:col-span-3")]
    public string FirstName { get; set; }
    [ValidateNotEmpty]
    [FieldCss(Field = "col-span-6 lg:col-span-3")]
    public string LastName { get; set; }
}
```

Plain text emails can be sent with the `SimpleTextEmail` API:

```csharp
[Renderer(typeof(RenderSimpleText))]
[Tag(Tag.Mail), ValidateIsAdmin]
[Description("Simple Text Email")]
public class SimpleTextEmail : CreateEmailBase, IPost, IReturn<MailMessage>
{
    [ValidateNotEmpty]
    [FieldCss(Field = "col-span-12")]
    public string Subject { get; set; }

    [ValidateNotEmpty]
    [Input(Type = "textarea"), FieldCss(Field = "col-span-12", Input = "h-36")]
    public string Body { get; set; }
    public bool? Draft { get; set; }
}
```

### Email UI

Which are rendered using the [Vue AutoForm component](https://docs.servicestack.net/vue/gallery/autoform) from the API
definition where the `SimpleTextEmail` Request DTO renders the new Email UI:

![](https://servicestack.net/img/pages/creatorkit/portal-messages-simple.png)

Which uses the custom `EmailInput` component to search for contacts and populates their Email, First and Last name fields.

The implementation for sending single emails are defined in
[EmailServices.cs](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit.ServiceInterface/EmailServices.cs)
which uses [EmailRenderer.cs](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit.ServiceInterface/EmailRenderer.cs)
to save and send non draft emails which follow the pattern below:

```csharp
public EmailRenderer Renderer { get; set; }

public async Task<object> Any(SimpleTextEmail request)
{
    var contact = await Db.GetOrCreateContact(request);
    var viewRequest = request.ConvertTo<RenderSimpleText>().FromContact(contact);
    var bodyText = (string) await Gateway.SendAsync(typeof(string), viewRequest);
    
    var email = await Renderer.CreateMessageAsync(Db, new MailMessage
    {
        Draft = request.Draft ?? false,
        Message = new EmailMessage
        {
            To = contact.ToMailTos(),
            Subject = request.Subject,
            Body = request.Body,
            BodyText = bodyText,
        },
    }.FromRequest(request));
    return email;
}
```

Live previews are generated and Emails rendered with renderer APIs that inherit `RenderEmailBase` e.g:

```csharp
[Tag(Tag.Mail), ValidateIsAdmin, ExcludeMetadata]
public class RenderSimpleText : RenderEmailBase, IGet, IReturn<string>
{
    public string Body { get; set; }
}
```

Which renders the Request DTO inside a [#Script](https://sharpscript.net) email context:

```csharp
public async Task<object> Any(RenderSimpleText request)
{
    var ctx = Renderer.CreateScriptContext();
    return await ctx.RenderScriptAsync(request.Body,request.ToObjectDictionary());
}
```

### Sending Custom HTML Emails

`CustomHtmlEmail` is a configurable API for sending HTML emails utilizing custom Email Layout and Templates
from populated dropdowns configured with available Templates in `/emails`:

```csharp
[Renderer(typeof(RenderCustomHtml))]
[Tag(Tag.Mail), ValidateIsAdmin]
[Icon(Svg = Icons.RichHtml)]
[Description("Custom HTML Email")]
public class CustomHtmlEmail : CreateEmailBase, IPost, IReturn<MailMessage>
{
    [ValidateNotEmpty]
    [Input(Type = "combobox", EvalAllowableValues = "AppData.EmailLayoutOptions")]
    public string Layout { get; set; }
    
    [ValidateNotEmpty]
    [Input(Type = "combobox", EvalAllowableValues = "AppData.EmailTemplateOptions")]
    public string Template { get; set; }
    
    [ValidateNotEmpty]
    [FieldCss(Field = "col-span-12")]
    public string Subject { get; set; }

    [Input(Type = "MarkdownEmailInput", Label = ""), FieldCss(Field = "col-span-12", Input = "h-56")]
    public string? Body { get; set; }
    public bool? Draft { get; set; }
}
```

![](https://servicestack.net/img/pages/creatorkit/portal-messages-custom.png)

#### Custom HTML Implementation

It follows the same pattern as other email implementations where it uses the `EmailRenderer` to create and send emails:

```csharp
public async Task<object> Any(CustomHtmlEmail request)
{
    var contact = await Db.GetOrCreateContact(request);
    var viewRequest = request.ConvertTo<RenderCustomHtml>().FromContact(contact);
    var bodyHtml = (string) await Gateway.SendAsync(typeof(string), viewRequest);

    var email = await Renderer.CreateMessageAsync(Db, new MailMessage
    {
        Draft = request.Draft ?? false,
        Message = new EmailMessage
        {
            To = contact.ToMailTos(),
            Subject = request.Subject,
            Body = request.Body,
            BodyHtml = bodyHtml,
        },
    }.FromRequest(viewRequest));
    return email;
}
```

Which uses the `RenderCustomHtml` to render the HTML and Live Previews which executes the populated Request DTO with
the Email **#Script** context configured to use the selected Email Layout and Template:

```csharp
public async Task<object> Any(RenderCustomHtml request)
{
    var context = Renderer.CreateMailContext(layout:request.Layout, page:request.Template);
    var evalBody = !string.IsNullOrEmpty(request.Body) 
        ? await context.RenderScriptAsync(request.Body, request.ToObjectDictionary())
        : string.Empty;

    return await Renderer.RenderToHtmlResultAsync(Db, context, request, 
        args:new() {
            ["body"] = evalBody,
        });
}
```

## CreatorKit.Extensions

Any additional services should be maintained in [CreatorKit.Extensions](https://github.com/NetCoreApps/CreatorKit/tree/main/CreatorKit.Extensions) 
project with any custom email implementations added to 
[CustomEmailServices.cs](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit.Extensions/CustomEmailServices.cs).

### Sending HTML Markdown Emails

[MarkdownEmail.cs](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit.Extensions.ServiceModel/MarkdownEmail.cs)
is an example of a more user-friendly custom HTML Email you may want to send, which is pre-configured to use the
[basic.html](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit/emails/layouts/basic.html)
Layout and the
[empty.html](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit/emails/empty.html)
Email Template to allow sending plain HTML Emails with a custom Markdown Email body:

```csharp
[Renderer(typeof(RenderCustomHtml), Layout = "basic", Template="empty")]
[Tag(Tag.Mail), ValidateIsAdmin]
[Icon(Svg = Icons.TextMarkup)]
[Description("Markdown Email")]
public class MarkdownEmail : CreateEmailBase, IPost, IReturn<MailMessage>
{
    [ValidateNotEmpty]
    [FieldCss(Field = "col-span-12")]
    public string Subject { get; set; }

    [ValidateNotEmpty]
    [Input(Type="MarkdownEmailInput",Label=""), FieldCss(Field="col-span-12",Input="h-56")]
    public string? Body { get; set; }
    public bool? Draft { get; set; }
}
```

As defined, this DTO renders the form utilizing a custom `MarkdownEmailInput` rich text editor which provides an optimal UX 
for authoring Markdown content with icons to assist with discovery of Markdown's different formatting syntax.

#### Template Variables

The editor also includes a dropdown to provide convenient access to your [Template Variables](creatorkit/customize#template-variables):

![](https://servicestack.net/img/pages/creatorkit/portal-messages-markdown.png)

The implementation of `MarkdownEmail` just sends a Custom HTML Email configured to use the **basic** Layout with the **empty** Email Template:

```csharp
public async Task<object> Any(MarkdownEmail request)
{
    var contact = await Db.GetOrCreateContact(request);
    var viewRequest = request.ConvertTo<RenderCustomHtml>().FromContact(contact);
    viewRequest.Layout = "basic";
    viewRequest.Template = "empty";
    var bodyHtml = (string) await Gateway.SendAsync(typeof(string), viewRequest);

    var email = await Renderer.CreateMessageAsync(Db, new MailMessage
    {
        Draft = request.Draft ?? false,
        Message = new EmailMessage
        {
            To = contact.ToMailTos(),
            Subject = request.Subject,
            Body = request.Body,
            BodyHtml = bodyHtml,
        },
    }.FromRequest(viewRequest));
    return email;
}
```