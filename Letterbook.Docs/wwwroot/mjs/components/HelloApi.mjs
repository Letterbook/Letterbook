import { ref } from "vue"
import { useClient } from "@servicestack/vue"
import { Hello } from "../dtos.mjs"

export default {
    template:/*html*/`<div class="flex flex-wrap justify-center">
        <TextInput v-model="name" @keyup="update" />
        <div class="ml-3 mt-2 text-lg">{{ result }}</div>
    </div>`,
    props:['value'],
    setup(props) {
        let name = ref(props.value)
        let result = ref('')
        let client = useClient()

        async function update() {
            let api = await client.api(new Hello({ name }))
            if (api.succeeded) {
                result.value = api.response.result
            }
        }
        update()

        return { name, update, result }
    }
}
