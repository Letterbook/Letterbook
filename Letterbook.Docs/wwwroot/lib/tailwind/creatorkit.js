import { computed, ref, inject } from "vue"
import { useClient } from "@servicestack/vue"
import { appendQueryString, combinePaths } from "@servicestack/client"

export const SignInDialog = {
    template:/*html*/`<ModalDialog @done="done" sizeClass="sm:max-w-prose sm:w-full">
    <div class="min-h-full flex flex-col justify-center py-12 sm:px-6 lg:px-8">
        <div class="sm:mx-auto sm:w-full sm:max-w-md">
            <h2 class="text-center text-3xl font-extrabold text-gray-900 dark:text-gray-50">
                Sign In
            </h2>
        </div>
        <div class="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
            <ErrorSummary class="mb-3" except="userName,password" />
            <form @submit.prevent="submit">
                <div class="flex flex-col gap-y-4">
                    <TextInput id="userName" label="Email" help="Email you signed up with" v-model="request.userName" placeholder="" />
                    <TextInput id="password" type="password" help="6 characters or more" v-model="request.password" placeholder="" />
                </div>
                <div class="mt-8">
                    <PrimaryButton class="w-full mb-4">Sign In</PrimaryButton>
                </div>
            </form>
    
            <div v-if="oauthProviders.length" class="mt-6">
                <div class="relative">
                    <div class="absolute inset-0 flex items-center">
                        <div class="w-full border-t border-gray-600"></div>
                    </div>
                    <div class="relative flex justify-center text-sm">
                        <span class="px-2 bg-white dark:bg-black text-gray-500">
                            Or continue with
                        </span>
                    </div>
                </div>
                <div class="mt-6 grid grid-cols-3 gap-3">
                    <div v-for="provider in oauthProviders">
                        <a :href="providerUrl(provider)" :title="providerLabel(provider)" 
                            class="group w-full inline-flex justify-center py-2 px-4 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-gray-50 dark:bg-gray-900 hover:bg-gray-200 dark:hover:bg-gray-800 text-sm font-medium text-gray-500">
                            <Icon v-if="provider.icon" :image="provider.icon" class="h-5 w-5 text-gray-500 dark:text-gray-700 group-hover:text-black dark:group-hover:text-white" />
                            <svg v-else class="h-5 w-5 text-gray-300 dark:text-gray-700 hover:text-white" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32">
                                <path d="M16 8a5 5 0 1 0 5 5a5 5 0 0 0-5-5z" fill="currentColor"/>
                                <path d="M16 2a14 14 0 1 0 14 14A14.016 14.016 0 0 0 16 2zm7.992 22.926A5.002 5.002 0 0 0 19 20h-6a5.002 5.002 0 0 0-4.992 4.926a12 12 0 1 1 15.985 0z" fill="currentColor"/>
                            </svg>
                        </a>
                    </div>                    
                </div>
            </div>
        </div>
        <TextLink class="mt-4 block text-center" @click="$emit('signup')">Sign Up &rarr;</TextLink>
    </div> 
    </ModalDialog>`,
    emits:['done','signup'],
    setup(props, { emit }) {
        /** @type {Store} */
        const store = inject('store')
        const client = useClient()

        /** @type {Ref<Authenticate>} */
        const request = ref(new Authenticate({ provider:'credentials' }))

        const signInHref = computed(() =>
            appendQueryString(combinePaths(store.BaseUrl, 'auth/google'), { redirect:location.origin }))
        const errorSummary = computed(() => '')
        const oauthProviders = [
            {
                name:'Facebook',
                href:'/auth/facebook',
                icon: { svg: "<svg xmlns='http://www.w3.org/2000/svg' fill='currentColor' viewBox='0 0 20 20'><path fill-rule='evenodd' d='M20 10c0-5.523-4.477-10-10-10S0 4.477 0 10c0 4.991 3.657 9.128 8.438 9.878v-6.987h-2.54V10h2.54V7.797c0-2.506 1.492-3.89 3.777-3.89 1.094 0 2.238.195 2.238.195v2.46h-1.26c-1.243 0-1.63.771-1.63 1.562V10h2.773l-.443 2.89h-2.33v6.988C16.343 19.128 20 14.991 20 10z' clip-rule='evenodd' /></svg>" }
            },
            {
                name:'Google',
                href:'/auth/google',
                icon: { svg: "<svg xmlns='http://www.w3.org/2000/svg' fill='currentColor' viewBox='0 0 24 24'><path d='M11.99 13.9v-3.72h9.36c.14.63.25 1.22.25 2.05c0 5.71-3.83 9.77-9.6 9.77c-5.52 0-10-4.48-10-10S6.48 2 12 2c2.7 0 4.96.99 6.69 2.61l-2.84 2.76c-.72-.68-1.98-1.48-3.85-1.48c-3.31 0-6.01 2.75-6.01 6.12s2.7 6.12 6.01 6.12c3.83 0 5.24-2.65 5.5-4.22h-5.51v-.01z' fill='currentColor' /></svg>" }
            },
            {
                name:'Microsoft',
                href:'/auth/microsoftgraph',
                icon: { svg: "<svg xmlns='http://www.w3.org/2000/svg' fill='currentColor' viewBox='0 0 20 20'><path d='M11.55 21H3v-8.55h8.55V21zM21 21h-8.55v-8.55H21V21zm-9.45-9.45H3V3h8.55v8.55zm9.45 0h-8.55V3H21v8.55z' fill='currentColor'/></svg>" }
            },
        ]
        function providerUrl(provider) {
            return appendQueryString(combinePaths(store.BaseUrl, provider.href), { 'continue': location.href })
        }
        function providerLabel(provider) {
            return `Sign In with ${provider.name}`
        }
        function done() {
            emit('done')
        }
        async function submit() {
            const api = await client.api(request.value)
            if (api.succeeded) {
                await store.signIn(api.response)
                await store.loadUserData()
                done()
            }
        }

        return { request, signInHref, done, errorSummary, submit, oauthProviders, providerUrl, providerLabel, }
    }
}

export const SignUpDialog = {
    template:/*html*/`<ModalDialog @done="done" sizeClass="sm:max-w-prose sm:w-full">
    <div class="min-h-full flex flex-col justify-center py-12 sm:px-6 lg:px-8">
        <div class="sm:mx-auto sm:w-full sm:max-w-md">
            <h2 class="text-center text-3xl font-extrabold text-gray-900 dark:text-gray-50">
                Sign Up
            </h2>
        </div>
        <div class="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
            <ErrorSummary class="mb-3" except="userName,password" />
            <form @submit.prevent="submit">
                <div class="flex flex-col gap-y-4">
                    <TextInput id="email" v-model="request.email" placeholder="" />
                    <TextInput id="displayName" v-model="request.displayName" placeholder="" />
                    <TextInput id="password" type="password" help="6 characters or more" v-model="request.password" placeholder="" />
                    <TextInput id="confirmPassword" type="password" v-model="request.confirmPassword" placeholder="" />
                </div>
                <div class="mt-8">
                    <PrimaryButton class="w-full mb-4">Sign Up</PrimaryButton>
                </div>
            </form>
        </div>
        <TextLink class="block text-center pr-4" @click="$emit('signin')">&larr; Sign In</TextLink>
    </div> 
    </ModalDialog>`,
    emits:['done','signin'],
    setup(props, { emit }) {
        /** @type {Store} */
        const store = inject('store')

        /** @type {Ref<Register>} */
        const request = ref(new Register({ autoLogin:true }))
        const client = useClient()

        function done() {
            emit('done')
        }
        async function submit() {
            if (request.value.password !== request.value.confirmPassword) {
                client.setError({ fieldName:'confirmPassword', message:'Passwords do not match' })
                return
            }
            if (request.value.password.length < 6) {
                client.setError({ fieldName:'password', message:'Minimum of 6 characters' })
                return
            }
            const api = await client.api(request.value)
            if (api.succeeded) {
                await store.signIn(api.response)
                await store.loadUserData()
                done()
            }
        }
        return { request, submit, done }
    }
}

