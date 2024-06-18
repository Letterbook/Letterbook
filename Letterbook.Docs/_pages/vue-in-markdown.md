---
title: Using Vue in Markdown
---

## Progressive Enhancement

Thanks to the Vue's elegant approach for progressively enhancing HTML content, Razor Press markdown documents can 
embed interactive reactive Vue components directly in Markdown which makes it possible to document the
[Vue Tailwind Component Library](/vue/autoquerygrid) and its interactive component examples, embedded directly in Markdown. 

## Markdown Documents are Vue Apps

We can embed Vue components directly in Markdown simply because all Markdown Documents are themselves Vue Apps, which by default 
are created with the same [/mjs/app.mjs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/wwwroot/mjs/app.mjs)
configuration that all Razor Pages uses. This allows using any 
[Vue DOM syntax](https://vuejs.org/guide/essentials/template-syntax.html) or global Vue components directly in Markdown, in the same
way that they're used in Vue Apps defined in Razor or HTML pages.


For example we can display the 
[GettingStarted.mjs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/wwwroot/mjs/components/GettingStarted.mjs) component
that's on the home page with:

#### Input

```html
<getting-started template="razor-press"></getting-started>
```

#### Output

<getting-started template="razor-press"></getting-started>

## Markdown with Custom Vue Apps

Pages that need advanced functionality beyond what's registered in the global App configuration can add additional
functionality by adding a JavaScript module with the same **path** and **filename** of the markdown page with
an `.mjs` extension:

```
/wwwroot/pages/<path>/<file>.mjs
```

This is utilized by most [/pages/vue](https://github.com/NetCoreTemplates/razor-press/tree/main/Letterbook.Docs/wwwroot/pages/vue)
Markdown pages to handle the unique requirements of each page's live examples.

E.g. the [autoquerygrid.mjs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/wwwroot/pages/vue/autoquerygrid.mjs)
uses a custom Vue App component that registers a custom **client** dependency and **Responsive** and **CustomBooking**
components that are only used in 
[/vue/autoquerygrid.md](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/_pages/vue/autoquerygrid.md)
page to render the interactive live examples in the [/vue/autoquerygrid](/vue/autoquerygrid) page:

```js
import { onMounted } from "vue"
import { Authenticate } from "./dtos.mjs"
import { useAuth, useClient } from '@servicestack/vue'
import { JsonApiClient } from "@servicestack/client"
import Responsive from "./autoquerygrid/Responsive.mjs"
import CustomBooking from "./autoquerygrid/CustomBooking.mjs"

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
        Responsive,
        CustomBooking,
    },
    setup() {
        const client = useClient()
        
        onMounted(async () => {
            const api = await client.api(new Authenticate({ 
                provider: 'credentials', userName:'admin@email.com', password:'p@55wOrd' }))
            if (api.succeeded) {
                const { signIn } = useAuth()
                signIn(api.response)
            }
        })
        return { }
    }
}
```

### Convert from Vue script setup

This is roughly equivalent to the sample below if coming from VitePress or other npm Vue project that uses Vue's 
compile-time syntactic [`<script setup>`](https://vuejs.org/api/sfc-script-setup.html):

```html
<script setup>
import { onMounted } from "vue"
import { Authenticate } from "./dtos.ts"
import { useAuth, useClient } from '@servicestack/vue'
import { JsonApiClient } from "@servicestack/client"
import Responsive from "./autoquerygrid/Responsive.vue"
import CustomBooking from "./autoquerygrid/CustomBooking.vue"

onMounted(async () => {
    const client = useClient()
    const api = await client.api(new Authenticate({ 
        provider: 'credentials', userName:'admin@email.com', password:'p@55wOrd' }))
    if (api.succeeded) {
        const { signIn } = useAuth()
        signIn(api.response)
    }
})
</script>
```

## Custom Styles in Markdown

If needed custom styles can also be added for individual pages by adding a `.css` file at the following location:

```
/wwwroot/pages/<path>/<file>.css
```
