import { ref } from "vue"
import { useAuth } from "@servicestack/vue"
import { JsonApiClient } from "@servicestack/client"

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
    },
    setup() {
        const showDialog = ref(false)
        const showSlide = ref(false)
        const { signIn, user } = useAuth()

        return { showDialog, showSlide, signIn, user }
    }
}