export class Authenticate {
    /** @param {{provider?:string,state?:string,oauth_token?:string,oauth_verifier?:string,userName?:string,password?:string,rememberMe?:boolean,errorView?:string,nonce?:string,uri?:string,response?:string,qop?:string,nc?:string,cnonce?:string,accessToken?:string,accessTokenSecret?:string,scope?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /**
     * @type {string}
     * @description AuthProvider, e.g. credentials */
    provider;
    /** @type {string} */
    state;
    /** @type {string} */
    oauth_token;
    /** @type {string} */
    oauth_verifier;
    /** @type {string} */
    userName;
    /** @type {string} */
    password;
    /** @type {?boolean} */
    rememberMe;
    /** @type {string} */
    errorView;
    /** @type {string} */
    nonce;
    /** @type {string} */
    uri;
    /** @type {string} */
    response;
    /** @type {string} */
    qop;
    /** @type {string} */
    nc;
    /** @type {string} */
    cnonce;
    /** @type {string} */
    accessToken;
    /** @type {string} */
    accessTokenSecret;
    /** @type {string} */
    scope;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'Authenticate' }
    getMethod() { return 'POST' }
    createResponse() { return new AuthenticateResponse() }
}
export class AuthenticateResponse {
    /** @param {{userId?:string,sessionId?:string,userName?:string,displayName?:string,referrerUrl?:string,bearerToken?:string,refreshToken?:string,profileUrl?:string,roles?:string[],permissions?:string[],responseStatus?:ResponseStatus,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userId;
    /** @type {string} */
    sessionId;
    /** @type {string} */
    userName;
    /** @type {string} */
    displayName;
    /** @type {string} */
    referrerUrl;
    /** @type {string} */
    bearerToken;
    /** @type {string} */
    refreshToken;
    /** @type {string} */
    profileUrl;
    /** @type {string[]} */
    roles;
    /** @type {string[]} */
    permissions;
    /** @type {ResponseStatus} */
    responseStatus;
    /** @type {{ [index: string]: string; }} */
    meta;
}
export class Register {
    /** @param {{userName?:string,firstName?:string,lastName?:string,displayName?:string,email?:string,password?:string,confirmPassword?:string,autoLogin?:boolean,errorView?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userName;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {string} */
    displayName;
    /** @type {string} */
    email;
    /** @type {string} */
    password;
    /** @type {string} */
    confirmPassword;
    /** @type {?boolean} */
    autoLogin;
    /** @type {string} */
    errorView;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'Register' }
    getMethod() { return 'POST' }
    createResponse() { return new RegisterResponse() }
}
export class RegisterResponse {
    /** @param {{userId?:string,sessionId?:string,userName?:string,referrerUrl?:string,bearerToken?:string,refreshToken?:string,roles?:string[],permissions?:string[],responseStatus?:ResponseStatus,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userId;
    /** @type {string} */
    sessionId;
    /** @type {string} */
    userName;
    /** @type {string} */
    referrerUrl;
    /** @type {string} */
    bearerToken;
    /** @type {string} */
    refreshToken;
    /** @type {string[]} */
    roles;
    /** @type {string[]} */
    permissions;
    /** @type {ResponseStatus} */
    responseStatus;
    /** @type {{ [index: string]: string; }} */
    meta;
}
import { $$, JsonApiClient, leftPart } from "@servicestack/client"
import { createApp, reactive } from "vue"
import ServiceStackVue from "@servicestack/vue"

export const BaseUrl = leftPart(import.meta.url, '/mjs')

let AppData = {
    init: false,
    Auth: null,
    UserData: null,
}
let client = null, store = null, Apps = []
export { client, AppData }


/** @param {any} [exports] */
export function init(exports) {
    if (AppData.init) return
    client = JsonApiClient.create(BaseUrl)
    AppData = reactive(AppData)
    AppData.init = true

    if (exports) {
        exports.client = client
        exports.AppData = AppData
        exports.Apps = Apps
    }
}

const alreadyMounted = el => el.__vue_app__

/** Mount Vue3 Component
 * @param sel {string|Element} - Element or Selector where component should be mounted
 * @param component
 * @param [props] {any} 
 * @param {{ mount?:(app, { client, AppData }) => void }} options= */
export function mount(sel, component, props, options) {
    if (!AppData.init) {
        init(globalThis)
    }
    const els = $$(sel)
    els.forEach(el => {
        if (alreadyMounted(el)) return
        const elProps = el.getAttribute('data-props')
        const useProps = elProps ? { ...props, ...(new Function(`return (${elProps})`)()) } : props
        const app = createApp(component, useProps)
        app.provide('client', client)

        app.use(ServiceStackVue)
        if (options?.mount) {
            options.mount(app, { client, AppData })
        }
        app.mount(el)
        Apps.push(app)
    })

    return Apps.length === 1 ? Apps[0] : Apps
}
import { computed, onMounted, onUnmounted, ref, watch } from "vue"
import { useClient, useUtils } from "@servicestack/vue"
import { QueryContacts, ViewAppData } from "../dtos.mjs"

const SelectEmail = {
    template:`<div v-if="show" class="relative w-full">
        <div class="z-10 mt-1 absolute bg-white border border-gray-200 rounded w-full">
          <ul v-if="results.length" role="list" class="divide-y divide-gray-100">
            <li v-for="(sub,index) in results" @click="selectIndex(index)"
                :class="['pl-3 flex gap-x-2 py-2 hover:bg-gray-50 cursor-pointer overflow-hidden whitespace-nowrap', index === active ? 'bg-sky-50' : '']">
              <div class="min-w-0">
                <p class="text-sm font-semibold leading-6 text-gray-900">{{sub.firstName}} {{sub.lastName}}</p>
                <p class="mt-1 truncate text-xs leading-5 text-gray-500">{{sub.email}}</p>
              </div>
            </li>
          </ul>
          <Loading v-else-if="loading" class="p-4 text-base" imageClass="w-5 h-5">Loading...</Loading>
        </div>
    </div>`,
    props:['modelValue','inputElement'],
    setup(props) {
        const client = useClient()
        const { focusNextElement } = useUtils()
        const popupStyle = ref('')
        const email = ref()
        const show = ref(false)
        const api = ref()
        const active = ref(-1)
        const results = computed(() => api.value?.response?.results || [])
        const loading = computed(() => client.loading.value)

        async function update() {
            await (async (search) => {
                const apiSearch = await client.api(new QueryContacts({ search, take:8, orderBy:'nameLower' }))
                if (apiSearch.succeeded && search === props.modelValue) {
                    api.value = apiSearch
                    active.value = -1
                }
            })(props.modelValue)
        }

        function selectIndex(index) {
            const contact = index >= 0 ? results.value[index] : null
            if (contact) {
                const setFields = ['email','firstName','lastName']
                setFields.forEach(id => {
                    const el = props.inputElement.form[id]
                    if (el) {
                        el.value = contact[id]
                        el.dispatchEvent(new Event('input', {bubbles:true}));
                    }
                })
                focusNextElement({ after:props.inputElement.form['lastName'] })
            }
        }

        const inputEvents = {
            focus(e) {
                show.value = true
            },
            blur(e) {
                setTimeout(() => show.value = false, 200)
            },
            keydown(e) {
                if (e.key === 'ArrowDown') {
                    if (!show.value) show.value = true
                    if (active.value === -1) {
                        active.value = 0
                    } else {
                        active.value = (active.value + 1) % results.value.length
                    }
                } else if (e.key === 'ArrowUp') {
                    if (active.value >= 0) {
                        active.value = (active.value - 1) % results.value.length
                        if (active.value < 0) active.value = results.value.length - 1
                    }
                } else if (e.key === 'Enter') {
                    show.value = false
                    e.preventDefault()
                    selectIndex(active.value)
                } else if (e.key === 'Escape') {
                    if (show.value) {
                        show.value = false
                        e.stopPropagation()
                    }
                }
            },
        }

        onMounted(() => {
            client.swr(new QueryContacts({ take:8, orderBy:'nameLower' }), r => api.value = r)
            const el = email.value = document.querySelector('#email')
            el.setAttribute('autocomplete','no-autofill')
            Object.keys(inputEvents).forEach(evt => {
                inputEvents[evt] = inputEvents[evt].bind(el)
                el.addEventListener(evt, inputEvents[evt])
            })
            const rect = el.getBoundingClientRect()
            popupStyle.value = `top:${Math.floor(rect.y+rect.height+2)}px;left:26px;width:${Math.floor(rect.width-4)}px`
        })

        onUnmounted(() => {
            Object.keys(inputEvents).forEach(evt => {
                email.value?.removeEventListener(evt, inputEvents[evt])
            })
        })
        
        watch(() => props.modelValue, update)

        return { show, results, loading, active, selectIndex }
    }
}

