---
title: Mail Runs
order: 8
group: Portal
---

Mail Runs is where you would go to craft and send emails to an entire Mailing List group. It has the same Simple,
Markdown and Custom HTML Email UIs as [Messages](creatorkit/portal-messages) except instead of a single contact, 
it will generate and send individual emails to every contact in the specified **Mailing List**:

![](https://servicestack.net/img/pages/creatorkit/portal-mailrun-custom.png)

You'll also be able to send personalized emails with the contact's `{{Email}}`, `{{FirstName}}` and `{{LastName}}`
template variables.

### MailRun Implementation

All Mail Run APIs inherit `MailRunBase` which contains the Mailing List that the Mail Run should send emails to:

```csharp
public abstract class MailRunBase
{
    [ValidateNotEmpty]
    public MailingList MailingList { get; set; }
}
```

It has the equivalent Standard, Markdown and Custom HTML Emails that messages has, which instead inherits from `MailRunBase`,
e.g. here's the Request DTO definition that's used to render the above **Custom HTML Email** UI:

```csharp
[Renderer(typeof(RenderCustomHtml))]
[Tag(Tag.Mail), ValidateIsAdmin]
[Icon(Svg = Icons.RichHtml)]
[Description("Custom HTML Email")]
public class CustomHtmlMailRun : MailRunBase, IPost, IReturn<MailRunResponse>
{
    [ValidateNotEmpty]
    [Input(Type = "combobox", EvalAllowableValues = "AppData.EmailLayoutOptions")]
    public string Layout { get; set; }
    [ValidateNotEmpty]
    [Input(Type = "combobox", EvalAllowableValues = "AppData.EmailTemplateOptions")]
    public string Template { get; set; }
    [ValidateNotEmpty]
    public string Subject { get; set; }
    [ValidateNotEmpty]
    [Input(Type = "MarkdownEmailInput", Label = ""), FieldCss(Field = "col-span-12", Input = "h-56")]
    public string? Body { get; set; }
}
```

It's implementation differs slightly from the Messages
[Custom HTML Implementation](creatorkit/portal-messages#custom-html-implementation) as an email needs to be generated
and sent per contact and are instead generated and saved to the `MailMessageRun` table:

```csharp
public async Task<object> Any(CustomHtmlMailRun request)
{
    var response = CreateMailRunResponse();
    
    var mailRun = await Renderer.CreateMailRunAsync(Db, new MailRun {
        Layout = request.Layout,
        Template = request.Template,
    }, request);
    
    foreach (var sub in await Db.GetActiveSubscribersAsync(request.MailingList))
    {
        var viewRequest = request.ConvertTo<RenderCustomHtml>().FromContact(sub);
        var bodyHtml = (string) await Gateway.SendAsync(typeof(string), viewRequest);

        response.AddMessage(await Renderer.CreateMessageRunAsync(Db, new MailMessageRun
        {
            Message = new EmailMessage
            {
                To = sub.ToMailTos(),
                Subject = request.Subject,
                Body = request.Body,
                BodyHtml = bodyHtml,
            }
        }.FromRequest(viewRequest), mailRun, sub));
    }
    
    await Db.CompletedMailRunAsync(mailRun, response);
    return response;
}
```

### Verifying Mail Run Messages

Creating a Mail Run generates messages for each Contact in the Mailing List, but doesn't send them immediately, 
it instead opens the saved Mail Run so you have an opportunity to inspect the generated messages to decide whether
you want to send or delete the messages.

![](https://servicestack.net/img/pages/creatorkit/portal-mailrun-newsletter-send.png)

Click **View Messages** to inspect a sample of the generated messages from the saved Mail Run then either 
**Send Messages** if you want to send them out or **Delete** to delete the Mail Run and start again.

Whilst the Mail Run Messages are being sent out you can click Refresh to monitor progress. 

## CreatorKit.Extensions

Any additional services should be maintained in [CreatorKit.Extensions](https://github.com/NetCoreApps/CreatorKit/tree/main/CreatorKit.Extensions)
project with any custom Mail Run implementations added to
[CustomEmailRunServices.cs](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit.Extensions/CustomEmailRunServices.cs).

## Generating Newsletters

The `NewsletterMailRun` API is an advanced Email Generation example for generating a Monthly Newsletter - that it automatically 
generates from new content added to [Razor SSG](https://razor-ssg.web-templates.io/posts/razor-ssg) Websites that it 
discovers from its pre-rendered API JSON metadata.

Even if you're not using Razor SSG website it should still serve as a good example for how to implement a Mail Run for
a custom mail campaign utilizing custom data sources.

The `NewsletterMailRun` API has 2 optional properties for the Year and Month you want to generate the Newsletter for:

```csharp
[Renderer(typeof(RenderNewsletter))]
[Tag(Tag.Emails)]
[ValidateIsAdmin]
[Description("Generate Newsletter")]
[Icon(Svg = Icons.Newsletter)]
public class NewsletterMailRun : MailRunBase, IPost, IReturn<MailRunResponse>
{
    public int? Month { get; set; }
    public int? Year { get; set; }
}
```

Which renders the **Generate Newsletter** UI: 

![](https://servicestack.net/img/pages/creatorkit/portal-mailrun-newsletter.png)

The implementation follows the standard Mail Run implementation, using the `EmailRenderer` to creating a `MailMessageRun`
for every contact in the mailing list. 

We can also see it will default to the current Month/Year if not provided and that it uses the 
[marketing.html](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit/emails/layouts/marketing.html) Layout and
the [newsletter.html](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit/emails/newsletter.html)
Email template:

```csharp
public async Task<object> Any(NewsletterMailRun request)
{
    var response = CreateMailRunResponse();
    request.Year ??= DateTime.UtcNow.Year;
    request.Month ??= DateTime.UtcNow.Month;

    var viewRequest = request.ConvertTo<RenderNewsletter>();
    var fromDate = new DateTime(request.Year.Value, request.Month.Value, 1);
    var bodyHtml = (string) await Gateway.SendAsync(typeof(string), viewRequest);

    var mailRun = await Renderer.CreateMailRunAsync(Db, new MailRun {
        Layout = "marketing",
        Template = "newsletter",
    }, request);
    
    foreach (var sub in await Db.GetActiveSubscribersAsync(request.MailingList))
    {
        response.AddMessage(await Renderer.CreateMessageRunAsync(Db, new MailMessageRun
        {
            Message = new EmailMessage
            {
                To = sub.ToMailTos(),
                Subject = string.Format(AppData.Info.NewsletterFmt, 
                    $"{fromDate:MMMM} {fromDate:yyyy}"),
                BodyHtml = bodyHtml,
            }
        }.FromRequest(viewRequest), mailRun, sub));
    }

    await Db.CompletedMailRunAsync(mailRun, response);
    return response;
}
```

The **newsletter.html** Email Template uses the #Script templating language to render the different Newsletter sections, e.g:

```html
{{#if meta.Posts.Count > 0 }}
{{ 'divider' |> partial }}
{{ 'section' |> partial({ iconSrc:images.blog_48x48, title:'New Posts' }) }}
<tr>
    <td width="100%" align="left" valign="top">
        {{#each meta.Posts }}
            <h3><a href="{{ it.url }}" target="_blank">{{ it.title }} →</a></h3>
            <p>{{ it.summary }}</p>
        {{/each}}
    </td>
</tr>
{{/if}}
```

Which uses the `RenderNewsletter` API to render the Newsletter emails and live previews which in addition to the App's
template variables adds a `meta` property containing the Data Source for the contents in **newsletter.html**:

```csharp
public async Task<object> Any(RenderNewsletter request)
{
    var year = request.Year ?? DateTime.UtcNow.Year;
    var fromDate = new DateTime(year, request.Month ?? 1, 1);
    var meta = await MailData.SearchAsync(fromDate: fromDate,
        toDate: request.Month != null ? new DateTime(year, request.Month.Value, 1).AddMonths(1) : null);
    
    var context = Renderer.CreateMailContext(layout:"marketing", page:"newsletter", 
        args:new() {
            ["meta"] = meta
        });

    return await Renderer.RenderToHtmlResultAsync(Db, context, request, args: new() {
        ["title"] = $"{fromDate:MMMM} {fromDate:yyyy}"
    });
}
```

The implementation of [MailData](https://github.com/NetCoreApps/CreatorKit/blob/main/CreatorKit.ServiceInterface/MailData.cs)
gets its data from [/meta/2023/all.json](https://razor-ssg.web-templates.io/meta/2023/all.json) which is prerendered with 
all the new website content added in **2023** which is filtered further to only include content published within the 
selected date range:


```csharp
public class MailData
{
    public DateTime LastUpdated { get; set; }
    public AppData AppData { get; }

    public MailData(AppData appData)
    {
        AppData = appData;
    }

    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(10);
    public ConcurrentDictionary<int, SiteMeta> MetaCache { get; } = new();

    public async Task<SiteMeta> SearchAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var year = fromDate?.Year ?? DateTime.UtcNow.Year;
        var metaCache = MetaCache.TryGetValue(year, out var siteMeta) 
            && siteMeta.CreatedDate < DateTime.UtcNow.Add(CacheDuration)
            ? siteMeta
            : null;

        if (metaCache == null)
        {
            var metaJson = await AppData.BaseUrl.CombineWith($"/meta/{year}/all.json")
                .GetJsonFromUrlAsync();
            metaCache = metaJson.FromJson<SiteMeta>();
            metaCache.CreatedDate = DateTime.UtcNow;
            MetaCache[year] = metaCache;
        }

        var results = new SiteMeta
        {
            CreatedDate = metaCache.CreatedDate,
            Pages = WithinRange(metaCache.Pages, fromDate, toDate).ToList(),
            Posts = WithinRange(metaCache.Posts, fromDate, toDate).ToList(),
            WhatsNew = WithinRange(metaCache.WhatsNew, fromDate, toDate).ToList(),
            Videos = WithinRange(metaCache.Videos, fromDate, toDate).ToList(),
        };
        return results;
    }

    private static IEnumerable<MarkdownFile> WithinRange(
        IEnumerable<MarkdownFile> docs, DateTime? fromDate, DateTime? toDate)
    {
        if (fromDate != null)
            docs = docs.Where(x => x.Date >= fromDate);
        if (toDate != null)
            docs = docs.Where(x => x.Date < toDate);
        return docs;
    }
}
```

The results of the external API Request are also cached for a short duration to speed up Live Previews when crafting
emails.