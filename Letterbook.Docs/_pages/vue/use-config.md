---
title: Vue Tailwind Global Configuration
group: Library
---

## Manage Global Configuration

`useConfig` is used to maintain global configuration that's used throughout the Vue Component Library.

```ts
import { useConfig } from "@servicestack/vue"

const {
    config,                   // Resolve configuration in a reactive Ref<UiConfig>
    setConfig,                // Set global configuration
    assetsPathResolver,       // Resolve Absolute URL to use for relative paths
    fallbackPathResolver,     // Resolve fallback URL to use if primary URL fails
    autoQueryGridDefaults,    // Resolve AutoQueryGrid default configuration
    setAutoQueryGridDefaults, // Set AutoQueryGrid default configuration
} = useConfig()
```

The asset and fallback URL resolvers are useful when hosting assets on a separate CDN from the hosted website.

### Default configuration

```js
setConfig({
    redirectSignIn:       '/signin',
    assetsPathResolver:   src => src,
    fallbackPathResolver: src => src,
})
```

## AutoQueryGrid Defaults

Use `setAutoQueryGridDefaults` to change the default configuration for all [AutoQueryGrid](/vue/autoquerygrid) components:

```ts
const { setAutoQueryGridDefaults } = useConfig()

setAutoQueryGridDefaults({
    deny: [],
    hide: [],
    toolbarButtonClass: undefined,
    tableStyle: "stripedRows",
    take: 25,
    maxFieldLength: 150,
})
```

TypeScript Definitions for available AutoQueryGridDefaults:

```ts
type AutoQueryGridDefaults = {
    deny?:GridAllowOptions[]
    hide?:GridShowOptions[]
    toolbarButtonClass?: string
    tableStyle?: TableStyleOptions
    take?:number
    maxFieldLength?: number
}

export type GridAllowOptions = "filtering" | "queryString" | "queryFilters"
export type GridShowOptions = "toolbar" | "preferences" | "pagingNav" | "pagingInfo" | "downloadCsv" 
    | "refresh" | "copyApiUrl" | "resetPreferences" | "filtersView" | "newItem"
```

## TypeScript Definition

TypeScript definition of the API surface area and type information for correct usage of `useConfig()`

```ts
interface UiConfig {
    redirectSignIn?: string
    assetsPathResolver?: (src:string) => string
    fallbackPathResolver?: (src:string) => string
}

/** Resolve configuration in a reactive Ref<UiConfig> */
const config:ComputedRef<UiConfig>

/** Set global configuration */
function setConfig(config: UiConfig): void;

/** Resolve Absolute URL to use for relative paths */
function assetsPathResolver(src?: string): string | undefined;

/** Resolve fallback URL to use if primary URL fails */
function fallbackPathResolver(src?: string): string | undefined;
```