export const EmailInput = {
    components: { SelectEmail },
    template: `<TextInput v-bind="$attrs" @update:modelValue="$emit('update:modelValue',$event)">
      <template #footer="{ id, inputElement, modelValue }">
        <SelectEmail v-if="inputElement" :inputElement="inputElement" :modelValue="modelValue" />
      </template>
    </TextInput>`,
    emits:['update:modelValue'],
    props: [],
    setup(props) {
        return { }
    }
}

const InsertVariableButton = {
    template:`<div>
        <svg @click="toggle()" class="w-5 h-5 select-none cursor-pointer text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" xmlns="http://www.w3.org/2000/svg" width="2048" height="2048" viewBox="0 0 2048 2048">
            <title>Insert template variable (CTRL+SPACE)</title>
            <path fill="currentColor" d="M2048 128v1664h-640l128-128h384v-384h-768V768H768v512H128v384h256l128 128H0V128h2048zM640 256H128v384h512V256zm640 0H768v384h512V256zm640 0h-512v384h512V256zm-621 1139l90 90l-429 429l-429-429l90-90l275 275V896h128v774l275-275z"/></svg>
        <div :class="['absolute z-10 mt-2 w-56 -ml-48 origin-top-left rounded-md bg-white shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none', transition1]" role="menu"
            aria-orientation="vertical" aria-labelledby="menu-button" tabindex="-1">
        <div v-if="toggleState" class="py-1" role="none">
          <div v-for="(collection,label) in vars">
            <button type="button" @click="toggleVar(label)" class="hover:bg-gray-50 flex items-center w-full text-left rounded-md p-2 gap-x-3 text-sm leading-6 font-semibold text-gray-700" :aria-controls="'sub-menu-'+label" aria-expanded="false">
              <svg :class="['h-5 w-5 shrink-0', expanded[label] ? 'rotate-90 text-gray-500' : 'text-gray-400']" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z" clip-rule="evenodd" />
              </svg>
              {{label}}
            </button>
            <ul v-if="expanded[label]" class="mt-1 px-2" :id="'sub-menu-'+label">
              <li v-for="(value,name) in collection">
                <span @click="select(label,name)" :title="value" class="cursor-pointer hover:bg-gray-50 block rounded-md py-2 pr-2 pl-9 text-sm leading-6 text-gray-700">{{name}}</span>
              </li>
            </ul>
          </div>
          
        </div>
    </div>
  </div>
    `,
    props:['instance'],
    setup(props) {
        const client = useClient()
        const { transition } = useUtils()
        const expanded = ref({})
        const api = ref()

        const vars = computed(() => ({ 
            contact: {
                Email: 'email@example.org',
                FirstName: 'First',
                LastName: 'Last',
                ExternalRef: '0123456789'
            },
            ...api.value?.response?.vars
        }))

        const toggleState = ref(false)
        const transition1 = ref('hidden')
        const rule1 = {
            entering: { cls:'transition ease-out duration-100', from:'transform opacity-0 scale-95',    to:'transform opacity-100 scale-100'},
            leaving:  { cls:'transition ease-in duration-75',   from:'transform opacity-100 scale-100', to:'transform opacity-0 scale-95'}
        }
        function toggle(show) {
            if (show == null) show = !toggleState.value
            transition(rule1, transition1, show)
            if (show)
                toggleState.value = show
            else 
                setTimeout(() => toggleState.value = show, 100)
        }
        
        onMounted(async () => {
            await client.swr(new ViewAppData(), r => api.value = r)
        })

        function toggleVar(label) {
            expanded.value[label] = !expanded.value[label]
        }       
        function select(group,name) {
            if (group === 'contact') {
                props.instance.insert('{{' + name + '}}','')
            }
            else if (group === 'images') {
                props.instance.insert('![]({{' + `${group}.${name}` + '}})','')
            } else {
                props.instance.insert('{{' + `${group}.${name}` + '}}','')
            }
            toggle(false)
        }
        
        /** @param {KeyboardEvent} e */
        function handleKeyDown(e) {
            console.log('handleKeyDown', e)
            if (e.code === 'Space' && e.ctrlKey) {
                toggle(true)
                e.stopPropagation()
            }
        }
        
        onMounted(() => props.instance.textarea.value?.addEventListener('keydown', handleKeyDown))
        onUnmounted(() => props.instance.textarea.value?.removeEventListener('keydown', handleKeyDown))
        
        return { toggle, toggleState, transition1, vars, expanded, select, toggleVar }
    }
}

export const MarkdownEmailInput = {
    components: { InsertVariableButton },
    template: `<MarkdownInput v-bind="$attrs" @update:modelValue="$emit('update:modelValue',$event)">
      <template #toolbarbuttons="{ instance, textarea }">
        <InsertVariableButton :instance="instance" :textarea="textarea" />
      </template>
    </MarkdownInput>`,
    emits:['update:modelValue'],
    props: [],
    setup(props) {
        return { }
    }
}import { computed, onMounted, ref } from "vue"
import { $$, appendQueryString, combinePaths, queryString, rightPart } from "@servicestack/client"
import ServiceStackVue, { useClient } from "@servicestack/vue"
import { SubscribeToMailingList, UpdateContactMailingLists, FindContact } from "../Mail.dtos.mjs"
import { BaseUrl, mount } from "./init.mjs"

