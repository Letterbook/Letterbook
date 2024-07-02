import { ref } from "vue"
import { JsonApiClient } from "@servicestack/client"
import { allContacts } from "./data.mjs"

export const AutocompleteExamples = {
    template:`<div class="p-4 shadow sm:rounded-md">
        <div class="max-w-xl grid grid-cols-6 gap-6">
            <form class="col-span-12">
                <div class="mb-3">
                    <Autocomplete id="simple" :options="allContacts" v-model="simple" label="Single Contact"
                        :match="(x,value) => x.displayName.toLowerCase().includes(value.toLowerCase())"
                        placeholder="Select Contact">
                        <template #item="{ displayName }">
                            <span class="block truncate">{{ displayName }}</span>
                        </template>
                    </Autocomplete>
                    <div class="mt-2 flex justify-end">
                        <p>
                            <b class="text-gray-500">Single:</b>
                            <div v-if="simple" class="flex">
                                <img :src="simple.profileUrl" class="w-8 h-8 rounded-full mr-2">
                                <b class="text-lg">{{ simple.displayName }}</b>
                            </div>
                        </p>
                    </div>
                </div>
    
                <div class="mb-3">
                    <Autocomplete id="contact" :options="allContacts" v-model="contact" label="Single Contact with Icon"
                        :match="(x,value) => x.displayName.toLowerCase().includes(value.toLowerCase())"
                        placeholder="Select Contact">
                        <template #item="{ displayName, profileUrl }">
                            <div class="flex items-center">
                                <Icon class="h-6 w-6 flex-shrink-0 rounded-full" :src="profileUrl" loading="lazy" />
                                <span class="ml-3 truncate">{{ displayName }}</span>
                            </div>
                        </template>
                    </Autocomplete>
                    <div class="mt-2 flex justify-end">
                        <p>
                            <b class="text-gray-500">Single with Icon:</b>
                            <div v-if="contact" class="flex">
                                <img :src="contact.profileUrl" class="w-8 h-8 rounded-full mr-2">
                                <b class="text-lg">{{ contact.displayName }}</b>
                            </div>
                        </p>
                    </div>
                </div>
    
                <div class="mb-3">
                    <Autocomplete id="contacts" :options="allContacts" v-model="contacts" multiple
                        label="Multiple Contacts with Icon"
                        :match="(x,value) => x.displayName.toLowerCase().includes(value.toLowerCase())"
                        placeholder="Select Contact">
                        <template #item="{ displayName, profileUrl }">
                            <div class="flex items-center">
                                <Icon class="h-6 w-6 flex-shrink-0 rounded-full" :src="profileUrl" loading="lazy" />
                                <span class="ml-3 truncate">{{ displayName }}</span>
                            </div>
                        </template>
                    </Autocomplete>
                    <div class="mt-2">
                        <div class="text-right"><b class="text-gray-500">Multiple with Icon:</b></div>
                        <p>
                        <div v-if="contacts.length" class="flex flex-wrap">
                            <div v-for="contact in contacts" class="flex ml-4 mb-2">
                                <img :src="contact.profileUrl" class="w-6 h-6 rounded-full mr-2">
                                <span>{{ contact.displayName }}</span>
                            </div>
                        </div>
                        </p>
                    </div>
                </div>
            </form>
        </div>
    </div>`,
    setup() {
        const simple = ref(null)
        const contact = ref(null)
        const contacts = ref([])
        return { allContacts, simple, contact, contacts }
    }
}

export default {
    install(app) {
        app.provide('client', JsonApiClient.create('https://blazor-gallery-api.jamstacks.net'))
    },
    components: {
        AutocompleteExamples,
    },
    setup() {
        const contact = allContacts[0]
        return { allContacts, contact }
    }
}
