import { ref } from "vue"
import { JsonApiClient } from "@servicestack/client"
import { QueryBookings } from "./dtos.mjs"
import { allContacts, files } from "./data.mjs"

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
    },
    setup() {
        const contact = allContacts[0]
        const show = ref(false)

        const results = ref()
        const onSuccess = response => results.value = response.results

        const request = ref(new QueryBookings({ skip:1, take:2, orderBy:'Name' }))
        return { allContacts, files, contact, show, results, onSuccess, request }
    }
}