export const JoinMailingList = {
    template:`<div>
      <form v-if="!submitted" v-on:submit.prevent="submit" class="w-full">
        <input type="hidden" name="source" value="Website">
        <input type="hidden" name="mailingLists" :value="mailingLists ? asStrings(mailingLists).join(',') : 'MonthlyNewsletter'">
        <ErrorSummary class="mb-3" except="email,firstName,lastName" />
        <div class="space-y-4">
          <label for="email-address" class="sr-only">Email address</label>
          <div :class="[expand ? 'w-full' : 'w-auto']" style="transition:width 1s ease-in-out, visibility 1s linear">
            <div class="grid grid-cols-2 items-end gap-3">
              <div :class="[expand ? 'col-span-2' : '']">
                  <TextInput class="" v-on:focus="expand=true" id="email" name="email" type="email" autocomplete="email" required label="" :placeholder="placeholder || 'Enter your email'" />
              </div>
              <div v-if="expand" class="">
                  <TextInput class="w-full" id="firstName" required label="" placeholder="First Name" autocomplete="given_name" />
              </div>
              <div v-if="expand" class="">
                  <TextInput class="w-full" id="lastName" required label="" placeholder="Last Name" autocomplete="family_name" />
              </div>
              <div :class="[expand ? 'col-span-2 flex justify-center' : '']">
                  <PrimaryButton>{{ submitLabel || 'Subscribe' }}</PrimaryButton>
              </div>
            </div>
          </div>
        </div>
      </form>
      <div v-else>
         <h3 class="mb-3 flex text-xl font-semibold leading-6 text-gray-900 group-hover:text-gray-600">
            <Icon v-if="thanksIcon" class="w-6 h-6 mr-2" :image="thanksIcon" /> 
            <svg v-else class="w-6 h-6 mr-2 text-green-600" xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15"><path fill="none" stroke="currentColor" d="M4 7.5L7 10l4-5m-3.5 9.5a7 7 0 1 1 0-14a7 7 0 0 1 0 14Z"/></svg>
            {{thanksHeading || 'Thanks for signing up!'}}
         </h3>
         <p class="text-sm leading-6">
            {{thanksMessage || 'To complete sign up, look for the verification email in your inbox and click the link in that email.'}}
         </p>
      </div>
    </div>
    `,
    props:['mailingLists','placeholder','submitLabel','thanksIcon','thanksHeading','thanksMessage'],
    setup(props) {
        const client = useClient()
        const expand = ref(false)
        const submitted = ref(false)
        
        /** @param {SubmitEvent} e */
        async function submit(e) {
            const api = await client.apiFormVoid(new SubscribeToMailingList(), new FormData(e.target))
            if (api.succeeded) {
                submitted.value = true
            }
        }
        function asStrings(o) { return typeof o == 'string' ? o.split(',') : o || [] }
        
        return { expand, submitted, submit, asStrings }
    }
}

export const MailPreferences = {
    template:`<div>
      <Loading v-if="!loaded">Loading...</Loading>
      <div v-else-if="saved" class="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl lg:col-span-7">
        <div class="flex justify-center">
          <svg class="my-4 w-16 h-16 text-green-600" xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15"><path fill="none" stroke="currentColor" d="M4 7.5L7 10l4-5m-3.5 9.5a7 7 0 1 1 0-14a7 7 0 0 1 0 14Z"/></svg>
        </div>
        <h2 class="mb-3 inline sm:block lg:inline xl:block">{{ updatedHeading || 'Updated!' }}</h2>
        <p class="inline sm:block lg:inline xl:block">{{ updatedMessage || 'Your email preferences have been saved.' }}</p>
      </div>
      <div v-else-if="unsubscribed" class="text-gray-900 sm:text-4xl lg:col-span-7">
        <div class="flex justify-center">
          <svg class="my-4 w-16 h-16 text-indigo-600" xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 48 48"><g fill="none" stroke="currentColor" stroke-linecap="round" stroke-width="4"><path d="m35 26.614l-19.854-19.3a2.928 2.928 0 0 0-4.259.188a3.334 3.334 0 0 0 .18 4.544l10.024 9.93"/><path stroke-linejoin="round" d="M21.09 21.976L10.178 11.155a3.486 3.486 0 0 0-4.735-.161a3.032 3.032 0 0 0-.18 4.417l10.993 11.203"/><path d="M16.255 26.614L10 20.5a3.299 3.299 0 0 0-4.633-.08a3.233 3.233 0 0 0-.093 4.588c9.23 9.536 14.02 14.04 15.817 15.545C24 42.99 29.735 44 32.73 42c2.995-2 5.702-4.846 6.987-7.671c.765-1.683 2.25-6.87 4.458-15.561a3.305 3.305 0 0 0-2.46-4.034a3.5 3.5 0 0 0-4.166 2.493L35 26.614m-3.284-13.948a19.597 19.597 0 0 0-8.752-7.187M5.194 33.776a19.599 19.599 0 0 0 8.364 7.635"/></g></svg>
        </div>
        <h2 class="mb-3 text-3xl font-bold tracking-tight inline sm:block lg:inline xl:block">{{ unsubscribeHeading || 'Updated!' }}</h2>
        <p class="text-2xl inline sm:block lg:inline xl:block">
          {{ unsubscribeMessage || "You've been unsubscribed from all email subscriptions, we're sorry to see you go!" }}
        </p>
      </div>
      <div v-else-if="contact">
        <div v-if="unsubscribeView">
          <p class="mb-3">
            {{ unsubscribePrompt || 'Unsubscribe from all future email communications:' }}
          </p>
          <div class="my-8 flex justify-center">
            <PrimaryButton type="button" color="red" @click="submitUnsubscribe">{{ submitUnsubscribeLabel || 'Unsubscribe' }}</PrimaryButton>
          </div>
        </div>
        <p class="mb-3">
          Manage mail preferences for <b>{{contact.email}}</b>:
        </p>
        <form @submit.prevent="submit" class="flex justify-center">
          <div class="">
            <div v-for="(value,index) in mailingListType.enumValues">
              <div v-if="parseInt(value) > 1">
                <input v-model="contactMailingLists" type="checkbox" :id="'chk-'+value" class="h-4 w-4 rounded border-gray-300 dark:border-gray-600 text-indigo-600 dark:text-indigo-300 focus:ring-indigo-600" :value="mailingListType.enumDescriptions[index] || mailingListType.enumNames[index]">
                <label class="select-none ml-3 text-sm font-medium leading-6 text-gray-900 dark:text-gray-50" :for="'chk-'+value">{{mailingListType.enumDescriptions[index] || mailingListType.enumNames[index]}}</label>
              </div>
            </div>
            <PrimaryButton class="mt-8">{{ submitLabel || 'Save Changes' }}</PrimaryButton>
          </div>
        </form>
      </div>
      <div v-else>
        <p>
          {{ emailPrompt || 'Enter your email to manage your email preferences:' }}
        </p>
        <ErrorSummary class="my-3" />
        <form @submit.prevent="findContact" class="space-x-4 flex items-end">
          <div><TextInput id="email" v-model="email" label="" placeholder="Enter your email" /></div>
          <div><PrimaryButton>{{ submitEmailLabel || 'Submit' }}</PrimaryButton></div>
        </form>
      </div>
    </div>`,
    props:['emailPrompt','submitEmailPrompt','updatedHeading','updatedMessage','unsubscribePrompt','unsubscribeHeading','unsubscribeMessage','submitLabel','submitUnsubscribeLabel'],
    setup(props) {
        const client = useClient()
        const contact = ref()
        const email = ref()
        const metadata = ref()
        const mailingListType = computed(() => metadata.value?.api.types.find(x => x.name === 'MailingList'))
        const contactMailingLists = ref([])
        const saved = ref(false)
        const unsubscribeView = ref(false)
        const unsubscribed = ref(false)
        const loaded = ref(false)
        
        async function findContact(e) {
            if (!email.value) return
            const api = await client.api(new FindContact({
                email: email.value,
            }))
            if (api.succeeded) {
                contact.value = api.response.result
                if (contact.value) {
                    contactMailingLists.value = enumFlags(contact.value.mailingLists)
                } else {
                    client.setError({ message: 'No existing email subscription was found' })
                }
            }
        }

        async function submitUnsubscribe(e) {
            const api = await client.apiVoid(new UpdateContactMailingLists({
                ref: contact.value.externalRef,
                unsubscribeFromAll: true,
            }))
            if (api.succeeded) {
                unsubscribed.value = true
            }
        }

        async function submit(e) {
            const api = await client.apiVoid(new UpdateContactMailingLists({
                ref: contact.value.externalRef,
                mailingLists: contactMailingLists.value
            }))
            if (api.succeeded) {
                saved.value = true
            }
        }

        function enumFlags(value) {
            const enumType = mailingListType.value
            if (!enumType) throw new Error(`Could not find MailingList`)
            const to = []
            for (let i=0; i<enumType.enumValues.length; i++) {
                const enumValue = parseInt(enumType.enumValues[i])
                if (enumValue > 0 && (enumValue & value) === enumValue) {
                    to.push(enumType.enumDescriptions?.[i] || enumType.enumNames?.[i] || `${value}`)
                }
            }
            return to
        }
        
        onMounted(async () => {
            metadata.value = await (await fetch(appendQueryString(combinePaths(BaseUrl, `/metadata/app.json`), {includeTypes: 'MailingList'}))).json()
            
            const search = location.search ? location.search : location.hash.includes('?') ? '?' + rightPart(location.hash,'?') : ''
            let qs = queryString(search)
            if (qs.email || qs.ref) {
                const api = await client.api(new FindContact({ 
                    email: qs.email,
                    ref: qs.ref
                }))
                if (api.succeeded) {
                    contact.value = api.response.result
                    if (contact.value) {
                        contactMailingLists.value = enumFlags(contact.value.mailingLists)
                    }
                }
            }
            loaded.value = true
            unsubscribeView.value = !!qs.unsubscribe
        })
        
      return { loaded, contact, email, findContact, submit, enumFlags, mailingListType, contactMailingLists, saved,
               unsubscribeView, unsubscribed, submitUnsubscribe }  
    }
}

