import { $$, $1, on, leftPart, map } from "@servicestack/client"
//change #hash on scroll
const headings = $$('h2[id],h3[id]')

function select(id) {
    $$(`.docmap .active`).forEach(el => el.classList.remove('active'))
    map($1(`.docmap [data-id='${id}']`), el => {
        el.classList.add('active')
        map(el.closest('.group'), el => el.classList.add('active'))
    })
}

on(document, {
    scroll(e) {
        headings.forEach(ha => {
            const rect = ha.getBoundingClientRect()
            if (rect.top > 0 && rect.top < 150) {
                const location = leftPart(window.location.toString(), '#')
                history.replaceState(null, null, location + '#' + ha.id)
                select(ha.id)
            }
        })
    }
})
if (location.hash) {
    select(location.hash.substring(1))
}

//scroll menu item into view
function isInView(el) {
    const box = el.getBoundingClientRect();
    return box.top < window.innerHeight && box.bottom >= 0;
}

const active = $1('.sidebar .active')
if (active && !isInView(active)) {
    (active.parentElement.previousElementSibling || active.parentElement.parentElement || active).scrollIntoView()
}

/* used in :::copy and :::sh CopyContainerRenderer */
globalThis.copy = function(e) {
    e.classList.add('copying')
    let $el = document.createElement("textarea")
    let text = (e.querySelector('code') || e.querySelector('p')).innerHTML
    $el.innerHTML = text
    document.body.appendChild($el)
    $el.select()
    document.execCommand("copy")
    document.body.removeChild($el)
    setTimeout(() => e.classList.remove('copying'), 3000)
}