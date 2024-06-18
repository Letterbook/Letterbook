export default {
    template:/*html*/`
    <div class="not-prose">
        <div class="mt-16 mx-auto flex justify-center">
            <div class="flex items-center">
                <svg xmlns="http://www.w3.org/2000/svg" aria-hidden="true" role="img" class="w-32 h-32 sm:w-44 sm:h-44 text-gray-700" viewBox="0 0 32 32"><path fill="currentColor" d="M10 6c1.544 1.76 2.276 4.15 2.217 6.61c3.968 1.67 9.924 6.12 11.181 12.39H28C26.051 14.31 14.918 6.77 10 6zm-2 7c4.67 4.913.81 11.582-4 12h18.97C21.5 18.289 11.95 13.533 8 13z"></path></svg>
                <svg xmlns="http://www.w3.org/2000/svg" aria-hidden="true" role="img" class="w-32 h-32 sm:w-44 sm:h-44" viewBox="0 0 32 32"><path fill="#41b883" d="M24.4 3.925H30l-14 24.15L2 3.925h10.71l3.29 5.6l3.22-5.6Z"></path><path fill="#41b883" d="m2 3.925l14 24.15l14-24.15h-5.6L16 18.415L7.53 3.925Z"></path><path fill="#35495e" d="M7.53 3.925L16 18.485l8.4-14.56h-5.18L16 9.525l-3.29-5.6Z"></path></svg>
                <svg xmlns="http://www.w3.org/2000/svg" aria-hidden="true" role="img" class="w-32 h-32 sm:w-44 sm:h-44" viewBox="0 0 32 32"><path fill="#44a8b3" d="M9 13.7q1.4-5.6 7-5.6c5.6 0 6.3 4.2 9.1 4.9q2.8.7 4.9-2.1q-1.4 5.6-7 5.6c-5.6 0-6.3-4.2-9.1-4.9q-2.8-.7-4.9 2.1Zm-7 8.4q1.4-5.6 7-5.6c5.6 0 6.3 4.2 9.1 4.9q2.8.7 4.9-2.1q-1.4 5.6-7 5.6c-5.6 0-6.3-4.2-9.1-4.9q-2.8-.7-4.9 2.1Z"></path></svg>
            </div>
        </div>
    
        <div class="text-center">
            <h1 class="mt-2 text-4xl font-bold tracking-tight text-gray-900 dark:text-gray-100 sm:text-5xl">
                Component Gallery
            </h1>
            <p class="mt-2 text-lg text-gray-500 dark:text-gray-400">
                Explore Vue Tailwind Component Gallery
            </p>
        </div>
        <div class="pt-4 pb-16">
            <div class="mx-auto">
                <NavList title="">
                    <NavListItem title="AutoQueryGrid" href="https://docs.servicestack.net/vue/autoquerygrid" :iconSvg="Icons.AutoQueryGrid">
                        Instant customizable UIs for calling AutoQuery CRUD APIs
                    </NavListItem>
                    <NavListItem title="DataGrid" href="https://docs.servicestack.net/vue/datagrid" :iconSvg="Icons.DataGrid">
                        DataGrid Component Examples for rendering tabular data
                    </NavListItem>
                    <NavListItem title="Auto Forms" href="https://docs.servicestack.net/vue/autoform" :iconSvg="Icons.AutoForms">
                        Render Auto Form UIs from a Request DTO class
                    </NavListItem>
                    <NavListItem title="Form Inputs" href="https://docs.servicestack.net/vue/form-inputs" :iconSvg="Icons.FormInputs">
                        Tailwind UI Input Components
                    </NavListItem>
                    <NavListItem title="Markdown Editor" href="https://docs.servicestack.net/vue/markdown" :iconSvg="Icons.Markdown">
                        Rich Markdown Editing Input Control
                    </NavListItem>
                    <NavListItem title="Modals" href="https://docs.servicestack.net/vue/modals" :iconSvg="Icons.Modals">
                        Modal Dialogs and Slide Overs
                    </NavListItem>
                    <NavListItem title="Navigation" href="https://docs.servicestack.net/vue/navigation" :iconSvg="Icons.Navigation">
                        Breadcrumbs and Link navigation components
                    </NavListItem>
                    <NavListItem title="Alerts" href="https://docs.servicestack.net/vue/alerts" :iconSvg="Icons.Alerts">
                        Tailwind Alert and Notification components
                    </NavListItem>
                    <NavListItem title="Formats" href="https://docs.servicestack.net/vue/formats" :iconSvg="Icons.Formats">
                        HTML Value Formatters
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
            AutoQueryGrid: svg("0 0 28 28",     "<path fill='currentColor' d='M3 6.75A3.75 3.75 0 0 1 6.75 3h14.5A3.75 3.75 0 0 1 25 6.75v6.262a3.296 3.296 0 0 0-1.5.22V11h-5v6h.856L17 19.356V18.5h-6v5h2.542a3.329 3.329 0 0 0-.02.077L13.166 25H6.75A3.75 3.75 0 0 1 3 21.25V6.75ZM4.5 18.5v2.75a2.25 2.25 0 0 0 2.25 2.25H9.5v-5h-5Zm5-1.5v-6h-5v6h5Zm7.5 0v-6h-6v6h6Zm6.5-10.25a2.25 2.25 0 0 0-2.25-2.25H18.5v5h5V6.75ZM17 4.5h-6v5h6v-5Zm-7.5 0H6.75A2.25 2.25 0 0 0 4.5 6.75V9.5h5v-5Zm13.6 10.17l-7.903 7.902a2.686 2.686 0 0 0-.706 1.247l-.458 1.831a1.087 1.087 0 0 0 1.319 1.318l1.83-.457a2.685 2.685 0 0 0 1.248-.707l7.902-7.902a2.286 2.286 0 0 0-3.232-3.233Z' />"),
            DataGrid:      svg("0 0 16 16",     "<path fill='currentColor' d='M0 1v14h16V1H0zm6 9V7h4v3H6zm4 1v3H6v-3h4zm0-8v3H6V3h4zM5 3v3H1V3h4zM1 7h4v3H1V7zm10 0h4v3h-4V7zm0-1V3h4v3h-4zM1 11h4v3H1v-3zm10 3v-3h4v3h-4z' />"),
            AutoForms:     svg("0 0 1024 1024", "<path fill='currentColor' d='M904 512h-56c-4.4 0-8 3.6-8 8v320H184V184h320c4.4 0 8-3.6 8-8v-56c0-4.4-3.6-8-8-8H144c-17.7 0-32 14.3-32 32v736c0 17.7 14.3 32 32 32h736c17.7 0 32-14.3 32-32V520c0-4.4-3.6-8-8-8z' /><path fill='currentColor' d='M355.9 534.9L354 653.8c-.1 8.9 7.1 16.2 16 16.2h.4l118-2.9c2-.1 4-.9 5.4-2.3l415.9-415c3.1-3.1 3.1-8.2 0-11.3L785.4 114.3c-1.6-1.6-3.6-2.3-5.7-2.3s-4.1.8-5.7 2.3l-415.8 415a8.3 8.3 0 0 0-2.3 5.6zm63.5 23.6L779.7 199l45.2 45.1l-360.5 359.7l-45.7 1.1l.7-46.4z' />"),
            FormInputs:    svg("0 0 36 36",     "<path fill='currentColor' d='M21 12H7a1 1 0 0 1-1-1V7a1 1 0 0 1 1-1h14a1 1 0 0 1 1 1v4a1 1 0 0 1-1 1ZM8 10h12V7.94H8Z' class='clr-i-outline clr-i-outline-path-1' /><path fill='currentColor' d='M21 14.08H7a1 1 0 0 0-1 1V19a1 1 0 0 0 1 1h11.36L22 16.3v-1.22a1 1 0 0 0-1-1ZM20 18H8v-2h12Z' class='clr-i-outline clr-i-outline-path-2' /><path fill='currentColor' d='M11.06 31.51v-.06l.32-1.39H4V4h20v10.25l2-1.89V3a1 1 0 0 0-1-1H3a1 1 0 0 0-1 1v28a1 1 0 0 0 1 1h8a3.44 3.44 0 0 1 .06-.49Z' class='clr-i-outline clr-i-outline-path-3' /><path fill='currentColor' d='m22 19.17l-.78.79a1 1 0 0 0 .78-.79Z' class='clr-i-outline clr-i-outline-path-4' /><path fill='currentColor' d='M6 26.94a1 1 0 0 0 1 1h4.84l.3-1.3l.13-.55v-.05H8V24h6.34l2-2H7a1 1 0 0 0-1 1Z' class='clr-i-outline clr-i-outline-path-5' /><path fill='currentColor' d='m33.49 16.67l-3.37-3.37a1.61 1.61 0 0 0-2.28 0L14.13 27.09L13 31.9a1.61 1.61 0 0 0 1.26 1.9a1.55 1.55 0 0 0 .31 0a1.15 1.15 0 0 0 .37 0l4.85-1.07L33.49 19a1.6 1.6 0 0 0 0-2.27ZM18.77 30.91l-3.66.81l.89-3.63L26.28 17.7l2.82 2.82Zm11.46-11.52l-2.82-2.82L29 15l2.84 2.84Z' class='clr-i-outline clr-i-outline-path-6' /><path fill='none' d='M0 0h36v36H0z' />"),
            Markdown:      svg("0 0 15 15",     "<path fill='currentColor' d='m2.5 5.5l.354-.354A.5.5 0 0 0 2 5.5h.5Zm2 2l-.354.354l.354.353l.354-.353L4.5 7.5Zm2-2H7a.5.5 0 0 0-.854-.354L6.5 5.5Zm4 4l-.354.354l.354.353l.354-.353L10.5 9.5ZM1.5 3h12V2h-12v1Zm12.5.5v8h1v-8h-1Zm-.5 8.5h-12v1h12v-1ZM1 11.5v-8H0v8h1Zm.5.5a.5.5 0 0 1-.5-.5H0A1.5 1.5 0 0 0 1.5 13v-1Zm12.5-.5a.5.5 0 0 1-.5.5v1a1.5 1.5 0 0 0 1.5-1.5h-1ZM13.5 3a.5.5 0 0 1 .5.5h1A1.5 1.5 0 0 0 13.5 2v1Zm-12-1A1.5 1.5 0 0 0 0 3.5h1a.5.5 0 0 1 .5-.5V2ZM3 10V5.5H2V10h1Zm-.854-4.146l2 2l.708-.708l-2-2l-.708.708Zm2.708 2l2-2l-.708-.708l-2 2l.708.708ZM6 5.5V10h1V5.5H6Zm4-.5v4.5h1V5h-1ZM8.146 7.854l2 2l.708-.708l-2-2l-.708.708Zm2.708 2l2-2l-.708-.708l-2 2l.708.708Z'/>"),
            Modals:        svg("0 0 32 32",     "<path fill='currentColor' d='M28 4H10a2.006 2.006 0 0 0-2 2v14a2.006 2.006 0 0 0 2 2h18a2.006 2.006 0 0 0 2-2V6a2.006 2.006 0 0 0-2-2Zm0 16H10V6h18Z'/><path fill='currentColor' d='M18 26H4V16h2v-2H4a2.006 2.006 0 0 0-2 2v10a2.006 2.006 0 0 0 2 2h14a2.006 2.006 0 0 0 2-2v-2h-2Z'/>"),
            Navigation:    svg("0 0 12 12",     "<path fill='currentColor' d='M1 3a2 2 0 0 1 2-2h3a2 2 0 0 1 2 2v3a2 2 0 0 1-2 2V7a1 1 0 0 0 1-1V3a1 1 0 0 0-1-1H3a1 1 0 0 0-1 1v3a1 1 0 0 0 1 1v1a2 2 0 0 1-2-2V3Zm3 6a2 2 0 0 0 2 2h3a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2v1a1 1 0 0 1 1 1v3a1 1 0 0 1-1 1H6a1 1 0 0 1-1-1V6a1 1 0 0 1 1-1V4a2 2 0 0 0-2 2v3Z' />"),
            Alerts:        svg("0 0 16 16",     "<path fill='currentColor' d='M8 2a4.5 4.5 0 0 0-4.5 4.5v2.401l-.964 2.414A.5.5 0 0 0 3 12h3c0 1.108.892 2 2 2s2-.892 2-2h3a.5.5 0 0 0 .464-.685L12.5 8.9V7a2.49 2.49 0 0 1-1-.208v2.206a.5.5 0 0 0 .036.185L12.262 11H3.738l.726-1.817a.5.5 0 0 0 .036-.185V6.5a3.5 3.5 0 0 1 5.625-2.782c.107-.325.279-.622.501-.873A4.48 4.48 0 0 0 8 2Zm1 10c0 .556-.444 1-1 1s-1-.444-1-1h2Zm2.368-8.484a1.494 1.494 0 0 0-.351 1.208A1.497 1.497 0 0 0 12.5 6a1.5 1.5 0 1 0-1.132-2.484Z' />"),
            Formats:       svg("0 0 1024 1024", "<path fill='currentColor' d='M840 192h-56v-72c0-13.3-10.7-24-24-24H168c-13.3 0-24 10.7-24 24v272c0 13.3 10.7 24 24 24h592c13.3 0 24-10.7 24-24V256h32v200H465c-22.1 0-40 17.9-40 40v136h-44c-4.4 0-8 3.6-8 8v228c0 .6.1 1.3.2 1.9c-.1 2-.2 4.1-.2 6.1c0 46.4 37.6 84 84 84s84-37.6 84-84c0-2.1-.1-4.1-.2-6.1c.1-.6.2-1.2.2-1.9V640c0-4.4-3.6-8-8-8h-44V520h351c22.1 0 40-17.9 40-40V232c0-22.1-17.9-40-40-40zM720 352H208V160h512v192zM477 876c0 11-9 20-20 20s-20-9-20-20V696h40v180z' />"),
            Code:          svg("0 0 24 24",     "<path fill='none' stroke='currentColor' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='m10 20l4-16m4 4l4 4l-4 4M6 16l-4-4l4-4'></path>"),
        }
        return { Icons }
    }
}