const components = { JoinMailingList, MailPreferences }
export function mail(selector, args) {
    const mountOptions = {
        mount(app, { client, AppData }) {
            app.component('RouterLink', ServiceStackVue.component('RouterLink'))
        }
    }

    $$(selector).forEach(el => {
        const mail = el.getAttribute('data-mail')
        if (!mail) throw new Error(`Missing data-mail=Component`)
        const component = components[mail]
        if (!component) throw new Error(`Unknown component '${mail}', available components: ${Object.keys(components).join(', ')}`)
        mount(el, component, args, mountOptions)
    })
}
import { onMounted, watch, computed, ref, inject, reactive, createApp, nextTick, getCurrentInstance } from "vue"
import ServiceStackVue, { useClient, useAuth, useUtils } from "@servicestack/vue"
import { isDate, toDate, fromXsdDuration, indexOfAny, map, leftPart, $$, enc, EventBus } from "@servicestack/client"
import {
    GetThread,
    GetThreadUserData,
    CreateThreadLike,
    DeleteThreadLike,
    QueryComments,
    CreateComment,
    CreateCommentVote,
    DeleteCommentVote,
    DeleteComment,
    CreateCommentReport,
    PostReport, 
} from '../Posts.dtos.mjs'
import { Authenticate, SignUpDialog, SignInDialog } from "./Auth.mjs"
import { BaseUrl, mount } from "./init.mjs"

export class Store {
    BaseUrl = BaseUrl
    DefaultProfileUrl = 'data:image/svg+xml,%3Csvg style=\'color:rgb(8 145 178);border-radius: 9999px;overflow: hidden;\' xmlns=\'http://www.w3.org/2000/svg\' viewBox=\'0 0 200 200\'%3E%3Cpath fill=\'currentColor\' d=\'M200,100 a100,100 0 1 0 -167.3,73.9 a3.6,3.6 0 0 0 0.9,0.8 a99.9,99.9 0 0 0 132.9,0 l0.8,-0.8 A99.6,99.6 0 0 0 200,100 zm-192,0 a92,92 0 1 1 157.2,64.9 a75.8,75.8 0 0 0 -44.5,-34.1 a44,44 0 1 0 -41.4,0 a75.8,75.8 0 0 0 -44.5,34.1 A92.1,92.1 0 0 1 8,100 zm92,28 a36,36 0 1 1 36,-36 a36,36 0 0 1 -36,36 zm-59.1,42.4 a68,68 0 0 1 118.2,0 a91.7,91.7 0 0 1 -118.2,0 z\' /%3E%3C/svg%3E%0A'
    /** @type {AuthenticateResponse|null} */
    auth = null
    /** @type {string|null} */
    authKey = null
    /** @type {GetThreadUserDataResponse|null} */
    userData = null
    /** @type {Thread|null} */
    thread = null
    /** @type {JsonServiceClient} */
    client = null
    events = new EventBus()
    config = {
        commentLink: null
    }

    constructor(client) {
        this.client = client
        const { swrCacheKey } = useUtils()
        this.authKey = swrCacheKey(new Authenticate())
        this.userDataKey = swrCacheKey(new GetThreadUserData())
    }
    signIn(auth) {
        const { signIn } = useAuth()
        this.auth = auth
        this.userData = {
            upVoted: [],
            downVoted: [],
        }
        signIn(auth)
        auth._date = new Date().valueOf()
        localStorage.setItem(this.authKey, JSON.stringify(auth))
        this.loadCachedUserData()
        this.events.publish('signIn', auth)
    }
    signOut() {
        this.auth = null
        this.userData = null
        this.userAlbumArtifactsKey = this.userLikesKey = null
        const { signOut } = useAuth()
        signOut()
        this.events.publish('signOut')
    }
    loadCachedUserData() {
        const cacheKey = this.userDataKey
        const json = localStorage.getItem(cacheKey)
        if (json) {
            this.userData = JSON.parse(json)
            this.events.publish('userData', this.userData)
        }
    }
    async loadUserData() {
        if (!this.thread) return
        const cacheKey = this.userDataKey
        const api = await this.client.api(new GetThreadUserData({
            threadId: this.thread.id
        }))
        if (api.succeeded) {
            this.userData = api.response
            localStorage.setItem(cacheKey, JSON.stringify(this.userData))
            this.events.publish('userData', this.userData)
        } else {
            this.userData = null
            localStorage.removeItem(cacheKey)
        }
    }    
}

const ModalForm = {
    template: /*html*/`<div class="relative z-10" aria-labelledby="modal-title" role="dialog" aria-modal="true">
        <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"></div>
        <div class="fixed inset-0 z-10 overflow-y-auto">
            <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
                <div class="relative transform overflow-hidden rounded-lg bg-white dark:bg-black text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg">
                    <slot></slot>
                </div>
            </div>
        </div>
    </div>`
}

const NewReport = {
    components: { ModalForm },
    template: /*html*/`<ModalForm class="z-30">
    <form @submit.prevent="submit">
        <div class="shadow overflow-hidden sm:rounded-md bg-white dark:bg-black">
            <div class="relative px-4 py-5 sm:p-6">
                <fieldset>
                    <legend class="text-base font-medium text-gray-900 dark:text-gray-100 text-center mb-4">Report Comment</legend>

                    <ErrorSummary :except="visibleFields" />

                    <div class="grid grid-cols-6 gap-6">
                        <div class="col-span-6">
                            <select-input id="type" label="Reason" v-model="postReport" :options="PostReport" />
                        </div>
                        <div class="col-span-6">
                            <textarea-input id="description" v-model="description" placeholder="Please describe the issue for our moderation team to review" />
                        </div>
                    </div>
                </fieldset>
            </div>
            <div class="mt-4 px-4 py-3 bg-gray-50 dark:bg-gray-900 text-right sm:px-6">
                <div class="flex justify-end items-center">
                    <SecondaryButton class="mr-2" @click="$emit('done')">Cancel</SecondaryButton>
                    <PrimaryButton type="submit">Submit</PrimaryButton>
                </div>
            </div>
        </div>
    </form>
</ModalForm>`,
    emits:['done'],
    props: { commentId:Number },
    setup(props, { emit }) {
        const visibleFields = ['PostReport', 'Description']
        const postReport = ref('')
        const description = ref('')
        const client = useClient()

        function submit() {
            const { commentId } = props
            client.apiVoid(new CreateCommentReport({ commentId, postReport:PostReport[postReport.value], description }))
                .then(r => emit('done'))
        }

        return {
            PostReport,
            visibleFields,
            postReport,
            description,
            submit,
        }
    }
}

