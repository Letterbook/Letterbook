---
title: App Metadata
group: Library
---

The rich server metadata about your APIs that's used to generate your App's DTOs in 
[Multiple Programming Languages](/add-servicestack-reference), power ServiceStack's 
[built-in Auto UIs](/locode/declarative) also power the Metadata driven components in the **@servicestack/vue** component library 
where it can be loaded in your `_Layout.cshtml` using an optimal configuration like:

```html
var dev = HostContext.AppHost.IsDevelopmentEnvironment();
@if (dev) {
    <script>window.Server = @await Html.ApiAsJsonAsync(new MetadataApp())</script>
}

<script type="module">
import { useMetadata } from "@@servicestack/vue"

const { loadMetadata } = useMetadata()
loadMetadata({
    olderThan: window.Server ? null : location.search.includes('clear=metadata') ? 0 : 60 * 60 * 1000 //1hr 
})
</script>
```

Where during development it always embeds the AppMetadata in each page but as this metadata can become quite large for systems with a lot of APIs, the above optimization clears and reloads the AppMetadata after **1 hr** or if the page was explicitly loaded with `?clear=metadata`, 
otherwise it will use a local copy cached in `localStorage` at `/metadata/app.json`, which Apps needing more 
fine-grained cache invalidation strategies can manage themselves.

Once loaded the AppMetadata features can be access with the helper functions in [useMetadata](/vue/use-metadata).

```js
import { useMetadata } from "@servicestack/vue"

const { 
    loadMetadata,      // Load {AppMetadata} if needed 
    setMetadata,       // Explicitly set AppMetadata and save to localStorage
    clearMetadata,     // Delete AppMetadata and remove from localStorage
    metadataApi,       // Reactive accessor to Ref<MetadataTypes>
    typeOf,            // Resolve {MetadataType} for DTO name
    typeOfRef,         // Resolve {MetadataType} by {MetadataTypeName}
    apiOf,             // Resolve Request DTO {MetadataOperationType} by name
    property,          // Resolve {MetadataPropertyType} by Type and Property name
    enumOptions,       // Resolve Enum entries for Enum Type by name
    propertyOptions,   // Resolve allowable entries for property by {MetadataPropertyType}
    createFormLayout,  // Create Form Layout's {InputInfo[]} from {MetadataType}
    typeProperties,    // Return all properties (inc. inherited) for {MetadataType}
    supportsProp,      // Check if a supported HTML Input exists for {MetadataPropertyType}
    Crud,              // Query metadata information about AutoQuery CRUD Types
    getPrimaryKey,     // Resolve PrimaryKey {MetadataPropertyType} for {MetadataType}
    getId,             // Resolve Primary Key value from {MetadataType} and row instance
    createDto,         // Create a Request DTO instance for Request DTO name
    toFormValues,      // Convert Request DTO values to supported HTML Input values
    formValues,        // Convert HTML Input values to supported DTO values
} = useMetadata()
```

For example you can use this to view all C# property names and Type info for the `Contact` C# DTO with:

```html
<HtmlFormat :value="typeOf('Contact').properties.map(({ name, type, namespace }) => ({ name, type, namespace }))" />
```
<div class="not-prose">
<html-format :value="typeOf('Contact').properties.map(({ name, type, namespace }) => ({ name, type, namespace }))"></html-format>
</div>

## Enum Values and Property Options

More usefully this can avoid code maintenance and duplication efforts from maintaining enum values on both server and client forms. 

