export function isMobile() {
    const result = /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
    console.log(result);
    return result;
}
