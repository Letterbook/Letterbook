---
title: Format Examples
group: Component Gallery
---

<api-reference component="PreviewFormat"></api-reference>
## PreviewFormat

Useful for rendering Table Cell data into different customizable formats, e.g:

### Currency

```html
<PreviewFormat :value="50" :format="Formats.currency" />
```
<div class="not-prose">
<preview-format :value="50" :format="Formats.currency"></preview-format>
</div>

### Bytes

```html
<PreviewFormat :value="10000000" :format="Formats.bytes" />
```
<div class="not-prose">
<preview-format :value="10000000" :format="Formats.bytes"></preview-format>
</div>

### Icon

```html
<PreviewFormat value="/pages/vue/1.jpg" :format="Formats.icon" />
```
<div class="not-prose">
<preview-format value="/pages/vue/1.jpg" :format="Formats.icon"></preview-format>
</div>

### Icon Rounded

```html
<PreviewFormat value="/pages/vue/1.jpg" :format="Formats.iconRounded" />
```
<div class="not-prose">
<preview-format value="/pages/vue/1.jpg" :format="Formats.iconRounded"></preview-format>
</div>

### Icon with custom class

```html
<PreviewFormat value="/pages/vue/1.jpg" :format="Formats.icon" class="w-40 h-40 rounded-full" />
```
<div class="not-prose">
<preview-format value="/pages/vue/1.jpg" :format="Formats.icon" class="not-prose w-40 h-40 rounded-full"></preview-format>
</div>

### Attachment (Image)

```html
<PreviewFormat value="/pages/vue/1.jpg" :format="Formats.attachment" />
```
<div class="not-prose">
<preview-format value="/pages/vue/1.jpg" :format="Formats.attachment"></preview-format>
</div>

### Attachment (Document)

```html
<PreviewFormat value="/content/hosting.md" :format="Formats.attachment" />
```
<div class="not-prose">
<preview-format value="/content/hosting.md" :format="Formats.attachment"></preview-format>
</div>

### Attachment (Document) with classes

```html
<PreviewFormat value="/content/hosting.md" :format="Formats.attachment" 
    class="text-xl text-indigo-700 font-semibold" icon-class="w-8 h-8" />
```
<div class="not-prose">
<preview-format value="/content/hosting.md" :format="Formats.attachment" class="text-xl text-indigo-700 font-semibold" icon-class="w-8 h-8"></preview-format>
</div>

### Link

```html
<PreviewFormat value="https://servicestack.net/blazor" :format="Formats.link" />
```
<div class="not-prose">
<preview-format value="https://servicestack.net/blazor" :format="Formats.link"></preview-format>
</div>

### Link with class

```html
<PreviewFormat value="https://servicestack.net/blazor" :format="Formats.link" class="text-xl" />
```
<div class="not-prose">
<preview-format value="https://servicestack.net/blazor" :format="Formats.link" class="text-xl text-blue-600"></preview-format>
</div>

### Link Email

```html
<PreviewFormat value="user@email.com" :format="Formats.linkMailTo" />
```
<div class="not-prose">
<preview-format value="user@email.com" :format="Formats.linkMailTo"></preview-format>
</div>

### Link Phone

```html
<PreviewFormat value="555 123 4567" :format="Formats.linkTel" />
```
<div class="not-prose">
<preview-format value="555 123 4567" :format="Formats.linkTel"></preview-format>
</div>

## Using Formatters

Your App and custom templates can also utilize @servicestack/vue's [built-in formatting functions](/vue/use-formatters) from:

```js
import { useFormatters } from '@servicestack/vue'

const {
    Formats,             // Available format methods to use in <PreviewFormat />
    formatValue,         // Format any value or object graph
    currency,            // Format number as Currency
    bytes,               // Format number in human readable disk size
    link,                // Format URL as <a> link
    linkTel,             // Format Phone Number as <a> tel: link
    linkMailTo,          // Format email as <a> mailto: link
    icon,                // Format Image URL as an Icon
    iconRounded,         // Format Image URL as a full rounded Icon
    attachment,          // Format File attachment URL as an Attachment
    hidden,              // Format as empty string
    time,                // Format duration in time format
    relativeTime,        // Format Date as Relative Time from now
    relativeTimeFromMs,  // Format time in ms as Relative Time from now
    formatDate,          // Format as Date
    formatNumber,        // Format as Number
} = useFormatters()
```

Many of these formatting functions return rich HTML markup which will need to be rendered using Vue's **v-html** directive:

```html
<span v-html="formatValue(value)"></span>
```

<api-reference component="HtmlFormat"></api-reference>
## HtmlFormat

`HtmlFormat` can be used to render any Serializable object into a human-friendly HTML Format:

### Single Model

```html
<HtmlFormat :value="tracks[0]" />
```
<div class="not-prose max-w-screen-sm">
    <html-format :value="tracks[0]"></html-format>
</div>

### Item Collections

```html
<HtmlFormat :value="tracks" />
```
<div class="not-prose max-w-screen-sm">
    <html-format :value="tracks"></html-format>
</div>

### Nested Complex Types

```html
<HtmlFormat :value="players" />
```
<div class="not-prose prose-table">
<html-format :value="players"></html-format>
</div>

### Nested Complex Types with custom classes

When needed most default classes can be overridden with a custom **classes** function that can inspect the
type, tag, depth, and row index to return a custom class. The TypeScript function shows an example of checking
these different parameters to render a custom HTML resultset:

```html
<HtmlFormat :value="players" :classes="classes" />

<script lang="ts">
function classes(type:'array'|'object', tag:'div'|'table'|'thead'|'th'|'tr'|'td',depth:number,cls:string,index?:number)
{
    if (type === 'array') {
        if (tag === 'th') return cls.replace('text-sm text-gray-500 font-medium',' ') + (depth === 0 
            ? 'text-xs uppercase font-semibold text-indigo-700'
            : 'text-xs font-medium text-gray-500')
        if (tag === 'tr') return depth > 0 || index! % 2 == 0 ? 'bg-white' : 'bg-yellow-50'
        if (tag === 'td' && depth > 0) return 'px-4 py-3 whitespace-nowrap text-xs'
    }
    return cls
}
</script>
```
<div class="not-prose prose-table">
<html-format :value="players" :classes="classes"></html-format>
</div>