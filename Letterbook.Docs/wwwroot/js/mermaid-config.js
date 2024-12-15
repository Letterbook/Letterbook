// Check if the user prefers dark mode
function isDarkMode() {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
}

function updateTheme() {
    mermaid.initialize({
        theme: isDarkMode() ? 'dark' : 'default',
    });
}

// Initial theme setup
updateTheme();

// Listen for changes in the color scheme preference
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', updateTheme);
