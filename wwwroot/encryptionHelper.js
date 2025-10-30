const encoder = new TextEncoder();
const decoder = new TextDecoder();
let cryptoKey = null;

/**
 * Loads or generates a persistent AES key for this browser.
 */
export async function initKey() {
    let keyBase64 = localStorage.getItem("form_encryption_key");

    if (!keyBase64) {
        // Generate random 32 bytes (256 bits)
        const rawKey = crypto.getRandomValues(new Uint8Array(32));
        keyBase64 = btoa(String.fromCharCode(...rawKey));
        localStorage.setItem("form_encryption_key", keyBase64);
    }

    const rawKey = Uint8Array.from(atob(keyBase64), c => c.charCodeAt(0));
    cryptoKey = await crypto.subtle.importKey(
        "raw",
        rawKey,
        { name: "AES-GCM" },
        false,
        ["encrypt", "decrypt"]
    );
}

/**
 * Encrypts text data.
 */
export async function encryptData(data) {
    if (!cryptoKey) throw new Error("Key not initialized. Call initKey() first.");
    const iv = crypto.getRandomValues(new Uint8Array(12));
    const encoded = encoder.encode(data);

    const cipher = await crypto.subtle.encrypt(
        { name: "AES-GCM", iv },
        cryptoKey,
        encoded
    );

    return JSON.stringify({
        iv: Array.from(iv),
        data: Array.from(new Uint8Array(cipher))
    });
}

/**
 * Decrypts previously encrypted data.
 */
export async function decryptData(encrypted) {
    if (!cryptoKey) throw new Error("Key not initialized. Call initKey() first.");
    try {
        const { iv, data } = JSON.parse(encrypted);
        const decrypted = await crypto.subtle.decrypt(
            { name: "AES-GCM", iv: new Uint8Array(iv) },
            cryptoKey,
            new Uint8Array(data)
        );

        return decoder.decode(decrypted);
    } catch (e) {
        console.error("Decryption failed:", e);
        return null;
    }
}