export const ThreadDialogs = {
    components: { SignInDialog, SignUpDialog, NewReport, },
    template:`<div>
        <SignInDialog v-if="show==='SignInDialog'" @done="$emit('done')"  @signup="$emit('showDialog','SignUpDialog')" />
        <SignUpDialog v-if="show==='SignUpDialog'" @done="$emit('done')"  @signin="$emit('showDialog','SignInDialog')" />
        <NewReport v-if="show==='NewReport'" :commentId="commentId" @done="$emit('done')" />
    </div>`,
    emits:['showDialog','done'],
    props: {
        show:String,
        commentId:Number
    },
    setup(props) {
        return { }
    }
}

export const InputComment = {
    template: /*html*/`
        <div class="w-full">
            <div class="flex flex-col w-full border border-gray-300 dark:border-gray-700 rounded bg-gray-50 dark:bg-gray-900 overflow-hidden">
                <textarea v-model="request.content" class="w-full h-24 m-0 border-none outline-none dark:bg-transparent" placeholder="Write a comment"></textarea>
                <div class="flex justify-between p-2 pl-4 bg-dark-100 dark:bg-black items-center">
                    <div>
                        <a v-if="store.config.commentLink" :href="store.config.commentLink.href" target="_blank" class="text-sm text-gray-400 hover:text-gray-600">{{ store.config.commentLink.label }}</a>
                    </div>
                    <div>
                        <span class="mr-2 text-sm text-gray-400">{{ remainingChars }}</span>
                        <SecondaryButton @click="submit" :disabled="loading">Post</SecondaryButton>
                    </div>
                </div>
            </div>
            <div class="flex flex-col">
                <ErrorSummary class="mt-1" />
            </div>
        </div>
    `,
    props: ['threadId','replyId'],
    emits: ['updated'],
    setup(props, { attrs, emit }) {
        /** @type {Store} */
        const store = inject('store')
        const client = useClient()
        
        const request = ref(new CreateComment({ 
            threadId: props.threadId, 
            replyId: props.replyId,
            content: ''
        }))
        const remainingChars = computed(() => 280 - request.value.content.length)

        async function submit() {
            const { threadId, replyId } = props
            const api = await client.api(request.value)
            if (api.succeeded) {
                request.value.content = ''
                emit('updated', api.response)
            }
        }

        return {
            store,
            request,
            loading: client.loading,
            remainingChars,
            submit,
        }
    },
}

const Comment = {
    components: { InputComment },
    template: /*html*/`<div class="py-1 border-b border-gray-100 dark:border-gray-800">
        <div class="relative group py-2 px-2 hover:bg-gray-50 dark:hover:bg-gray-900 rounded-lg">
            <div class="hidden group-hover:block absolute top-2 right-2">
                <svg @click="toggleMenu" class="w-7 h-7 bg-gray-100 dark:bg-gray-800 rounded cursor-pointer hover:bg-white dark:hover:bg-black" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 256 256"><circle cx="64" cy="128" r="12" fill="currentColor"/><circle cx="192" cy="128" r="12" fill="currentColor"/><circle cx="128" cy="128" r="12" fill="currentColor"/></svg>
                <div v-if="showMenu" class="absolute -ml-20">
                    <div class="select-none rounded-md whitespace-nowrap bg-white dark:bg-black shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none" role="menu" aria-orientation="vertical" aria-labelledby="menu-button" tabindex="-1">
                        <div class="py-1" role="none">
                            <div @click="showDialog('NewReport')" class="flex cursor-pointer text-gray-700 dark:text-gray-300 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800 px-4 py-2 text-sm" role="menuitem" tabindex="-1">
                                <svg class="mr-2 h-5 w-5 text-gray-400 group-hover:text-gray-500" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M3 3v1.5M3 21v-6m0 0l2.77-.693a9 9 0 0 1 6.208.682l.108.054a9 9 0 0 0 6.086.71l3.114-.732a48.524 48.524 0 0 1-.005-10.499l-3.11.732a9 9 0 0 1-6.085-.711l-.108-.054a9 9 0 0 0-6.208-.682L3 4.5M3 15V4.5"/></svg>
                                Report
                            </div>
                            <div @click="deleteComment" class="flex cursor-pointer text-gray-700 dark:text-gray-300 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800 px-4 py-2 text-sm" role="menuitem" tabindex="-1">
                                <svg class="mr-2 h-5 w-5 text-gray-400 group-hover:text-gray-500" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path fill="currentColor" d="M6 19a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V7H6v12M8 9h8v10H8V9m7.5-5l-1-1h-5l-1 1H5v2h14V4h-3.5Z"/></svg>
                                Delete
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="flex select-none">
                <img :src="comment.avatar ? comment.avatar : comment.profileUrl" class="w-6 h-6 rounded-full mr-2" />
                <div class="text-sm text-gray-600 dark:text-gray-300">
                    {{comment.handle || comment.displayName}} <span class="px-1">&#8226;</span> {{ timeAgo }}
                </div>
            </div>
            <div class="py-2 text-gray-900 dark:text-gray-50">
                <span v-if="comment.flagReason" class="text-gray-500 text-sm">[flagged]</span>
                <span v-else>{{comment.content}}</span>
            </div>
            <div class="text-sm text-gray-600 dark:text-gray-300 flex justify-between h-6">
                <div class="flex items-center">
                    <svg v-if="hasUpVoted()" class="w-4 h-4 cursor-pointer" @click="upVote" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 256 256"><title>unvote</title><path fill="currentColor" d="M231.4 123.1a8 8 0 0 1-7.4 4.9h-40v80a16 16 0 0 1-16 16H88a16 16 0 0 1-16-16v-80H32a8 8 0 0 1-7.4-4.9a8.4 8.4 0 0 1 1.7-8.8l96-96a8.1 8.1 0 0 1 11.4 0l96 96a8.4 8.4 0 0 1 1.7 8.8Z"/></svg>
                    <svg v-else class="w-4 h-4 cursor-pointer" @click="upVote" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><title>vote</title><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="m12 3l9 7h-4.99L16 21H8V10H3l9-7Z"/></svg>
                    <span class="px-2 select-none">{{ comment.votes }}</span>
                    <svg v-if="hasDownVoted()" class="w-4 h-4 cursor-pointer" @click="downVote" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 256 256"><title>unvote</title><path fill="currentColor" d="m229.7 141.7l-96 96a8.1 8.1 0 0 1-11.4 0l-96-96a8.4 8.4 0 0 1-1.7-8.8A8 8 0 0 1 32 128h40V48a16 16 0 0 1 16-16h80a16 16 0 0 1 16 16v80h40a8 8 0 0 1 7.4 4.9a8.4 8.4 0 0 1-1.7 8.8Z"/></svg>
                    <svg v-else class="w-4 h-4 cursor-pointer" @click="downVote" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><title>down vote</title><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="m12 21l9-7h-4.99L16 3H8v11H3l9 7Z"/></svg>
                </div>
                <div>
                    <button type="button" @click="toggleReply" class="hidden group-hover:inline-flex items-center rounded-full rounded-tl-none border border-transparent bg-indigo-600 px-3 py-1 text-xs font-medium text-white shadow-sm hover:bg-indigo-700 focus:outline-none focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-indigo-500 focus:ring-offset-2 dark:ring-offset-black">
                        <svg class="-ml-0.5 mr-2 h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16"><path fill="currentColor" d="M8 1C3.6 1 0 3.5 0 6.5c0 2 2 3.8 4 4.8c0 2.1-2 2.8-2 2.8c2.8 0 4.4-1.3 5.1-2.1H8c4.4 0 8-2.5 8-5.5S12.4 1 8 1z"/></svg>
                        reply
                    </button>
                </div>
            </div>
        </div>
        <div v-if="showReply">
            <div class="flex p-2">
                <div class="grow-0 flex flex-col">
                    <div class="grow-0">
                        <img :src="comment.avatar ? comment.avatar : comment.profileUrl" class="w-6 h-6 rounded-full mr-2" />
                    </div>
                    <div class="grow relative">
                        <span class="absolute top-1.5 left-2.5 -ml-px h-full w-[1px] bg-gray-200 dark:bg-gray-800" aria-hidden="true"></span>
                    </div>
                </div>
                <div class="grow pl-1 relative">
                    <svg @click="showReply=false" class="absolute top-2 right-2 w-5 h-5 text-gray-300 dark:text-gray-700 hover:text-gray-500 cursor-pointer" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                        <title>close</title>
                        <path fill="currentColor" d="M6.4 19L5 17.6l5.6-5.6L5 6.4L6.4 5l5.6 5.6L17.6 5L19 6.4L13.4 12l5.6 5.6l-1.4 1.4l-5.6-5.6Z"/></svg>
                    <InputComment :thread-id="comment.threadId" :reply-id="comment.id" @updated="replyDone" />
                </div>
            </div>
        </div>
    </div>`,
    props: ['comment', 'nested'],
    emits: ['voted', 'unvoted', 'showDialog', 'refresh'],
    setup(props, { emit }) {
        /** @type {Store} */
        const store = inject('store')
        const { user } = useAuth()
        const instance = getCurrentInstance()

        const timeAgo = computed(() => Relative.from(props.comment.createdDate))
        const client = useClient()
        const showMenu = ref(false)
        const showReply = ref(false)
        const replyDone = () => { showReply.value = false; emit('refresh');  }
        let hasUpVoted = () => store.userData?.upVoted.indexOf(props.comment.id) >= 0
        let hasDownVoted = () => store.userData?.downVoted.indexOf(props.comment.id) >= 0

        function showDialog(dialog) {
            showMenu.value = false
            emit('showDialog', dialog, props.comment)
        }
        
        function toggleMenu() {
            if (!user.value) {
                emit('showDialog', 'SignInDialog')
                return
            }
            showMenu.value =! showMenu.value
        }
        
        function toggleReply() {
            if (!user.value) {
                emit('showDialog', 'SignInDialog')
                return
            }
            showReply.value =! showReply.value
        }

        async function vote(value) {
            if (user.value) {
                if (!hasUpVoted() && !hasDownVoted()) {
                    props.comment.votes += value
                    const api = await client.apiVoid(new CreateCommentVote({
                        commentId: props.comment.id,
                        vote: value,
                    }))
                    if (!api.succeeded) {
                        props.comment.votes -= value
                    } else {
                        emit('voted', props.comment, value, api)
                    }
                } else {
                    if (hasUpVoted()) {
                        props.comment.votes += -1
                    }
                    if (hasDownVoted()) {
                        props.comment.votes += 1
                    }
                    const api = await client.apiVoid(new DeleteCommentVote({ commentId: props.comment.id }))
                    if (!api.succeeded) {
                        if (hasUpVoted.value) {
                            props.comment.votes += 1
                        }
                        if (hasDownVoted.value) {
                            props.comment.votes += -1
                        }
                    } else {
                        emit('unvoted', props.comment, value, api)
                    }
                }
            } else {
                emit('showDialog','SignInDialog')
            }
        }

        async function deleteComment() {
            showMenu.value = false
            const api = await client.apiVoid(new DeleteComment({ id: props.comment.id }))
            if (api.succeeded) {
                emit('refresh')
            }
        }

        const upVote = () => vote(1)
        const downVote = () => vote(-1)
        
        onMounted(() => {
            store.events.subscribe('userData', () => {
                instance?.proxy?.$forceUpdate()
            })
        })

        return {
            showMenu,
            showReply,
            toggleMenu,
            toggleReply,
            showDialog,
            replyDone,
            timeAgo,
            upVote,
            downVote,
            hasUpVoted,
            hasDownVoted,
            deleteComment,
        }
    }
}

