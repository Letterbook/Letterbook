---
title: Alert Components
group: Component Gallery
---

<api-reference component="Alert"></api-reference>
## Alert

<p class="mb-4 text-lg">
    Show basic alert message:
</p>

```html
<Alert>Default <b>Message</b></Alert>
<Alert type="info">Information <b>Message</b></Alert>
<Alert type="success">Success <b>Message</b></Alert>
<Alert type="warn">Warning <b>Message</b></Alert>
<Alert type="error">Error <b>Message</b></Alert>
```

<div class="not-prose mb-4">
<alert>Default <b>Message</b></alert>
<alert type="info">Information <b>Message</b></alert>
<alert type="success">Success <b>Message</b></alert>
<alert type="warn">Warning <b>Message</b></alert>
<alert type="error">Error <b>Message</b></alert>
</div>

Show alert message from dynamic HTML string:

```html
<Alert v-html="message" />

<script>
const message = "Requires <b>Employee</b> Role"
</script>
```

<div class="not-prose">
<alert v-html="message"></alert>
</div>

<api-reference component="AlertSuccess"></api-reference>
## Alert Success

Show success alert message:

```html
<AlertSuccess>Order was received</AlertSuccess>
```

<div class="not-prose">
<alert-success class="not-prose">Order was received</alert-success>
</div>

<api-reference component="ErrorSummary"></api-reference>
## Error Summary

Show failed Summary API Error Message:

```html
<ErrorSummary :status="{ message:'Requires Employee Role' }" />
```

<div class="not-prose">
<error-summary :status="{ message:'Requires Employee Role' }" class="not-prose"></error-summary>
</div>