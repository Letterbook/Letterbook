import { ref, computed, onMounted, provide, watchEffect } from "vue"
import { JsonApiClient } from "@servicestack/client"
import { useAuth, useClient, useMetadata } from "@servicestack/vue"
import { DeleteBooking, QueryBookings, UpdateBooking, UpdateContact } from "./dtos.mjs"
import { tracks, allContacts, files, fetchBookings } from "./data.mjs"


export const BookingsForm = {
    template: `<form @submit.prevent="submit">
        <div class="shadow sm:overflow-hidden sm:rounded-md">
            <div class="space-y-6 py-6 px-4 sm:p-6 bg-white dark:bg-black">
                <div>
                    <h3 class="text-lg font-medium leading-6 text-gray-900 dark:text-gray-100">
                        Update an existing Booking
                    </h3>
                </div>
                <fieldset>
                    <ErrorSummary :except="visibleFields" class="mb-4" />
                    <div class="grid grid-cols-6 gap-6">
                        <div class="col-span-6 sm:col-span-3">
                            <TextInput id="name" v-model="request.name" required placeholder="Name for this booking" />
                        </div>
                        <div class="col-span-6 sm:col-span-3">
                            <SelectInput id="roomType" v-model="request.roomType" :options="enumOptions('RoomType')" />
                        </div>
                        <div class="col-span-6 sm:col-span-3">
                            <TextInput type="number" id="roomNumber" v-model="request.roomNumber" min="0" required />
                        </div>
                        <div class="col-span-6 sm:col-span-3">
                            <TextInput type="number" id="cost" v-model="request.cost" min="0" required />
                        </div>
                        <div class="col-span-6 sm:col-span-3">
                            <TextInput type="date" id="bookingStartDate" v-model="request.bookingStartDate" required />
                        </div>
                        <div class="col-span-6 sm:col-span-3">
                            <TextInput type="date" id="bookingEndDate" v-model="request.bookingEndDate" />
                        </div>
                        <div class="col-span-6">
                            <TextareaInput id="notes" v-model="request.notes" placeholder="Notes about this booking" class="h-24" />
                        </div>
                    </div>
                </fieldset>
            </div>
            <div class="mt-4 bg-gray-50 dark:bg-gray-800 px-4 py-3 text-right sm:px-12">
                <div class="flex justify-between space-x-3">
                    <div>
                        <ConfirmDelete v-if="canDelete" @delete="onDelete">Delete</ConfirmDelete>
                    </div>
                    <div>
                        <PrimaryButton @click="submit">Update Booking</PrimaryButton>
                    </div>
                </div>
            </div>
        </div>
    </form>`,
    emits: ['done'],
    props: { id: Number },
    setup(props, { emit }) {
        const { enumOptions, toFormValues } = useMetadata()

        const visibleFields = "name,roomType,roomNumber,bookingStartDate,bookingEndDate,cost,notes"

        const { hasRole, user, isAuthenticated } = useAuth()
        const canDelete = computed(() => hasRole('Manager'))
        const client = useClient()
        const request = ref(new UpdateBooking())

        watchEffect(async () => {
            const api = await client.api(new QueryBookings({ id: props.id }))
            if (api.succeeded) {
                request.value = new UpdateBooking(toFormValues(api.response?.results[0]))
            }
        })

        /** @param {Event} e */
        async function submit(e) {
            const api = await client.api(request.value)
            if (api.succeeded) close()
        }
        async function onDelete() {
            const api = await client.apiVoid(new DeleteBooking({ id: props.id }))
            if (api.succeeded) close()
        }

        const close = () => emit('done')
        
        return { visibleFields, request, enumOptions, submit, canDelete, onDelete, close, }
    }
}

/*
export const TagInputExamples = {
    template:`<form @submit.prevent="submit">
        <div class="shadow sm:rounded-md bg-white dark:bg-black">
            <div class="relative px-4 py-5 sm:p-6">
                <fieldset>
                    <legend class="text-base font-medium text-gray-900 dark:text-gray-100 text-center mb-4">
                        TagInput Examples
                    </legend>
    
                    <ErrorSummary :except="visibleFields" />
    
                    <div class="grid grid-cols-12 gap-6">
                        <div class="col-span-6">
                            <TextInput v-model="request.firstName" />
                        </div>
    
                        <div class="col-span-6">
                            <TextInput v-model="request.lastName" />
                        </div>
    
                        <div class="col-span-12">
                            <TagInput v-model="request.skills" label="Technology Skills" />
                        </div>
    
                    </div>
                </fieldset>
            </div>
            <div class="mt-4 px-4 py-3 bg-gray-50 dark:bg-gray-900 sm:px-6 flex flex-wrap justify-between">
                <div></div>
                <div class="flex justify-end">
                    <SecondaryButton class="mr-4">Cancel</SecondaryButton>
                    <PrimaryButton type="submit">Submit</PrimaryButton>
                </div>
            </div>
        </div>
    </form>`,
    setup() {
        const visibleFields ="firstName,lastName,skills"
        const request = new UpdateContact(allContacts[0])
        function submit(e) {}
        return { allContacts, visibleFields, request, submit }
    }
}
*/

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
        //AutocompleteExamples,
        BookingsForm,
        //TagInputExamples,
    },
    setup() {
        const bookings = ref([])
        const contact = allContacts[0]
        const booking = computed(() => bookings.value[0])
        const show = ref(false)

        const results = ref()
        const onSuccess = response => results.value = response.results

        const request = ref(new QueryBookings({ skip:1, take:2, orderBy:'Name' }))

        onMounted(async () => {
            bookings.value = await fetchBookings()
        })
        return { tracks, bookings, booking, allContacts, contact, files, show, results, onSuccess, request }
    }
}
