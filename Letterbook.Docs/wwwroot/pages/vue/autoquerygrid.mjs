import { onMounted } from "vue"
import { Authenticate } from "./dtos.mjs"
import { useAuth, useClient } from '@servicestack/vue'
import { JsonApiClient } from "@servicestack/client"
import Responsive from "./autoquerygrid/Responsive.mjs"
import CustomBooking from "./autoquerygrid/CustomBooking.mjs"

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
        Responsive,
        CustomBooking,
    },
    setup() {
        const client = useClient()
        
        onMounted(async () => {
            const api = await client.api(new Authenticate({ 
                provider: 'credentials', userName:'admin@email.com', password:'p@55wOrd' }))
            if (api.succeeded) {
                const { signIn } = useAuth()
                signIn(api.response)
            }
        })
        return { }
    }
}
