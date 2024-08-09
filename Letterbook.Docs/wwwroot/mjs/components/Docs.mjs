export const PagingNav = {
    template:`
    <nav class="mt-8 mb-8 flex items-center justify-between border-t border-gray-200 dark:border-gray-700 px-4 sm:px-0">
        <div class="-mt-px flex w-0 flex-1">
            <a v-if="prevHref" :href="prevHref" class="inline-flex items-center border-t-2 border-transparent pt-4 pr-1 text-sm font-medium text-gray-500 hover:border-gray-300 dark:hover:border-gray-600 hover:text-gray-700 dark:hover:text-gray-200">
                <!-- Heroicon name: mini/arrow-long-left -->
                <svg class="mr-3 h-5 w-5 text-gray-500" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd" d="M18 10a.75.75 0 01-.75.75H4.66l2.1 1.95a.75.75 0 11-1.02 1.1l-3.5-3.25a.75.75 0 010-1.1l3.5-3.25a.75.75 0 111.02 1.1l-2.1 1.95h12.59A.75.75 0 0118 10z" clip-rule="evenodd"></path>
                </svg>
                {{prevLabel ?? 'Previous'}}
            </a>
        </div>
        <div class="hidden md:-mt-px md:flex"></div>
        <div class="-mt-px flex w-0 flex-1 justify-end">
            <a v-if="nextHref" :href="nextHref" class="inline-flex items-center border-t-2 border-transparent pt-4 pl-1 text-sm font-medium text-gray-500 hover:border-gray-300 dark:hover:border-gray-600 hover:text-gray-700 dark:hover:text-gray-200">
                {{nextLabel ?? 'Next'}}                    
                <!-- Heroicon name: mini/arrow-long-right -->
                <svg class="ml-3 h-5 w-5 text-gray-500" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd" d="M2 10a.75.75 0 01.75-.75h12.59l-2.1-1.95a.75.75 0 111.02-1.1l3.5 3.25a.75.75 0 010 1.1l-3.5 3.25a.75.75 0 11-1.02-1.1l2.1-1.95H2.75A.75.75 0 012 10z" clip-rule="evenodd"></path>
                </svg>
            </a>
        </div>
    </nav>    
    `,
    props:['prevHref', 'prevLabel', 'nextHref', 'nextLabel']
}

/**
 files: {
        emails: { 
            layouts:  { _: ['basic.html','empty.html','marketing.html'] },
            partials: { _: ['button-centered.html','divider.html','image-centered.html','section.html','title.html'] },
            vars:     { _: ['info.txt','urls.txt'] },
            _:        ['empty.html','newsletter-welcome.html','newsletter.html','verify-email.html'] 
        }
    }
 */
const File = {
    template:`
        <div>
            <div v-if="label == '_'">
                <div v-for="file in contents" class="ml-6 flex items-center text-base leading-8">
                    <svg class="mr-1 text-slate-600" aria-hidden="true" focusable="false" role="img" viewBox="0 0 16 16" width="16" height="16" fill="currentColor" style="display: inline-block; user-select: none; vertical-align: text-bottom; overflow: visible;"><path d="M2 1.75C2 .784 2.784 0 3.75 0h6.586c.464 0 .909.184 1.237.513l2.914 2.914c.329.328.513.773.513 1.237v9.586A1.75 1.75 0 0 1 13.25 16h-9.5A1.75 1.75 0 0 1 2 14.25Zm1.75-.25a.25.25 0 0 0-.25.25v12.5c0 .138.112.25.25.25h9.5a.25.25 0 0 0 .25-.25V6h-2.75A1.75 1.75 0 0 1 9 4.25V1.5Zm6.75.062V4.25c0 .138.112.25.25.25h2.688l-.011-.013-2.914-2.914-.013-.011Z"></path></svg>
                    <span>{{file}}</span>
                </div>
            </div>
            <div v-else>
                <div class="ml-6">
                    <div class="flex items-center text-base leading-8">
                        <svg class="mr-1 text-slate-600" aria-hidden="true" focusable="false" role="img" viewBox="0 0 12 12" width="12" height="12" fill="currentColor" style="display: inline-block; user-select: none; vertical-align: text-bottom; overflow: visible;"><path d="M6 8.825c-.2 0-.4-.1-.5-.2l-3.3-3.3c-.3-.3-.3-.8 0-1.1.3-.3.8-.3 1.1 0l2.7 2.7 2.7-2.7c.3-.3.8-.3 1.1 0 .3.3.3.8 0 1.1l-3.2 3.2c-.2.2-.4.3-.6.3Z"></path></svg>
                        <svg class="mr-1 text-sky-500" aria-hidden="true" focusable="false" role="img" viewBox="0 0 16 16" width="16" height="16" fill="currentColor" style="display: inline-block; user-select: none; vertical-align: text-bottom; overflow: visible;"><path d="M.513 1.513A1.75 1.75 0 0 1 1.75 1h3.5c.55 0 1.07.26 1.4.7l.9 1.2a.25.25 0 0 0 .2.1H13a1 1 0 0 1 1 1v.5H2.75a.75.75 0 0 0 0 1.5h11.978a1 1 0 0 1 .994 1.117L15 13.25A1.75 1.75 0 0 1 13.25 15H1.75A1.75 1.75 0 0 1 0 13.25V2.75c0-.464.184-.91.513-1.237Z"></path></svg>
                        <span>{{label}}</span>
                    </div>
                    <File v-for="(children,item) in contents" :label="item" :contents="children" />
                </div>
            </div>
        </div>
    `,
    props:['label','contents','depth'],
    setup(props) {
        return { }
    }
}
File.components = { File }

export const FileLayout = {
    components: { File },
    template: `<div>
        <File v-for="(contents,label) in files" :label="label" :contents="contents" />
    </div>`,
    props: ['files']
}