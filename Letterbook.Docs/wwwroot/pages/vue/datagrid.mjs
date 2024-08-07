//import Formatters from '../../.vitepress/includes/vue/formatters.md'

import { ref, provide, inject, onMounted } from "vue"
import { Authenticate, Booking } from "./dtos.mjs"
import { fetchBookings, tracks } from "./data.mjs"
import { useAuth, useClient, useFormatters } from '@servicestack/vue'
import { JsonApiClient } from "@servicestack/client"
import { forecasts } from './data.mjs'

export const Default = {
    template:`<DataGrid :items="bookings" />`,
    setup() {
        const bookings = inject('bookings')
        return { bookings }
    }
}

export const Custom = {
    template:`<DataGrid :items="forecasts" class="max-w-screen-md" :tableStyle="['stripedRows','uppercaseHeadings']"
              :header-titles="{ temperatureC:'TEMP. (C)', temperatureF:'TEMP. (F)' }">
        <template #date-header>
            <span class="text-indigo-600">Date</span>
        </template>
        <template #date="{ date }">
            {{ new Intl.DateTimeFormat().format(new Date(date)) }}
        </template>
        <template #temperatureC="{ temperatureC }">
            {{ temperatureC }}&deg;
        </template>
        <template #temperatureF="{ temperatureF }">
            {{ temperatureF }}&deg;
        </template>
        <template #summary="{ summary }">{{ summary }}</template>
    </DataGrid>`,
    setup() {
        return { forecasts }
    }
}

export const Responsive = {
    template:`<DataGrid :items="bookings" 
          :visible-from="{ name:'xl', bookingStartDate:'sm', bookingEndDate:'xl' }"
          @header-selected="headerSelected"
          @row-selected="rowSelected" :is-selected="row => selected == row.id">
        <template #id="{ id }">
            <span class="text-gray-900">{{ id }}</span>
        </template>
        
        <template #name="{ name }">
            {{ name }}
        </template>
        
        <template #roomNumber-header>
            <span class="hidden lg:inline">Room </span>No
        </template>
    
        <template #cost="{ cost }">{{ currency(cost) }}</template>
        
        <template #bookingStartDate-header>
            Start<span class="hidden lg:inline"> Date</span>
        </template>
        
        <template #bookingEndDate-header>
            End<span class="hidden lg:inline"> Date</span>
        </template>
    
        <template #createdBy-header>
            Employee
        </template>
        <template #createdBy="{ createdBy }">{{ createdBy }}</template>
    </DataGrid>`,
    setup() {
        const bookings = inject('bookings')
        const { currency } = useFormatters()
        const selected = ref()

        /** @param {string} column */
        function headerSelected(column) {
            console.log('headerSelected',column)
        }
        
        /** @param {Booking} row */
        function rowSelected(row) {
            selected.value = selected.value === row.id ? null : row.id
            console.log('rowSelected', row)
        }
        
        return { bookings, currency, selected, headerSelected, rowSelected }
    }
}

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
        Default,
        Custom,
        Responsive,
    },
    setup() {
        const bookings = ref([])
        const client = useClient()
        provide('bookings', bookings)

        onMounted(async () => {
            bookings.value = await fetchBookings()
            const api = await client.api(new Authenticate({ provider: 'credentials', userName:'admin@email.com', password:'p@55wOrd' }))
            if (api.succeeded) {
                const { signIn } = useAuth()
                signIn(api.response)
            }
        })
        return { tracks, bookings }
    }
}