const Thread = {
    components: { Comment },
    template: /*html*/`
    <div>
        <div v-for="(comment,index) in comments.filter(x => x.replyId == parentId)">
            <div :class="['flex', nested ? 'pl-1' : '']">
                <div v-if="nested" class="grow-0 flex flex-col">
                    <div class="w-6"></div>
                    <div class="grow relative">
                        <div class="">
                            <span class="absolute top-1 left-4 -ml-px h-full w-[1px] border-gray-200 dark:bg-gray-800" aria-hidden="true"></span>
                        </div>
                    </div>
                </div>
                <div class="grow relative">
                    <Comment :comment="comment" :nested="nested"
                            @voted="refreshUserData" @unvoted="refreshUserData" @showDialog="showDialog" @refresh="refresh" />
                    <div class="">
                        <Thread :comments="comments" :parentId="comment.id" 
                                @refresh="refresh" @refreshUserData="refreshUserData" @showDialog="showDialog" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    `,
    props: ['comments','parentId'],
    emits: ['refresh','refreshUserData','showDialog'],
    setup(props, { emit }) {
        /** @type {Store} */
        const store = inject('store')
        const refreshUserData = () => emit('refreshUserData')
        const showDialog = (dialog, comment) => emit('showDialog', dialog, comment)
        const refresh = () => emit('refresh')
        const nested = computed(() => props.parentId != null)
        return {
            store,
            showDialog,
            nested,
            refresh,
            refreshUserData,
        }
    }
}
Thread.components['Thread'] = Thread

