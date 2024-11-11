export default {
    template:/*html*/`
    <div class="not-prose">
        <div class="text-center">
            <h1 class="mt-2 text-4xl font-bold tracking-tight text-gray-900 dark:text-gray-100 sm:text-5xl">
                Vue Library
            </h1>
        </div>
        <div class="pt-4 pb-16">
            <div class="mx-auto">
                <NavList class="mt-8" title="">
                    <NavListItem title="useMetadata" href="https://docs.servicestack.net/vue/use-metadata" :iconSvg="Icons.Code">
                        Reflective utils for inspecting API AppMetadata
                    </NavListItem>
                    <NavListItem title="useClient" href="https://docs.servicestack.net/vue/use-client" :iconSvg="Icons.Code">
                        Utilize JSON Api Client features in Components
                    </NavListItem>
                    <NavListItem title="useAuth" href="https://docs.servicestack.net/vue/use-auth" :iconSvg="Icons.Code">
                        Inspect Authenticated Users Info, Roles &amp; Permissions
                    </NavListItem>
                    <NavListItem title="useFormatters" href="https://docs.servicestack.net/vue/use-formatters" :iconSvg="Icons.Code">
                        Built-in Formats and formatting functions
                    </NavListItem>
                    <NavListItem title="useFiles" href="https://docs.servicestack.net/vue/use-files" :iconSvg="Icons.Code">
                        File utils for resolving SVG icons, extensions and MIME types
                    </NavListItem>
                    <NavListItem title="useConfig" href="https://docs.servicestack.net/vue/use-config" :iconSvg="Icons.Code">
                        Manage global configuration &amp; defaults
                    </NavListItem>
                    <NavListItem title="useUtils" href="https://docs.servicestack.net/vue/use-utils" :iconSvg="Icons.Code">
                        General functionality and utils
                    </NavListItem>
                </NavList>
            </div>
        </div>
    </div>        
    `,
    setup() {
        const svg = (viewbox,body) =>
            `<svg class='h-6 w-6 text-indigo-700 dark:text-indigo-300' xmlns='http://www.w3.org/2000/svg' viewBox='${viewbox}'>${body}</svg>`

        const Icons = {
            Code: svg("0 0 24 24",     "<path fill='none' stroke='currentColor' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='m10 20l4-16m4 4l4 4l-4 4M6 16l-4-4l4-4'></path>"),
        }
        return { Icons }
    }
}
