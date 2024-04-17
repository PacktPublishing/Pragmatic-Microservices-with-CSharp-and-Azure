export function beforeStart(options, extensions) {
    console.log('Initialize theme in method beforeStart on client');
    const current = localStorage.getItem('theme');
    if (!current) {
        const darkThemeMq = window.matchMedia("(prefers-color-scheme: dark)");
        let result = 'light';
        if (darkThemeMq.matches) {
            result = 'dark';
        }
        current = result;
    }
    document.body.classList += current;
}

export function afterStarted(blazor) {
    console.log("afterStarted");
}