export const PostComments = {
    components: { ThreadDialogs, Thread, Comment, InputComment, NewReport },
    template: /*html*/`
    <div class="mt-24 mx-auto flex flex-col w-full max-w-3xl transition-opacity">
        <div v-if="!hide.includes('threadLikes')" class="mb-12 flex">
            <div @click="toggleLike" class="cursor-pointer flex items-center select-none">
                <svg v-if="!store.userData?.liked" class="text-gray-700 w-8 h-8" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><title>Recommend Post</title><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M6.633 10.5c.806 0 1.533-.446 2.031-1.08a9.041 9.041 0 0 1 2.861-2.4c.723-.384 1.35-.956 1.653-1.715a4.498 4.498 0 0 0 .322-1.672V3a.75.75 0 0 1 .75-.75A2.25 2.25 0 0 1 16.5 4.5c0 1.152-.26 2.243-.723 3.218c-.266.558.107 1.282.725 1.282h3.126c1.026 0 1.945.694 2.054 1.715c.045.422.068.85.068 1.285a11.95 11.95 0 0 1-2.649 7.521c-.388.482-.987.729-1.605.729H13.48a4.53 4.53 0 0 1-1.423-.23l-3.114-1.04a4.501 4.501 0 0 0-1.423-.23H5.904M14.25 9h2.25M5.904 18.75c.083.205.173.405.27.602c.197.4-.078.898-.523.898h-.908c-.889 0-1.713-.518-1.972-1.368a12 12 0 0 1-.521-3.507c0-1.553.295-3.036.831-4.398C3.387 10.203 4.167 9.75 5 9.75h1.053c.472 0 .745.556.5.96a8.958 8.958 0 0 0-1.302 4.665a8.97 8.97 0 0 0 .654 3.375Z"/></svg>
                <svg v-else class="text-gray-700 w-8 h-8" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><title>Unrecommend</title><path fill="currentColor" d="M7.493 18.75c-.425 0-.82-.236-.975-.632A7.48 7.48 0 0 1 6 15.375a7.47 7.47 0 0 1 1.602-4.634c.151-.192.373-.309.6-.397c.473-.183.89-.514 1.212-.924a9.042 9.042 0 0 1 2.861-2.4c.723-.384 1.35-.956 1.653-1.715a4.498 4.498 0 0 0 .322-1.672V3a.75.75 0 0 1 .75-.75a2.25 2.25 0 0 1 2.25 2.25c0 1.152-.26 2.243-.723 3.218c-.266.558.107 1.282.725 1.282h3.126c1.026 0 1.945.694 2.054 1.715c.045.422.068.85.068 1.285a11.95 11.95 0 0 1-2.649 7.521c-.388.482-.987.729-1.605.729H14.23a4.53 4.53 0 0 1-1.423-.23l-3.114-1.04a4.501 4.501 0 0 0-1.423-.23h-.777Zm-5.162-7.773a11.969 11.969 0 0 0-.831 4.398a12 12 0 0 0 .52 3.507c.26.85 1.084 1.368 1.973 1.368H4.9c.445 0 .72-.498.523-.898a8.963 8.963 0 0 1-.924-3.977c0-1.708.476-3.305 1.302-4.666c.245-.403-.028-.959-.5-.959H4.25c-.832 0-1.612.453-1.918 1.227Z"/></svg>
                <span class="ml-2">{{ store.thread?.likesCount || ''}}</span>
            </div>
        </div>
        
        <div v-if="user" class="flex justify-center w-full">
            <InputComment :thread-id="threadId" @updated="refresh" />
        </div>
        <div v-else class="flex justify-center w-full">
            <div class="flex justify-between w-full max-w-2xl border border-gray-200 dark:border-gray-700 rounded bg-gray-50 dark:bg-gray-900 overflow-hidden">
                <div class="p-2 pl-4 flex items-center">
                    <span class="text-gray-600 dark:text-gray-300">Sign in to leave a comment</span>
                </div>
                <div class="m-1">
                    <TextLink @click="show='SignInDialog'" class="mr-4 text-gray-500 font-semibold">Sign In</TextLink>
                    <SecondaryButton @click="show='SignUpDialog'">Sign Up</SecondaryButton>
                </div>
            </div>
        </div>
        <div v-if="comments.length" class="mt-8">
            <h2 class="text-xl border-b border-gray-200 dark:border-gray-800 py-2">{{ comments.length }} Comment{{ comments.length > 1 ? 's' : '' }}</h2>
            <Thread :comments="comments" :parent-id="null"
                    @refresh="refresh" @refreshUserData="refreshUserData" @showDialog="showDialog" />
        </div>
        <ErrorSummary />
        <ThreadDialogs :show="show" :comment-id="showTarget?.id" @done="show=''" @showDialog="show=$event" />
    </div>
    `,
    props: ['hide','commentLink'],
    setup(props) {
        /** @type {Store} */
        const store = inject('store')
        const client = useClient()
        const { user } = useAuth()
        const { asStrings } = useUtils()
        const instance = getCurrentInstance()
        store.config.commentLink = props.commentLink
        
        const hide = computed(() => asStrings(props.hide))
        let comments = ref([])
        let show = ref('')
        let showTarget = ref(null)
        const threadId = computed(() => store.thread?.id || 0)
        
        function showDialog(dialog,comment) {
            show.value = dialog
            showTarget.value = comment
        }

        async function refreshUserData() {
            await store.loadUserData()
        }

        async function refresh() {
            // console.log('refresh', store.thread?.id)
            const threadId = store.thread?.id
            if (!threadId) return
            const api = await client.api(new QueryComments({ threadId }));
            if (api.succeeded) {
                comments.value = api.response.results
            }
            await refreshUserData()
        }
        
        async function toggleLike() {
            if (!user.value) {
                showDialog('SignInDialog')
                return
            }
            
            const threadId = store.thread?.id
            if (!threadId || !store.userData) return

            const liked = store.userData.liked
            
            store.userData.liked = !liked 
            store.thread.likesCount += liked ? -1 : 1
            
            const request = !liked
                ? new CreateThreadLike({ threadId })
                : new DeleteThreadLike({ threadId })
            const api = await client.apiVoid(request)
            if (!api.succeeded) {
                store.userData.liked = liked
                store.thread.likesCount += liked ? 1 : -1
            }
            instance?.proxy.$forceUpdate()
        }

        onMounted(async () => {
            store.events.subscribe('thread', refresh)
            await refresh()
        })
        watch(() => user.value, refresh)

        return {
            store,
            hide,
            threadId,
            comments,
            toggleLike,
            user,
            loading: client.loading,
            refresh,
            refreshUserData,
            show,
            showDialog,
            showTarget,
        }
    }
}


const defaultFormats = { locale: map(navigator.languages, x => x[0]) || navigator.language || 'en' }
const Relative = (function () {
    let nowMs = () => new Date().getTime()

    let DateChars = ['/', 'T', ':', '-']
    /** @param {string|Date|number} val */
    function toRelativeNumber(val) {
        if (val == null) return NaN
        if (typeof val == 'number')
            return val
        if (isDate(val))
            return val.getTime() - nowMs()
        if (typeof val === 'string') {
            let num = Number(val)
            if (!isNaN(num))
                return num
            if (val[0] === 'P' || val.startsWith('-P'))
                return fromXsdDuration(val) * 1000 * -1
            if (indexOfAny(val, DateChars) >= 0)
                return toDate(val).getTime() - nowMs()
        }
        return NaN
    }
    let defaultRtf = new Intl.RelativeTimeFormat(defaultFormats.locale, {})
    let year = 24 * 60 * 60 * 1000 * 365
    let units = {
        year,
        month: year / 12,
        day: 24 * 60 * 60 * 1000,
        hour: 60 * 60 * 1000,
        minute: 60 * 1000,
        second: 1000
    }
    /** @param {number} elapsedMs
     *  @param {Intl.RelativeTimeFormat} [rtf] */
    function fromMs(elapsedMs, rtf) {
        for (let u in units) {
            if (Math.abs(elapsedMs) > units[u] || u === 'second')
                return (rtf || defaultRtf).format(Math.round(elapsedMs / units[u]), u)
        }
    }
    /** @param {string|Date|number} val
     *  @param {Intl.RelativeTimeFormat} [rtf] */
    function from(val, rtf) {
        let num = toRelativeNumber(val)
        if (!isNaN(num))
            return fromMs(num, rtf)
        console.error(`Cannot convert ${val}:${typeof val} to relativeTime`)
        return ''
    }
    /** @param {Date} d
     *  @param {Date} [from] */
    let fromDate = (d, from) =>
        fromMs(d.getTime() - (from ? from.getTime() : nowMs()))

    return {
        from,
        fromMs,
        fromDate,
    }
})();


const components = { PostComments }

export function post(selector, args) {
    const mountOptions = {
        mount(app, { client, AppData }) {
            const store = new Store(client)
            nextTick(async () => {
                const [api, threadApi] = await Promise.all([
                    client.api(new Authenticate()),
                    client.api(new GetThread({ url: leftPart(location.href.replace('#','?'),'?') }))
                ])
                if (threadApi.succeeded) {
                    store.thread = threadApi.response.result
                    store.events.publish('thread', store.thread)
                }
                if (api.succeeded) {
                    store.signIn(api.response)
                    await store.loadUserData()
                } else {
                    store.signOut()
                }
            })
            
            app.provide('store', store)
            app.component('RouterLink', ServiceStackVue.component('RouterLink'))
            app.directive('highlightjs', (el, binding) => {
                if (binding.value) {
                    el.innerHTML = enc(binding.value)
                    globalThis.hljs.highlightElement(el)
                }
            })
            globalThis.store = store
        }
    }
    
    $$(selector).forEach(el => {
        const post = el.getAttribute('data-post')
        if (!post) throw new Error(`Missing data-post=Component`)
        const component = components[post]
        if (!component) throw new Error(`Unknown component '${post}', available components: ${Object.keys(components).join(', ')}`)
        mount(el, component, args, mountOptions)
    })
}
