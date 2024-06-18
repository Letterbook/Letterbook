---
title: Navigation Components
group: Component Gallery
---

<api-reference component="Tabs"></api-reference>
## Tabs

The `<Tabs>` component lets you switch between different Vue components from a object component dictionary where the **Key** is used for the Tab's label and URL param and the **Value** component for the tab body. 

```html
<script setup>
import A from "./A.vue"
import B from "./B.vue"
import C from "./C.vue"
const tabs = { A, B, C }
</script>
```

The Tab's Label can alternatively be overridden with a custom **label** function, e.g:

```html
<Tabs :tabs="tabs" :label="tab => `${tab} Tab Label`" />
```
<tabs :tabs="tabs" :label="tab => `${tab} Tab Label`" class="not-prose mb-8"></tabs>

**Tabs properties**

```ts
defineProps<{
    tabs: {[name:string]:Component }
    id?: string                      //= tabs
    param?: string                   //= tab - URL param to use
    label?: (tab:string) => string   // - Custom function to resolve Tab Label
    selected?: string                // - The selected tab
    tabClass?: string                // - Additional classes for Tab Label
    bodyClass?: string               // - Classes for Tab Body
    url?:boolean                     //= true - Whether to maintain active tab in history.pushState()
}>()
```

<api-reference component="Breadcrumbs"></api-reference>
## Breadcrumbs

Breadcrumb example:

```html
<Breadcrumbs home-href="/vue/">
  <Breadcrumb href="/vue/gallery">gallery</Breadcrumb>
  <Breadcrumb>Navigation Examples</Breadcrumb>
</Breadcrumbs>
```

<Breadcrumbs class="not-prose my-8" home-href="/vue/">
  <Breadcrumb href="/vue#gallery">gallery</Breadcrumb>
  <Breadcrumb>Navigation Examples</Breadcrumb>
</Breadcrumbs>

<api-reference component="NavList"></api-reference>
## NavList

Use `NavList` for rendering a vertical navigation list with Icons:

```html
<NavList title="Explore Vue Tailwind Components">
    <NavListItem title="DataGrid" href="/vue/gallery/datagrid" :iconSvg="Icons.DataGrid">
        DataGrid Component Examples for rendering tabular data
    </NavListItem>
    <NavListItem title="AutoQuery Grid" href="/vue/gallery/autoquerygrid" :iconSvg="Icons.AutoQueryGrid">
        Instant customizable UIs for calling AutoQuery CRUD APIs
    </NavListItem>
</NavList>
```

<div class="my-8 not-prose">
    <nav-list-examples class="max-w-3xl mx-auto"></nav-list-examples>
</div>

<api-reference component="PrimaryButton"></api-reference>
## Link Buttons

Using `href` with Button components will style hyper links to behave like buttons:

```html
<PrimaryButton href="https://vue-mjs.web-templates.io/" class="mr-2">
    Vue.mjs Template
</PrimaryButton>

<SecondaryButton href="/vue/">
    Vue Component Docs
</SecondaryButton>
```

<div class="my-8 not-prose">
    <primary-button href="https://vue-mjs.web-templates.io/" class="mr-2">Vue.mjs Template</primary-button>
    <secondary-button href="/vue/">Vue Component Docs</secondary-button>
</div>

<api-reference component="PrimaryButton"></api-reference>
## PrimaryButton

That can use **color** to render it in different colors:

```html
<PrimaryButton>Default</PrimaryButton>
<PrimaryButton color="blue">Blue</PrimaryButton>
<PrimaryButton color="purple">Purple</PrimaryButton>
<PrimaryButton color="red">Red</PrimaryButton>
<PrimaryButton color="green">Green</PrimaryButton>
<PrimaryButton color="sky">Sky</PrimaryButton>
<PrimaryButton color="cyan">Cyan</PrimaryButton>
<PrimaryButton color="indigo">Indigo</PrimaryButton>
```

<div class="my-8 not-prose space-x-2">
    <primary-button>Default</primary-button>
    <primary-button color="blue">Blue</primary-button>
    <primary-button color="purple">Purple</primary-button>
    <primary-button color="red">Red</primary-button>
    <primary-button color="green">Green</primary-button>
    <primary-button color="sky">Sky</primary-button>
    <primary-button color="cyan">Cyan</primary-button>
    <primary-button color="indigo">Indigo</primary-button>
</div>

<api-reference component="TextLink"></api-reference>
## TextLink

Tailwind `<a>` hyper links, e.g:

```html
<TextLink href="/vue/" class="text-xl">
    docs.servicestack.net/vue
</TextLink>
```

<div class="not-prose">
<text-link href="/vue/" class="text-xl">docs.servicestack.net/vue</text-link>
</div>

That can also use **color** to render it in different colors:

```html
<TextLink @click="say('Hi!')" title="Greetings">Default <b>Link</b></TextLink>
<TextLink color="purple" href="https://google.com" target="_blank" title="Google Link">Purple <b>Link</b></TextLink>
<TextLink color="red"    href="https://google.com" target="_blank" title="Google Link">Red <b>Link</b></TextLink>
<TextLink color="green"  href="https://google.com" target="_blank" title="Google Link">Green <b>Link</b></TextLink>
<TextLink color="sky"    href="https://google.com" target="_blank" title="Google Link">Sky <b>Link</b></TextLink>
<TextLink color="cyan"   href="https://google.com" target="_blank" title="Google Link">Cyan <b>Link</b></TextLink>
<TextLink color="indigo" href="https://google.com" target="_blank" title="Google Link">Indigo <b>Link</b></TextLink>
```

<div class="not-prose flex space-x-4">
  <text-link @click="say('Hi!')" title="Greetings">Default <b>Link</b></text-link>
  <text-link color="purple" href="https://google.com" target="_blank" title="Google Link">Purple <b>Link</b></text-link>
  <text-link color="red"    href="https://google.com" target="_blank" title="Google Link">Red <b>Link</b></text-link>
  <text-link color="green"  href="https://google.com" target="_blank" title="Google Link">Green <b>Link</b></text-link>
  <text-link color="sky"    href="https://google.com" target="_blank" title="Google Link">Sky <b>Link</b></text-link>
  <text-link color="cyan"   href="https://google.com" target="_blank" title="Google Link">Cyan <b>Link</b></text-link>
  <text-link color="indigo" href="https://google.com" target="_blank" title="Google Link">Indigo <b>Link</b></text-link>
</div>
