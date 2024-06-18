import { JsonApiClient } from "@servicestack/client"

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
    },
    setup() {
        const message = "Requires <b>Employee</b> Role"
        return { message }
    }
}
