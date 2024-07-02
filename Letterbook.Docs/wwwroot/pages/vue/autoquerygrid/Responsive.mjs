import { ref } from "vue"
import { useClient, useFormatters } from "@servicestack/vue"
import { QueryCoupons } from "../dtos.mjs"

export default {
    template:`<AutoQueryGrid type="Booking" :visibleFrom="{ name:'xl', bookingStartDate:'sm', bookingEndDate:'xl', createdBy:'2xl' }">
        <template #id="{ id }">
            <span class="text-gray-900">{{ id }}</span>
        </template>
        
        <template #name="{ name }">
            {{ name }}
        </template>
        
        <template #roomNumber-header>
            <span class="hidden lg:inline">Room </span>No
        </template>
    
        <template #cost="{ cost }">
            <span v-html="currency(cost)"></span>
        </template>
        
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
    </AutoQueryGrid>`,
    setup() {
        const client = useClient()
        const coupon = ref()
        const { currency } = useFormatters()

        /** @param {string} id */
        async function showCoupon(id) {
            const api = await client.api(new QueryCoupons({ id }))
            if (api.succeeded) {
                coupon.value = api.response.results[0]
            }
        }
        const close = () => coupon.value = null

        return { coupon, showCoupon, close, currency }
    }
}