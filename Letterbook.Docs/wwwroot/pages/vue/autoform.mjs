import { ref, computed, onMounted, provide } from "vue"
import { JsonApiClient } from "@servicestack/client"
import { useClient, useMetadata } from "@servicestack/vue"
import { QueryBookings, UpdateBooking } from "./dtos.mjs"
import { fetchBookings } from "./data.mjs"

export const Fields = {
    template:`<form v-if="request" @submit.prevent="submit">
        <div class="shadow sm:overflow-hidden sm:rounded-md">
            <div class="space-y-6 py-6 px-4 sm:p-6 bg-white dark:bg-black">
                <div>
                    <h3 class="text-lg font-medium leading-6 text-gray-900 dark:text-gray-100">
                        Change an existing Room Booking
                    </h3>
                    <p class="notes mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Find out how to quickly create a 
                        <a class='svg-external' target='_blank' href='https://youtu.be/rSFiikDjGos'>
                            C# Bookings App from Scratch
                        </a>
                    </p>
                </div>
    
                <AutoFormFields v-model="request" :api="api" />
    
            </div>
            <div class="bg-gray-50 dark:bg-gray-800 px-4 py-3 text-right sm:px-12">
                <PrimaryButton>Save</PrimaryButton>
            </div>
        </div>
    </form>`,
    setup() {
        const { toFormValues } = useMetadata()
        const client = useClient()

        /** @type {Ref<ApiResponse>} */
        let api = ref()
        /** @type {Ref<UpdateBooking>} */
        let request = ref()

        /** @param {Event} e */
        async function submit(e) {
            api.value = await client.api(request.value)
        }

        onMounted(async () => {
            let api = await client.api(new QueryBookings({ id: 1 }))
            if (api.succeeded) {
                request.value = new UpdateBooking(toFormValues(api.response.results[0]))
            }
        })
        
        return { api, request, submit }
    }
}

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: { Fields },
    setup() {
        const bookings = ref([])

        const booking = computed(() => bookings.value[0])
        const show = ref(false)

        const results = ref()
        const onSuccess = response => results.value = response.results

        const request = ref(new QueryBookings({ skip:1, take:2, orderBy:'Name' }))

        onMounted(async () => {
            bookings.value = await fetchBookings()
        })
        return { bookings, booking, show, results, onSuccess, request }
    }
}
