---
title: Form Inputs Components
group: Component Gallery
---

<api-reference component="TextInput"></api-reference>
## Bookings Form

The `TextInput`, `SelectInput`, `CheckboxInput` and `TextAreaInput` contains the most popular
Input controls used by C# POCOs which can be bound directly to Request DTOs and includes support for
[declarative](/declarative-validation) and [Fluent Validation](/validation) binding.

```html
<form @submit.prevent="submit">
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
</form>
```

<bookings-form :id="1" class="not-prose mb-4"></bookings-form>

Which can be wired up to handle querying, updating and deleting including limiting functionality to authorized users with:

```html
<script setup lang="ts">
import { computed, ref, watchEffect } from 'vue'
import { useMetadata, useAuth, useClient } from '@servicestack/vue'
import { DeleteBooking, QueryBookings, UpdateBooking } from '../dtos'

const props = defineProps<{
    id: number
}>()
const emit = defineEmits<{
    (e: 'done'): void
}>()

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

async function submit(e:Event) {
    const api = await client.api(request.value)
    if (api.succeeded) close()
}
async function onDelete() {
    const api = await client.apiVoid(new DeleteBooking({ id: props.id }))
    if (api.succeeded) close()
}

const close = () => emit('done')
</script>
```

This also shows how we can utilize `enumOptions` from our [App Metadata](/vue/use-metadata) to populate select drop downs from C# enums.
