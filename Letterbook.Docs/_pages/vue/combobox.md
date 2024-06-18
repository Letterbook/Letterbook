---
title: Combobox Component
group: Component Gallery
---

The `Combobox` component provides an Autocomplete Input optimized for searching a List of string values, Key Value Pairs or Object Dictionary, e.g:

```html
<div class="grid grid-cols-12 gap-6">
  <Combobox id="Strings" class="col-span-4" v-model="strings" :values="['Alpha','Bravo','Charlie']" />
  <Combobox id="Object"  class="col-span-4" v-model="objects" :options="{ A:'Alpha', B:'Bravo', C:'Charlie' }" />
  <Combobox id="Pairs"   class="col-span-4" v-model="pairs"   label="Multiple from Pairs" multiple
    :entries="[{key:'A',value:'Alpha'}, {key:'B',value:'Bravo'}, {key:'C',value:'Charlie'}]" />
</div>
```

<div class="not-prose grid grid-cols-12 gap-6">
  <combobox id="Strings" class="col-span-4" v-model="strings" :values="['Alpha','Bravo','Charlie']"></combobox>
  <combobox id="Object"  class="col-span-4" v-model="objects" :options="{ A:'Alpha', B:'Bravo', C:'Charlie' }"></combobox>
  <combobox id="Pairs"   class="col-span-4" v-model="pairs"   label="Multiple from Pairs" multiple
    :entries="[{key:'A',value:'Alpha'}, {key:'B',value:'Bravo'}, {key:'C',value:'Charlie'}]"></combobox>
</div>

Which supports populating both a single string value or multiple strings in an Array with **multiple** property.

<api-reference component="Combobox"></api-reference>
## Auto Forms

Combobox components can also be used in [Auto Form Components](/vue/autoform) on `string` or string collection properties
with the `[Input(Type="combobox")]` [declarative UI Attribute](/locode/declarative#ui-metadata-attributes) on C# Request DTOs, e.g:

```csharp
public class ComboBoxExamples : IReturn<ComboBoxExamples>, IPost
{
    [Input(Type="combobox", Options = "{ allowableValues:['Alpha','Bravo','Charlie'] }")]
    public string? SingleClientValues { get; set; }

    [Input(Type="combobox", Options = "{ allowableValues:['Alpha','Bravo','Charlie'] }", Multiple = true)]
    public List<string>? MultipleClientValues { get; set; }

    [Input(Type="combobox", EvalAllowableValues = "['Alpha','Bravo','Charlie']")]
    public string? SingleServerValues { get; set; }

    [Input(Type="combobox", EvalAllowableValues = "AppData.AlphaValues", Multiple = true)]
    public List<string>? MultipleServerValues { get; set; }

    [Input(Type="combobox", EvalAllowableEntries = "{ A:'Alpha', B:'Bravo', C:'Charlie' }")]
    public string? SingleServerEntries { get; set; }

    [Input(Type="combobox", EvalAllowableEntries = "AppData.AlphaDictionary", Multiple = true)]
    public List<string>? MultipleServerEntries { get; set; }
}
```

Which can then be rendered with:

```html
<AutoForm type="ComboBoxExamples" />
```
<auto-form type="ComboBoxExamples" class="not-prose mb-4"></auto-form>

**Combobox Options**

Each property shows a different way of populating the Combobox's optional values, they can be populated from a JavaScript
Object literal using `Options` or on the server with a [#Script Expression](https://sharpscript.net) where they can be 
populated from a static list or from a C# class as seen in the examples referencing `AppData` properties:

```csharp
public class AppData
{
    public List<string> AlphaValues { get; set; }
    public Dictionary<string, string> AlphaDictionary { get; set; }
    public List<KeyValuePair<string, string>> AlphaKeyValuePairs { get; set; }
}
```

Which are populated on in the AppHost on Startup with:

```csharp
ScriptContext.Args[nameof(AppData)] = new AppData
{
    AlphaValues = new() {
        "Alpha", "Bravo", "Charlie"
    },
    AlphaDictionary = new()
    {
        ["A"] = "Alpha",
        ["B"] = "Bravo",
        ["C"] = "Charlie",
    },
    AlphaKeyValuePairs = new()
    {
        new("A","Alpha"),
        new("B","Bravo"),
        new("C","Charlie"),
    },
};
```

Which can alternatively be populated from a dynamic source like an RDBMS table.

As C# Dictionaries have an undetermined sort order, you can use a `List<KeyValuePair<string, string>>` instead when you need to
display an ordered list of Key/Value pairs.