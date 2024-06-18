---
title: DataGrid Component
group: Component Gallery
---

<api-reference component="DataGrid<Model>"></api-reference>
## Default

In its most simple usage the DataGrid component can be used to render typed collections:

```html
<DataGrid :items="tracks" />

<script>
const tracks = [
    track("Everythings Ruined", "Faith No More", "Angel Dust", 1992),
    track("Lightning Crashes", "Live", "Throwing Copper", 1994),
    track("Heart-Shaped Box", "Nirvana", "In Utero", 1993),
    track("Alive", "Pearl Jam", "Ten", 1991),
]
</script>
```

Which by default will display all object properties:

<div class="not-prose">
<data-grid :items="tracks" class="mb-4"></data-grid>
</div>

Use **selected-columns** to control which columns to display and **header-titles** to use different column names:

```html
<DataGrid :items="tracks" :selected-columns="['year','album','name','artist']" 
          :header-titles="{ name:'Track' }" />
```
<div class="not-prose">
<data-grid :items="tracks" :selected-columns="['year','album','name','artist']" :header-titles="{ name:'Track' }" class="mb-4"></data-grid>
</div>

Which for a wrist-friendly alternative also supports a string of comma delimited column names, e.g:

```html
<DataGrid :items="tracks" selected-columns="year,album,name,artist" />
```

## Simple Customizations

Which columns are shown and how they're rendered is customizable with custom `<template #column>` definitions:

```html
<DataGrid :items="forecasts" class="max-w-screen-md" :table-style="['stripedRows','uppercaseHeadings']"
          :header-titles="{ temperatureC:'TEMP. (C)', temperatureF:'TEMP. (F)' }">
    <template #date-header>
        <span class="text-indigo-600">Date</span>
    </template>
    <template #date="{ date }">
        {{ new Intl.DateTimeFormat().format(new Date(date)) }}
    </template>
    <template #temperatureC="{ temperatureC }">
        {{ temperatureC }}&deg;
    </template>
    <template #temperatureF="{ temperatureF }">
        {{ temperatureF }}&deg;
    </template>
    <template #summary="{ summary }">{{ summary }}</template>
</DataGrid>
```

<custom class="not-prose mb-4"></custom>

Column names can be changed with a **header-titles** alias mapping, or dynamically with a **header-title** mapping function.

Alternatively for more advanced customizations, custom `<template #column-header>` definitions can be used 
to control how column headers are rendered.

If any custom column or header definitions are provided, only those columns will be displayed. 
Alternatively specify an explicit array of column names in **selected-columns**
to control the number and order or columns displayed.

## Responsive

A more advanced example showing how to implement a responsive datagrid defining what columns and Headers
are visible at different screen sizes using **visible-from** to specify which columns to show 
from different Tailwind responsive breakpoints and `<template #column-header>` definitions to 
collapse column names at small screen sizes:

```html
<template>
<DataGrid :items="bookings" 
      :visible-from="{ name:'xl', bookingStartDate:'sm', bookingEndDate:'xl' }"
      @header-selected="headerSelected"
      @row-selected="rowSelected" :is-selected="row => selected == row.id">
    <template #id="{ id }">
        <span class="text-gray-900" v-html="id"></span>
    </template>
    
    <template #name="{ name }" v-html="name"></template>
    
    <template #roomNumber-header>
        <span class="hidden lg:inline">Room </span>No
    </template>

    <template #cost="{ cost }" v-html="currency(cost)"></template>
    
    <template #bookingStartDate-header>
        Start<span class="hidden lg:inline"> Date</span>
    </template>
    
    <template #bookingEndDate-header>
        End<span class="hidden lg:inline"> Date</span>
    </template>

    <template #createdBy-header>
        Employee
    </template>
    <template #createdBy="{ createdBy }" v-html="createdBy"></template>
</DataGrid>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useFormatters } from '@servicestack/vue'
import { bookings } from '../data'
import { Booking } from '../dtos'

const { currency } = useFormatters()
const selected = ref()

function headerSelected(column:string) {
    console.log('headerSelected',column)
}
function rowSelected(row:Booking) {
    selected.value = selected.value === row.id ? null : row.id
    console.log('rowSelected', row)
}
</script>
```

