import { createApp, reactive, ref } from "vue"
import { JsonApiClient, $1, $$ } from "@servicestack/client"
import ServiceStackVue from "@servicestack/vue"
import HelloApi from "./components/HelloApi.mjs"
import GettingStarted from "./components/GettingStarted.mjs"
import ShellCommand from "./components/ShellCommand.mjs"
import VueComponentGallery from "./components/VueComponentGallery.mjs"
import VueComponentLibrary from "./components/VueComponentLibrary.mjs"
import { PagingNav, FileLayout } from './components/Docs.mjs'

let client = null, Apps = []
let AppData = {
    init:false
}
export { client, Apps }

/** Simple inline component examples */
const Hello = {
    template: `<b>Hello, {{name}}!</b>`,
    props: { name:String }
}
const Counter = {
    template: `<b @click="count++">Counter {{count}}</b>`,
    setup() {
        let count = ref(1)
        return { count }
    }
}
const Plugin = {
    template:`<div>
        <PrimaryButton @click="show=true">Open Modal</PrimaryButton>
        <ModalDialog v-if="show" @done="show=false">
            <div class="p-8">Hello @servicestack/vue!</div>
        </ModalDialog>
    </div>`,
    setup() {
        const show = ref(false)
        return { show }
    }
}

/** Shared Components */
const Components = {
    HelloApi,
    GettingStarted,
    ShellCommand,
    Hello,
    Counter,
    Plugin,
    VueComponentGallery,
    VueComponentLibrary,
    PagingNav,
    FileLayout,
}
const CustomElements = [
    'lite-youtube'
]

const alreadyMounted = el => el.__vue_app__ 

/** Mount Vue3 Component
 * @param sel {string|Element} - Element or Selector where component should be mounted
 * @param component 
 * @param [props] {any} */
export function mount(sel, component, props) {
    if (!AppData.init) {
        init(globalThis)
    }
    const el = $1(sel)
    if (alreadyMounted(el)) return
    const app = createApp(component, props)
    app.provide('client', client)
    Object.keys(Components).forEach(name => {
        app.component(name, Components[name])
    })
    app.use(ServiceStackVue)
    app.component('RouterLink', ServiceStackVue.component('RouterLink'))
    app.directive('hash', (el,binding) => {
        /** @param {Event} e */
        el.onclick = (e) => {
            e.preventDefault()
            location.hash = binding.value
        }
    })
    if (component.install) {
        component.install(app)
    }
    if (client && !app._context.provides.client) {
        app.provide('client', client)
    }
    app.config.compilerOptions.isCustomElement = tag => CustomElements.includes(tag)
    app.mount(el)
    Apps.push(app)
    return app
}

export function mountAll() {
    $$('[data-component]').forEach(el => {
        if (alreadyMounted(el)) return
        let componentName = el.getAttribute('data-component')
        if (!componentName) return
        let component = Components[componentName] || ServiceStackVue.component(componentName)
        if (!component) {
            console.error(`Could not create component ${componentName}`)
            return
        }

        let propsStr = el.getAttribute('data-props')
        let props = propsStr && new Function(`return (${propsStr})`)() || {}
        mount(el, component, props)
    })
}

/** @param {any} [exports] */
export function init(exports) {
    if (AppData.init) return
    client = JsonApiClient.create()
    AppData = reactive(AppData)
    AppData.init = true
    mountAll()

    if (exports) {
        exports.client = client
        exports.Apps = Apps
    }
}
