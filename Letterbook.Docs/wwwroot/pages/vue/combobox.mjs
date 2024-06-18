import { ref } from "vue"
import { JsonApiClient } from "@servicestack/client"

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    setup() {
        const strings = ref()
        const objects = ref()
        const pairs = ref([])

        return { strings, objects, pairs }
    }
}