<responsive class="not-prose mb-4"></responsive>

Behavior of the DataGrid can be customized with the `@header-selected` event to handle when column headers are selected to 
apply custom filtering to the **items** data source whilst the `@row-selected` event can be used to apply custom behavior 
when a row is selected.

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

The [PreviewFormat](/vue/formats) component also offers a variety of flexible formatting options.

## Table Styles

The appearance of DataGrids can use **tableStyles** to change to different
[Tailwind Table Styles](https://tailwindui.com/components/application-ui/lists/tables), e.g:

### Default (Striped Rows)

```html
<DataGrid :items="tracks" />
```

<div class="not-prose">
<data-grid :items="tracks"></data-grid>
</div>

### Simple

```html
<DataGrid :items="tracks" table-style="simple" />
```

<div class="not-prose">
<data-grid :items="tracks" table-style="simple"></data-grid>
</div>

### Uppercase Headings

```html
<DataGrid :items="tracks" table-style="uppercaseHeadings" />
```

<div class="not-prose">
<data-grid :items="tracks" table-style="uppercaseHeadings"></data-grid>
</div>

### Vertical Lines

```html
<DataGrid :items="tracks" table-style="verticalLines" />
```

<div class="not-prose">
<data-grid :items="tracks" table-style="verticalLines"></data-grid>
</div>

### White Background

```html
<DataGrid :items="tracks" table-style="whiteBackground" />
```

<div class="not-prose">
<data-grid :items="tracks" table-style="whiteBackground"></data-grid>
</div>

### Full Width

```html
<DataGrid :items="tracks" table-style="fullWidth" />
```

<div class="not-prose">
<data-grid :items="tracks" table-style="fullWidth"></data-grid>
</div>

### Full Width, Uppercase with Vertical Lines

```html
<DataGrid :items="tracks" :table-style="['uppercaseHeadings', 'fullWidth', 'verticalLines']" />
```

<div class="not-prose">
<data-grid :items="tracks" :table-style="['uppercaseHeadings', 'fullWidth', 'verticalLines']"></data-grid>
</div>

## Using App Metadata

By default DataGrid will render values using its default configured formatters, so results with strings, numbers and defaults
will display a stock standard resultset:

```html
<DataGrid :items="bookings" />
```
<div class="not-prose prose-table">
<data-grid :items="bookings" class="mb-4"></data-grid>
</div>

Another option for formatting this dataset is to use the rich [format functions](/locode/formatters) in ServiceStack
to annotate the DTOs with how each field should be formatted, e.g:

```csharp
public class Booking
{
    [AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public RoomType RoomType { get; set; }
    public int RoomNumber { get; set; }

    [IntlDateTime(DateStyle.Long)]
    public DateTime BookingStartDate { get; set; }

    [IntlRelativeTime]
    public DateTime? BookingEndDate { get; set; }

    [IntlNumber(Currency = NumberCurrency.USD)]
    public decimal Cost { get; set; }
}
```

Which can be enabled when using [useMetadata](/vue/use-metadata) by specifying the `MetadataType` for the DataGrid's results in **type**:

```html
<DataGrid :items="bookings" type="Booking" />
```

<div class="not-prose prose-table">
<data-grid :items="bookings" type="Booking" class="mb-4"></data-grid>
</div>

Declaratively annotating your DTOs with preferred formatting hints makes this rich metadata information available to clients where
it's used to enhance ServiceStack's built-in UI's and Components like:

 - [API Explorer](/api-explorer)
 - [Locode](https://docs.servicestack.net/locode/)
 - [Blazor Tailwind Components](/templates/blazor-components)

