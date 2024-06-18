<h2 id="formatters" class="mt-8 mb-4 text-2xl font-semibold text-gray-900 dark:text-gray-100">
    Using Formatters
</h2>

Your App and custom templates can also utilize @servicestack/vue's <TextLink href="/vue/use-formatters">built-in formatting functions</TextLink> from:

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
