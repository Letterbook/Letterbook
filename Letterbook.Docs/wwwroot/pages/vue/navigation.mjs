import { JsonApiClient } from "@servicestack/client"
import { Icons } from "./data.mjs"

export const NavListExamples = {
    template:`<div>
        <NavList title="Explore Vue Tailwind Components">
            <NavListItem title="DataGrid" href="/vue/gallery/datagrid" :iconSvg="Icons.DataGrid">
                DataGrid Component Examples for rendering tabular data
            </NavListItem>
            <NavListItem title="AutoQuery Grid" href="/vue/gallery/autoquerygrid" :iconSvg="Icons.AutoQueryGrid">
                Instant customizable UIs for calling AutoQuery CRUD APIs
            </NavListItem>
        </NavList>
    </div>`,
    setup() {
        return { Icons }
    }
}

const A = { template:`<h3 class="text-center text-2xl">A Tab Body</h3>` }
const B = { template:`<h3 class="text-center text-2xl">B Tab Body</h3>` }
const C = { template:`<h3 class="text-center text-2xl">C Tab Body</h3>` }

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
    },
    setup() {
        const say = msg => alert(msg)
        const tabs = { A, B, C }
        return { say, tabs }
    }
}
