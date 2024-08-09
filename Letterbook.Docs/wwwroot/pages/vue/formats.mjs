import { JsonApiClient } from "@servicestack/client"
import { useFormatters } from "@servicestack/vue"
import { tracks, players } from "./data.mjs"

//import Formatters from '../../.vitepress/includes/vue/formatters.md'

function classes(type, tag,depth,cls,index) {
    if (type === 'array') {
        if (tag === 'th') return cls.replace('text-sm text-gray-500 font-medium',' ') + (depth === 0
            ? 'text-xs uppercase font-semibold text-indigo-700'
            : 'text-xs font-medium text-gray-500')
        if (tag === 'tr') return depth > 0 || index % 2 === 0 ? 'bg-white' : 'bg-yellow-50'
        if (tag === 'td' && depth > 0) return 'px-4 py-3 whitespace-nowrap text-xs'
    }
    return cls
}

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
    },
    setup() {
        const { Formats } = useFormatters()
        const message = "Requires <b>Employee</b> Role"
        return { classes, tracks, players, Formats, message }
    }
}
