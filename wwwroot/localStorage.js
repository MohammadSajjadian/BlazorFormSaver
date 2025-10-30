export function saveItem(key, value) {
    localStorage.setItem(key, value);
    return true;
}

export function loadItem(key) {
    return localStorage.getItem(key);
}

export function removeItem(key) {
    localStorage.removeItem(key);
    return true;
}
