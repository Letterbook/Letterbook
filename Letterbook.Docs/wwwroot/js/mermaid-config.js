// Check if the user prefers dark mode
function isDarkMode() {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
}
mermaid.initialize({
    theme: isDarkMode() ? 'dark' : 'default',
});
