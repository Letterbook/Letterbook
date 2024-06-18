import { JsonApiClient } from "@servicestack/client"
import { UpdateContact } from "./dtos.mjs"
import { allContacts, files } from "./data.mjs"

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
    },
    setup() {
        const contact = allContacts[0]
        const visibleFields ="firstName,lastName,skills"
        const request = new UpdateContact(allContacts[0])
        function submit(e) {}

        return { allContacts, files, contact, visibleFields, request, submit }
    }
}
