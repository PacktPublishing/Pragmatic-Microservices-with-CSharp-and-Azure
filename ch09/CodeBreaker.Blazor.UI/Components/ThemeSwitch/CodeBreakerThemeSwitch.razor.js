export function switchTheme(dark) {
    var body = document.getElementsByTagName('body');
    if (dark) {
        body[0].classList = 'dark';
    } else {
        body[0].classList = '';
    }
}
