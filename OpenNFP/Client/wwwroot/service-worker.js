// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
//self.addEventListener('fetch', () => { });

self.addEventListener("periodicsync", (event) => {
    console.log("Sync event initialed!");
    if (event.tag === "show_notify") {
        event.waitUntil(show_notify());
    }
});


function show_notify() {
    return new Promise((resolve, reject) => {
        console.info("Running backgorund notify")
        const options = {
            body: "Don't forget to add your info for today.",
        };
        self.registration.showNotification("Add Todays Info!", options);
        resolve();
    });
}