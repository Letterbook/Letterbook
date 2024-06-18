---
title: JSON API Client Features
group: Library
---

[useClient()](https://github.com/ServiceStack/servicestack-vue/blob/main/src/api.ts) provides managed APIs around the `JsonServiceClient` 
instance registered in Vue App's with:

```js
app.provide('client', client)
```

Which maintains contextual information around your API calls like **loading** and **error** states, used by `@servicestack/vue` components to 
enable its auto validation binding. Other functionality in this provider include:

```js
let { 
    api,            // Send a typed API request and return results in an ApiResult<TResponse>
    apiVoid,        // Send a typed API request and return empty response in a void ApiResult
    apiForm,        // Send a FormData API request and return results in an ApiResult<TResponse>
    apiFormVoid,    // Send a FormData API request and return empty response in a void ApiResult
    loading,        // Maintain loading state whilst API Request is in transit
    error,          // Maintain API Error response in reactive Ref<ResponseStatus>
    setError,       // Set API error state with summary or field validation error
    addFieldError,  // Add field error to API error state
    unRefs          // Returns a dto with all Refs unwrapped
} = useClient()
```

Typically you would need to unwrap `ref` values when calling APIs, i.e:

```js
let client = new JsonServiceClient()
let api = await client.api(new Hello({ name:name.value }))
```

### api

This is unnecessary in useClient `api*` methods which automatically unwraps ref values, allowing for the more pleasant API call:

```js
let api = await client.api(new Hello({ name }))
```

### unRefs

But as DTOs are typed, passing reference values will report a type annotation warning in IDEs with type-checking enabled, 
which can be avoided by explicitly unwrapping DTO ref values with `unRefs`:

```js
let api = await client.api(new Hello(unRefs({ name })))
```

### setError

`setError` can be used to populate client-side validation errors which the 
[SignUp.mjs](https://github.com/NetCoreTemplates/vue-mjs/blob/main/Letterbook.Docs/wwwroot/Pages/SignUp.mjs)
component uses to report an invalid submissions when passwords don't match:

```js
const { api, setError } = useClient()
async function onSubmit() {
    if (password.value !== confirmPassword.value) {
        setError({ fieldName:'confirmPassword', message:'Passwords do not match' })
        return
    }
    //...
}
```

## Form Validation

All `@servicestack/vue` Input Components support contextual validation binding that's typically populated from API
[Error Response DTOs](/error-handling) but can also be populated from client-side validation
as done above.

### Explicit Error Handling

This populated `ResponseStatus` DTO can either be manually passed into each component's **status** property as done in [/Todos](https://vue-mjs.web-templates.io/TodoMvc):

```html
<template id="TodoMvc-template">
    <div class="mb-3">
        <text-input :status="store.error" id="text" label="" placeholder="What needs to be done?"
                    v-model="store.newTodo" v-on:keyup.enter.stop="store.addTodo()"></text-input>
    </div>
    <!-- ... -->
</template>
```

Where if you try adding an empty Todo the `CreateTodo` API will fail and populate its `store.error` reactive property with the 
APIs Error Response DTO which the `<TextInput />` component checks for to display any field validation errors matching the
field in `id` adjacent to the HTML Input:

```js
let store = {
    /** @type {Todo[]} */
    todos: [],
    newTodo:'',
    error:null,
    async refreshTodos(errorStatus) {
        this.error = errorStatus
        let api = await client.api(new QueryTodos())
        if (api.succeeded)
            this.todos = api.response.results
    },
    async addTodo() {
        this.todos.push(new Todo({ text:this.newTodo }))
        let api = await client.api(new CreateTodo({ text:this.newTodo }))
        if (api.succeeded)
            this.newTodo = ''
        return this.refreshTodos(api.error)
    },
    //...
}
```

### Implicit Error Handling

More often you'll want to take advantage of the implicit validation support in `useClient()` which makes its state available to child
components, alleviating the need to explicitly pass it in each component as seen in razor-tailwind's 
[Contacts.mjs](https://github.com/NetCoreTemplates/razor-tailwind/blob/main/Letterbook.Docs/wwwroot/Pages/Contacts.mjs) `Edit` component for its 
[/Contacts](https://vue-mjs.web-templates.io/Contacts) page which doesn't do any manual error handling:

```js
const Edit = {
    template:/*html*/`<SlideOver @done="close" title="Edit Contact">
    <form @submit.prevent="submit">
      <input type="submit" class="hidden">
      <fieldset>
        <ErrorSummary except="title,name,color,filmGenres,age,agree" class="mb-4" />
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
      </fieldset>
    </form>
    <template #footer>
      <div class="flex justify-between space-x-3">
        <div><ConfirmDelete @delete="onDelete">Delete</ConfirmDelete></div>
        <div><PrimaryButton @click="submit">Update Contact</PrimaryButton></div>
      </div>
    </template>
  </SlideOver>`,
    props:['contact'],
    emits:['done'],
    setup(props, { emit }) {
        const client = useClient()
        const request = ref(new UpdateContact(props.contact))
        const colorOptions = propertyOptions(getProperty('UpdateContact','Color'))

        async function submit() {
            const api = await client.api(request.value)
            if (api.succeeded) close()
        }
        
        async function onDelete () {
            const api = await client.apiVoid(new DeleteContact({ id:props.id }))
            if (api.succeeded) close()
        }

        const close = () => emit('done')
        
        return { request, enumOptions, colorOptions, submit, onDelete, close }
    }
}
```

This effectively makes form validation binding a transparent detail where all `@servicestack/vue` 
Input Components are able to automatically apply contextual validation errors next to the fields they apply to: 

<div class="my-8">
  <img class="mx-auto max-w-2xl" src="https://servicestack.net/img/pages/scripts/edit-contact-validation.png">
</div>

## Example using apiForm

An alternative method of invoking APIs is to submit a HTML Form Post which can be achieved with Ajax by sending a populated `FormData` 
with `client.apiForm()` as done in vue-mjs's 
[SignUp.mjs](https://github.com/NetCoreTemplates/vue-mjs/blob/main/Letterbook.Docs/wwwroot/Pages/SignUp.mjs) for its 
[/signup](https://vue-mjs.web-templates.io/signup) page:

```js
import { ref } from "vue"
import { leftPart, rightPart, toPascalCase } from "@servicestack/client"
import { useClient } from "@servicestack/vue"
import { Register } from "../mjs/dtos.mjs"

export default {
    template:/*html*/`    
    <form @submit.prevent="submit">
      <div class="shadow overflow-hidden sm:rounded-md">
        <ErrorSummary except="displayName,userName,password,confirmPassword,autoLogin" />
        <div class="px-4 py-5 bg-white dark:bg-black space-y-6 sm:p-6">
          <div class="flex flex-col gap-y-4">
            <TextInput id="displayName" help="Your first and last name" v-model="request.displayName" />
            <TextInput id="userName" label="Email" placeholder="Email" help="" v-model="request.userName" />
            <TextInput id="password" type="password" help="6 characters max" v-model="request.password "/>
            <TextInput id="confirmPassword" type="password" v-model="request.confirmPassword" />
            <CheckboxInput id="autoLogin" v-model="request.autoLogin" />
          </div>
        </div>
        <div class="pt-5 px-4 py-3 bg-gray-50 dark:bg-gray-900 text-right sm:px-6">
          <div class="flex justify-end">
            <FormLoading v-if="loading" class="flex-1" />
            <PrimaryButton :disabled="loading" class="ml-3">Sign Up</PrimaryButton>
          </div>
        </div>
      </div>
    </form>`,
    props: { returnUrl:String },
    setup(props) {
        const client = useClient()
        const { setError, loading } = client
        const request = ref(new Register({ autoLogin:true }))

        /** @param email {string} */
        function setUser(email) {
            let first = leftPart(email, '@')
            let last = rightPart(leftPart(email, '.'), '@')
            const dto = request.value
            dto.displayName = toPascalCase(first) + ' ' + toPascalCase(last)
            dto.userName = email
            dto.confirmPassword = dto.password = 'p@55wOrd'
        }
        
        /** @param {Event} e */
        async function submit(e) {
            if (request.value.password !== request.value.confirmPassword) {
                setError({ fieldName: 'confirmPassword', message: 'Passwords do not match' })
                return
            }
            
            // Example using client.apiForm()
            const api = await client.apiForm(new Register(), new FormData(e.target))
            if (api.succeeded) {
                location.href = props.returnUrl || '/signin'
            }
        }
        
        return { loading, request, setUser, submit }
    }
}
```

Which method to use is largely a matter of preference except if your form needs to upload a file in which case using `apiForm` is required.

## AutoForm Components

We can elevate our productivity even further with [Auto Form Components](/vue/autoform) that can automatically generate an
instant API-enabled form with validation binding by just specifying the Request DTO to create the form for, e.g:

```html
<AutoCreateForm type="CreateBooking" formStyle="card" />
```

<div class="mb-4 not-prose">
<auto-create-form type="CreateBooking" form-style="card"></auto-create-form>
</div>

The AutoForm components are powered by your [App Metadata](/vue/use-metadata) which allows creating 
highly customized UIs from [declarative C# attributes](/locode/declarative) whose customizations are
reused across all ServiceStack Auto UIs.

## TypeScript Definition

TypeScript definition of the API surface area and type information for correct usage of `useClient()`

```ts
/** Maintain loading state whilst API Request is in transit */
const loading: Ref<boolean>

/** Maintain API Error in reactive Ref<ResponseStatus> */
const error: Ref<ResponseStatus>

/** Set error state with summary or field validation error */
function setError({ message, errorCode, fieldName, errors }: IResponseStatus);

/** Add field error to API error state */
function addFieldError({ fieldName, message, errorCode }: IResponseError);

/** Send a typed API request and return results in an ApiResult<TResponse> */
async function api<TResponse>(request:IReturn<TResponse> | ApiRequest, args?:any, method?:string);

/** Send a typed API request and return empty response in a void ApiResult */
async function apiVoid(request:IReturnVoid | ApiRequest, args?:any, method?:string);

/** Send a FormData API request and return results in an ApiResult<TResponse> */
async function apiForm<TResponse>(request:IReturn<TResponse> | ApiRequest, body:FormData, args?:any, method?:string);

/** Send a FormData API request and return empty response in a void ApiResult */
async function apiFormVoid(request: IReturnVoid | ApiRequest, body: FormData, args?: any, method?: string);
```
