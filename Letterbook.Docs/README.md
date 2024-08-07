# razor-press

.NET 6.0 razor-press Static Generated Razor Pages Project Template

[![](https://raw.githubusercontent.com/ServiceStack/Assets/master/csharp-templates/razor-press.png)](https://razor-press.web-templates.io)

> Browse [source code](https://github.com/NetCoreTemplates/razor-press), view live demo [razor-press.web-templates.io](https://razor-press.web-templates.io) and install with [dotnet-new](https://docs.servicestack.net/dotnet-new):

    $ dotnet tool install -g x

    $ x new razor-press ProjectName

Alternatively write new project files directly into an razor-press repository, using the Directory Name as the ProjectName:

    $ git clone https://github.com/<User>/<ProjectName>.git
    $ cd <ProjectName>
    $ x new razor-press

## Development Tasks

Tasks to run live reload Tailwind and .NET Razor Pages App during development:

### Run Tailwind in Background

    $ npm run ui:dev

### Run .NET App with Live Reload

    $ dotnet watch

## Local Production Tasks

Tasks to prerender and preview production ssg website build:

### Prerender website

    $ npm run prerender

### Preview pre-rendered website

    $ npm run serve
