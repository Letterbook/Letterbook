import { ref, computed } from "vue"
import ShellCommand from "./ShellCommand.mjs"

export default {
    components: {
        ShellCommand,
    },
    template:/*html*/`
    <div class="flex flex-col w-96">
        <h4 class="py-6 text-center text-xl">Create New Project</h4>

      <input type="text" v-model="project" autocomplete="off" spellcheck="false" @keydown="validateSafeName"
             class="mb-8 sm:text-lg rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 border-gray-300 dark:bg-gray-800"/>

        <section class="w-full flex justify-center text-center">
           <div class="mb-2">
              <div class="flex justify-center text-center">
                 <a class="archive-url hover:no-underline netcoretemplates_empty" :href="zipUrl('NetCoreTemplates/razor-press')">
                    <div class="bg-white dark:bg-gray-800 px-4 py-4 mr-4 mb-4 rounded-lg shadow-lg text-center items-center justify-center hover:shadow-2xl dark:border-2 dark:border-pink-600 dark:hover:border-blue-600 dark:border-2 dark:border-pink-600 dark:hover:border-blue-600" style="min-width:150px">
                       <div class="text-center font-extrabold flex items-center justify-center mb-2">
                          <div class="text-4xl text-blue-400 my-3">
                             <svg class="w-12 h-12 text-indigo-600" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 19a9 9 0 0 1 9 0a9 9 0 0 1 9 0M3 6a9 9 0 0 1 9 0a9 9 0 0 1 9 0M3 6v13m9-13v13m9-13v13"></path></svg>
                          </div>
                       </div>
                       <div class="text-xl font-medium text-gray-700">Razor Press</div>
                       <div class="flex justify-center h-8"></div>
                       <span class="archive-name px-4 pb-2 text-blue-600 dark:text-indigo-400">{{ projectZip }}</span>
                       <div class="count mt-1 text-gray-400 text-sm"></div>
                    </div>
                 </a>
              </div>
           </div>
        </section>

      <ShellCommand class="mb-2">dotnet tool install -g x</ShellCommand>
      <ShellCommand class="mb-2">x new {{template}} {{project}}</ShellCommand>

      <h4 class="py-6 text-center text-xl">In <span class="font-semibold text-indigo-700">/Letterbook.Docs</span>, Run Tailwind</h4>
      <ShellCommand class="mb-2">npm run ui:dev</ShellCommand>

      <h4 class="py-6 text-center text-xl">Run .NET Project (New Terminal)</h4>
      <ShellCommand class="mb-2">dotnet watch</ShellCommand>

    </div>`,
    props: { template:String },
    setup(props) {
        const project = ref('ProjectName')

        const projectZip = computed(() => (project.value || 'Letterbook.Docs') + '.zip')

        /** @param {string} template */
        const zipUrl = (template) =>
            `https://account.servicestack.net/archive/${template}?Name=${project.value || 'Letterbook.Docs'}`

        /** @param {KeyboardEvent} e */
        const isAlphaNumeric = (e) => {
            const c = e.charCode;
            if (c >= 65 && c <= 90 || c >= 97 && c <= 122 || c >= 48 && c <= 57 || c === 95) //A-Za-z0-9_
                return;
            e.preventDefault()
        }

        /** @param path {string}
          * @returns {string} */
        const resolvePath = (path) => navigator.userAgent.indexOf("Win") >= 0 ? path.replace(/\//g,'\\') : path
        const uiPath = () => resolvePath(`ui`)
        const apiPath = () => resolvePath(`api/${project.value}`)

        /** @param e {KeyboardEvent} */
        function validateSafeName(e) {
            if (e.key.match(/[\W]+/g)) {
                e.preventDefault()
                return false
            }
        }
        return { project, projectZip, zipUrl, isAlphaNumeric, uiPath, apiPath, validateSafeName }
    }
}