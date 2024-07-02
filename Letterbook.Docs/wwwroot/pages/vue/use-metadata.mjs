import { ref } from "vue"
import { useMetadata } from "@servicestack/vue"
import { JsonApiClient } from "@servicestack/client"

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
    },
    setup() {
        const { typeOf } = useMetadata()
        return { typeOf }
    }
}