An example of this is in the [Contacts.mjs](https://github.com/NetCoreTemplates/razor-tailwind/blob/main/Letterbook.Docs/wwwroot/Pages/Contacts.mjs) 
component which uses the server metadata to populate the **Title** and **Favorite Genre** select options from the `Title` and `FilmGenre` enums:

```html
<div class="grid grid-cols-6 gap-6">
  <div class="col-span-6 sm:col-span-3">
    <SelectInput id="title" v-model="request.title" :options="enumOptions('Title')" />
  </div>
  <div class="col-span-6 sm:col-span-3">
    <TextInput id="name" v-model="request.name" required placeholder="Contact Name" />
  </div>
  <div class="col-span-6 sm:col-span-3">
    <SelectInput id="color" v-model="request.color" :options="colorOptions" />
  </div>
  <div class="col-span-6 sm:col-span-3">
    <SelectInput id="favoriteGenre" v-model="request.favoriteGenre" :options="enumOptions('FilmGenre')" />
  </div>
  <div class="col-span-6 sm:col-span-3">
    <TextInput type="number" id="age" v-model="request.age" />
  </div>
</div>
```

Whilst the `colorOptions` gets its values from the available options on the `CreateContact.Color` property:    

```js
const Edit = {
    //...
    setup(props) {
        const { property, propertyOptions, enumOptions } = useMetadata()
        const colorOptions = propertyOptions(property('CreateContact','Color'))
        return { enumOptions, colorOptions }
        //..
    }
}
```

Which instead of an enum, references the C# Dictionary in:

```csharp
public class CreateContact : IPost, IReturn<CreateContactResponse>
{
    [Input(Type="select", EvalAllowableEntries = "AppData.Colors")]
    public string? Color { get; set; }
    //...
}
```

To return a C# Dictionary of custom colors defined in:

```csharp
public class ConfigureUi : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureAppHost(appHost => {
            //Enable referencing AppData.* in #Script expressions
            appHost.ScriptContext.Args[nameof(AppData)] = AppData.Instance;
        });
}

public class AppData
{
    public static readonly AppData Instance = new();
    public Dictionary<string, string> Colors { get; } = new() {
        ["#F0FDF4"] = "Green",
        ["#EFF6FF"] = "Blue",
        ["#FEF2F2"] = "Red",
        ["#ECFEFF"] = "Cyan",
        ["#FDF4FF"] = "Fuchsia",
    };
}
```

## AutoForm Components

See [Auto Form Components](/vue/autoform) docs for examples of easy to use, high productivity `AppMetadata` powered components.

## TypeScript Definition

TypeScript definition of the API surface area and type information for correct usage of `useMetadata()`

```ts
import type { 
    AppMetadata, MetadataType, MetadataPropertyType, MetadataOperationType, InputInfo, KeyValuePair 
} from "./types"


/** Load {AppMetadata} if needed 
 * @param olderThan   - Reload metadata if age exceeds ms
 * @param resolvePath - Override `/metadata/app.json` path use to fetch metadata
 * @param resolve     - Use a custom fetch to resolve AppMetadata
*/
function loadMetadata(args: {
    olderThan?: number;
    resolvePath?: string;
    resolve?: () => Promise<Response>;
}): Promise<AppMetadata>;

/** Check if AppMetadata is valid */
function isValid(metadata: AppMetadata | null | undefined): boolean | undefined;

/** Delete AppMetadata and remove from localStorage */
function setMetadata(metadata: AppMetadata | null | undefined): boolean;

/** Delete AppMetadata and remove from localStorage */
function clearMetadata(): void;


/** Query metadata information about AutoQuery CRUD Types */
const Crud: {
    Create: string;
    Update: string;
    Patch: string;
    Delete: string;
    AnyRead: string[];
    AnyWrite: string[];
    isQuery: (op: MetadataOperationType) => any;
    isCrud: (op: MetadataOperationType) => boolean | undefined;
    isCreate: (op: MetadataOperationType) => boolean | undefined;
    isUpdate: (op: MetadataOperationType) => boolean | undefined;
    isPatch: (op: MetadataOperationType) => boolean | undefined;
    isDelete: (op: MetadataOperationType) => boolean | undefined;
    model: (type?: MetadataType | null) => string | null | undefined;
};

/** Resolve HTML Input type to use for {MetadataPropertyType}  */
function propInputType(prop: MetadataPropertyType): string;

/** Resolve HTML Input type to use for C# Type name */
function inputType(type: string): string;

/** Check if C# Type name is numeric */
function isNumericType(type?: string | null): boolean;

/** Check if C# Type is an Array or List */
function isArrayType(type: string): boolean;

/** Check if a supported HTML Input exists for {MetadataPropertyType} */
function supportsProp(prop?: MetadataPropertyType): boolean;

/** Create a Request DTO instance for Request DTO name */
function createDto(name: string, obj?: any): any;

/** Convert Request DTO values to supported HTML Input values */
function toFormValues(dto: any, metaType?: MetadataType | null): any;

/** Convert HTML Input values to supported DTO values */
function formValues(form: HTMLFormElement, props?: MetadataPropertyType[]): {
    [k: string]: any;
};

/**
 * Resolve {MetadataType} for DTO name
 * @param name        - Find MetadataType by name
 * @param [namespace] - Find MetadataType by name and namespace 
 */
function typeOf(name?: string | null, namespace?: string | null): MetadataType | null;

/** Resolve Request DTO {MetadataOperationType} by name */
function apiOf(name: string): MetadataOperationType | null;

/** Resolve {MetadataType} by {MetadataTypeName} */
function typeOfRef(ref?: {
    name: string;
    namespace?: string;
}): MetadataType | null;

function property(typeName: string, name: string): MetadataPropertyType | null;

/** Resolve Enum entries for Enum Type by name */
function enumOptions(name: string): { [name: string]: string; } | null;

function enumOptionsByType(type?: MetadataType | null): { [name: string]: string; } | null;

/** Resolve Enum entries for Enum Type by MetadataType */
function propertyOptions(prop: MetadataPropertyType): { [name: string]: string; } | null;

/** Convert string dictionary to [{ key:string, value:string }] */
function asKvps(options?: { [k: string]: string; } | null): KeyValuePair<string, string>[] | undefined;

/** Create InputInfo from MetadataPropertyType and custom InputInfo */
function createInput(prop: MetadataPropertyType, input?: InputInfo): InputInfo;

/** Create Form Layout's {InputInfo[]} from {MetadataType} */
function createFormLayout(metaType?: MetadataType | null): InputInfo[];

/** Return all properties (inc. inherited) for {MetadataType} */
function typeProperties(type?: MetadataType | null): MetadataPropertyType[];

/** Check if MetadataOperationType implements interface by name */
function hasInterface(op: MetadataOperationType, cls: string): boolean;

/** Resolve PrimaryKey {MetadataPropertyType} for {MetadataType} */
function getPrimaryKey(type?: MetadataType | null): MetadataPropertyType | null;

/** Resolve Primary Key value from {MetadataType} and row instance  */
function getId(type: MetadataType, row: any): any;
```