---
title: Formatting Functions and Methods
group: Library
---

### Using Formatters

Your App and components can also utilize the built-in formatting functions in `useFormatters()`:

```js
import { useFormatters } from '@servicestack/vue'

const {
    Formats,              // Available format methods to use in <PreviewFormat />
    formatValue,          // Format any value or object graph
    currency,             // Format number as Currency
    bytes,                // Format number in human readable disk size
    link,                 // Format URL as <a> link
    linkTel,              // Format Phone Number as <a> tel: link
    linkMailTo,           // Format email as <a> mailto: link
    icon,                 // Format Image URL as an Icon
    iconRounded,          // Format Image URL as a full rounded Icon
    attachment,           // Format File attachment URL as an Attachment
    hidden,               // Format as empty string
    time,                 // Format duration in time format
    relativeTime,         // Format Date as Relative Time from now
    relativeTimeFromMs,   // Format time in ms as Relative Time from now
    relativeTimeFromDate, // Format difference between dates as Relative Time
    formatDate,           // Format as Date
    formatNumber,         // Format as Number

    setDefaultFormats,    // Set default locale, number and Date formats
    setFormatters,        // Register additional formatters for use in <PreviewFormat />
    indentJson,           // Prettify an API JSON Response
    truncate,             // Truncate text that exceeds maxLength with an ellipsis
    apiValueFmt,          // Format an API Response value
} = useFormatters()
```

Many of these formatting functions return rich HTML markup which will need to be rendered using Vue's **v-html** directive:

```html
<span v-html="formatValue(value)"></span>
```

See the [PreviewFormat](/vue/formats) for examples for what each of these format functions render to. 

<api-reference component="setDefaultFormats"></api-reference>
## Set global default formats

Global default formats can be customized with `setDefaultFormats`:

```js
setDefaultFormats({
    locale: null,     // Use Browsers default local
    assumeUtc: true,
    number: null,     // Use locale Number format
    date: {
        method: "Intl.DateTimeFormat",
        options: "{dateStyle:'medium'}"
    },
    maxFieldLength: 150,
    maxNestedFields: 150,
    maxNestedFieldLength: 150,
})
```

<api-reference component="setFormatters"></api-reference>
## Register custom formatters

Use `setFormatters` to register new formatters that you want to use in `[Format("method")]` or 
within `<PreviewFormat/>` components, e.g. you could register a formatter that renders a QR Code image of the content with:

```ts
import { QRCode } from "qrcode-svg"

function qrcode(content) {
    return new QRCode({ content, padding:4, width:256, height:256 }).svg()
}

setFormatters({
    qrcode,
})
```

Where it will be able to be used within format components, e.g:

```html
<PreviewFormat :value="url" :format="{ method:'qrcode' }" />
```

That can also be used to decorate properties in C# DTOs with the [Format Attribute](/locode/formatters), e.g:

```csharp
[Format("qrcode")]
public string Code { get; set; }
```

## Overriding built-in formatters

`setFormatters` can also be to override the built-in formatting functions by registering alternative implementations for:

```ts
setFormatters({
    currency,
    bytes,
    link,
    linkTel,
    linkMailTo,
    icon,
    iconRounded,
    attachment,
    hidden,
    time,
    relativeTime,
    relativeTimeFromMs,
    formatDate,
    formatNumber,
})
```

## TypeScript Definition

TypeScript definition of the API surface area and type information for correct usage of `useFormatters()`

```ts
class Formats {
    static currency: FormatInfo;
    static bytes: FormatInfo;
    static link: FormatInfo;
    static linkTel: FormatInfo;
    static linkMailTo: FormatInfo;
    static icon: FormatInfo;
    static iconRounded: FormatInfo;
    static attachment: FormatInfo;
    static time: FormatInfo;
    static relativeTime: FormatInfo;
    static relativeTimeFromMs: FormatInfo;
    static date: FormatInfo;
    static number: FormatInfo;
    static hidden: FormatInfo;
}

/** Format any value or object graph */
function formatValue(value: any, format?: FormatInfo | null, attrs?: any): any;

/** Format number as Currency */
function currency(val: number, attrs?: any): string;

/** Format number in human readable disk size */
function bytes(val: number, attrs?: any): string;


/** Format URL as <a> link */
function link(href: string, opt?: {
    cls?: string;
    target?: string;
    rel?: string;
}): string;

/** Format email as <a> mailto: link */
function linkMailTo(email: string, opt?: {
    subject?: string;
    body?: string;
    cls?: string;
    target?: string;
    rel?: string;
}): string;

/** Format Phone Number as <a> tel: link */
function linkTel(tel: string, opt?: {
    cls?: string;
    target?: string;
    rel?: string;
}): string;

/** Format Image URL as an Icon */
function icon(url: string, attrs?: any): string;

/** Format Image URL as a full rounded Icon */
function iconRounded(url: string, attrs?: any): string;

/** Format File attachment URL as an Attachment */
function attachment(url: string, attrs?: any): string;

/** Format as empty string */
function hidden(o: any): string;

/** Format duration in time format */
function time(o: any, attrs?: any): string;

/** Format Date as Relative Time from now */
function relativeTime(val: string | Date | number, rtf?: Intl.RelativeTimeFormat): string | undefined;

/** Format difference between dates as Relative Time */
function relativeTimeFromDate(d: Date, from?: Date): string | undefined;

/** Format time in ms as Relative Time from now */
function relativeTimeFromMs(elapsedMs: number, rtf?: Intl.RelativeTimeFormat): string | undefined;

/** Format as Date */
function formatDate(d: Date | string | number, attrs?: any): string;

/** Format as Number */
function formatNumber(n: number, attrs?: any): string;

/** Set default locale, number and Date formats */
function setDefaultFormats(newFormat: DefaultFormats): void;

/** Register additional formatters for use in <PreviewFormat /> */
function setFormatters(formatters: {
    [name: string]: Function;
}): void;

/** Prettify an API JSON Response */
function indentJson(o: any): string;

/** Truncate text that exceeds maxLength with an ellipsis */
function truncate(str: string, maxLength: number): string;

/** Format an API Response value */
function apiValueFmt(o: any, format?: FormatInfo | null, attrs?: any): any;
```
