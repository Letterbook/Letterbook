---
title: Auth Features
group: Library
---

Vue.js Apps can access Authenticated Users using [useAuth()](/vue/use-auth)
which can also be populated without the overhead of an Ajax request by embedding the response of the built-in
[Authenticate API](https://vue-mjs.web-templates.io/ui/Authenticate?tab=details) inside `_Layout.cshtml` with:

```html
<script type="module">
import { useAuth } from "@@servicestack/vue"
const { signIn } = useAuth()
signIn(@await Html.ApiAsJsonAsync(new Authenticate()))
</script>
```

Where it enables access to the below [useAuth()](/vue/use-auth) utils for inspecting the 
current authenticated user:  

```js
const { 
    signIn,           // Sign In the currently Authenticated User
    signOut,          // Sign Out currently Authenticated User
    user,             // Access Authenticated User info in a reactive Ref<AuthenticateResponse>
    isAuthenticated,  // Check if the current user is Authenticated in a reactive Ref<boolean>
    hasRole,          // Check if the Authenticated User has a specific role
    hasPermission,    // Check if the Authenticated User has a specific permission
    isAdmin           // Check if the Authenticated User has the Admin role
} = useAuth()
```

An example where this is used is in 
[Bookings.mjs](https://github.com/NetCoreTemplates/vue-mjs/blob/main/Letterbook.Docs/wwwroot/Pages/Bookings.mjs)
to control whether the `<AutoEditForm>` component should enable its delete functionality:

```js
export default {
    template/*html*/:`
    <AutoEditForm type="UpdateBooking" :deleteType="canDelete ? 'DeleteBooking' : null" />
    `,
    setup(props) {
        const { hasRole } = useAuth()
        const canDelete = computed(() => hasRole('Manager'))
        return { canDelete }
    }
}
```

## TypeScript Definition

TypeScript definition of the API surface area and type information for correct usage of `useAuth()`

```ts
/** Access the currently Authenticated User info in a reactive Ref<AuthenticateResponse> */
const user: Ref<AuthenticateResponse>

/** Check if the current user is Authenticated in a reactive Ref<boolean> */
const isAuthenticated: Ref<boolean>

/** Sign In the currently Authenticated User */
function signIn(user:AuthenticateResponse): void;

/** Sign Out currently Authenticated User */
function signOut(): void;

/** Check if the Authenticated User has a specific role */
function hasRole(role:string): boolean;

/** Check if the Authenticated User has a specific permission */
function hasPermission(permission:string): boolean;

/** Check if the Authenticated User has the Admin role */
function isAdmin(): boolean;
```
