---
title: Installation
---

## Manual Installation

**@servicestack/vue** can be added to existing Vue SPA Apps by installing via npm:

```bash
$ npm install @servicestack/vue
```

Where it will also install its **vue** and **@servicestack/client** dependencies.

## Installation-less option

Alternatively you can take advantage of modern browsers [JS Modules](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Modules) support to
use these libraries without installation by registering an [importmap](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/script/type/importmap) to define where it should load the ESM builds of these libraries from, e.g:

```html
<script type="importmap">
{
    "imports": {
        "vue":                  "https://unpkg.com/vue@3/dist/vue.esm-browser.prod.js",
        "@servicestack/client": "https://unpkg.com/@servicestack/client@2/dist/servicestack-client.min.mjs",
        "@servicestack/vue":    "https://unpkg.com/@servicestack/vue@3/dist/servicestack-vue.min.mjs"
    }
}
</script>
```

For intranet Web Apps that need to work without internet access, save and reference local copies of these libraries, e.g:

```html
<script type="importmap">
{
    "imports": {
        "vue":                  "/lib/mjs/vue.mjs",
        "@servicestack/client": "/lib/mjs/servicestack-client.mjs",
        "@servicestack/vue":    "/lib/mjs/servicestack-vue.mjs"
    }
}
</script>
```

## @Html.ImportMap

Razor Pages or MVC Apps can use the `Html.ImportMaps()` to use local debug builds during development and optimal CDN hosted minified production builds in production:

```csharp
@Html.ImportMap(new()
{
    ["vue"]                  = ("/lib/mjs/vue.mjs",                 "https://unpkg.com/vue@3/dist/vue.esm-browser.prod.js"),
    ["@servicestack/client"] = ("/lib/mjs/servicestack-client.mjs", "https://unpkg.com/@servicestack/client@2/dist/servicestack-client.min.mjs"),
    ["@servicestack/vue"]    = ("/lib/mjs/servicestack-vue.mjs",    "https://unpkg.com/@servicestack/vue@3/dist/servicestack-vue.min.mjs")
})
```

> It's recommended to use exact versions to eliminate redirect latencies and to match the local version your App was developed against

### Polyfill for Safari

Unfortunately Safari is the last modern browser to [support import maps](https://caniuse.com/import-maps) which is only now in
Technical Preview. Luckily this feature can be polyfilled with the pre-configured [ES Module Shims](https://github.com/guybedford/es-module-shims):

```html
@if (Context.Request.Headers.UserAgent.Any(x => x.Contains("Safari") && !x.Contains("Chrome")))
{
    <script async src="https://ga.jspm.io/npm:es-module-shims@1.6.3/dist/es-module-shims.js"></script>
}
```

## Registration

Then register the `@servicestack/vue` component library with your Vue app with:

```js
import { JsonApiClient } from "@servicestack/client"
import ServiceStackVue from "@servicestack/vue"

const client = JsonApiClient.create()

const app = createApp(component, props)
app.provide('client', client)
app.use(ServiceStackVue)

//...
app.mount('#app')
```

The **client** instance is used by API-enabled components to call your APIs using the [/api predefined route](/routing#json-api-pre-defined-route). ServiceStack Apps not running on .NET 6+ or have the **/api** route disabled should use
`JsonServiceClient` instead:

```js
const client = new JsonServiceClient()
```

## Not using Vue Router

Non SPA Vue Apps that don't use [Vue Router](https://router.vuejs.org) should register a replacement `<router-link>` component
that uses the browser's native navigation in [navigational components](/vue/navigation):

```js
app.component('RouterLink', ServiceStackVue.component('RouterLink'))
```
