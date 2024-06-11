# A Year in the Life

First, let me wish a happy birthday 🎂 to this toot:

<iframe src="https://tech.lgbt/@jenniferplusplus/110567090111682149/embed" width="400" allowfullscreen="allowfullscreen" sandbox="allow-scripts allow-same-origin allow-popups allow-popups-to-escape-sandbox allow-forms"></iframe>

As much as any single thing can be, this is where Letterbook began. From the start, this was a project with an *ambitious* scope. It's been slower than I wanted, but it's starting to come together as usable app. Letterbook can now support a complete, if limited, usage loop. You can:

1. Sign up
2. Login
3. Post
4. Have followers
5. Follow other people
6. Scroll through posts from people you follow

## A Little Review

This is all federated over ActivityPub. As a spec, ActivityPub is _really_ easy to implement for toy projects, and _really_ hard to implement for robust long-lived applications. The datatypes are notably difficult to implement. And CSharp does actually make that harder than some other languages would have. A lot of projects lean heavily on their language's dynamic typing system to handle ActivityPub documents. But that seems like it's a trap. Given how fundamental ActivityPub is to Letterbook (and all fediverse services), I think it's well worth it to have robust and type-safe parsing. Which we do, thanks to [ActivityPubSharp][apsharp].

In practice, ActivityPub depends on HttpMessageSignatures. That RFC was recently published, but in practice, the fediverse implements a draft of that RFC from 2017. It's not a broadly implemented spec, but there is some support for it. Of course, the general purpose implementations track the RFC. Which is to say, that's another thing we implemented for ourselves, although in this case we had a decent foundation to start from, in the form of [NSign][nsign].

The final really core fediverse protocol that you have to implement for real world interoperability is Webfinger. It will play a supporting role for letterbook, but Mastodon relies on it extensively. Fortunately, that one required only minor adjustments to the package from [DarkLink][webfinger].

But, protocols are not an application. This is public, web hosted, multi-user software. We need to support account and identity management. And as a social web service, it doesn't make much sense to depend on external identity providers to do that. Which is too bad, it would have been *way* easier. Taking on identity management means handling usernames, contact info, and passwords. Which is absolutely not something I want to roll myself. Fortunately, Microsoft has us covered there, with [AspNet Identity][msidentity], another in the long tradition of Microsoft just capitalizing a generic  noun with a specific technical meaning and calling that a product name. Of course, integrating this is actually pretty hard. Microsoft _really_ wants you to just use Azure whatever.

Social networks and ActivityPub both more or less demand a work queueing system. ActivityPub explicitly specifies that most of the functional behavior is implemented as side effects, often out of band with the HTTP request/response cycle that prompted it. And almost every post you make on the fediverse has to be sent to all of your followers. Which can easily be hundreds of POST messages for even relatively small followings. It's not feasible to handle that volume of potentially very flaky work inline with a single API request. After building and then ditching a temporary solution to that problem, our work queues are now managed by [MassTransit][masstransit]. Everything from composing timelines and sending notifications to media processing and spam filtering will flow through those queue workers.

The rest of the stack is getting into very familiar territory for most dotnet developers. AspNet MVC and AspNet WebAPI power the web and REST interfaces to the system. Persistence is managed through EntityFramework Core, on top of Postrgresql as the system of record. Letterbook will have a number of chronological feeds, and those are powered by a [Timescale][timescale] database, also managed through EFCore.

# How Do You Measure a Year

Is it five hundred twenty-five thousand, six hundred minutes? I guess. Midnights? Cups of coffee? Actually, yeah, it was a lot more of both of those than is probably wise. It was also:

- 900 commits
- 296 tests
- 146 pull requests
- 546 CI builds
- 119 issues
- 546 source files (I wanted to count lines of code, but MS is being uncooperative and I refuse to use VisualStudio)
- 35 pages of docs (including this one! So meta)

But most importantly: **_13 contributors_**

## Thank You



[apsharp]: https://github.com/warriordog/activitypubsharp
[nsign]: https://github.com/Unisys/NSign
[webfinger]: https://github.com/WiiPlayer2/DarkLink.Web.ActivityPub 
[msidentity]: https://github.com/aspnet/AspNetIdentity
[masstransit]: https://masstransit.io
[timescale]: https://timescale.com