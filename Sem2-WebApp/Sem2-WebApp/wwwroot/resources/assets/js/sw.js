self.addEventListener("push",
    function(event) {
        console.log("[Service Worker] Push Received.");
        console.log(`[Service Worker] Push had this data: "${event.data.text()}"`);

        const title = "Aubergine Console";
        const options = {
            body: event.data.text(),
            icon: "/resources/assets/images/rsz_aubergine_logo.png",
            badge: "/resources/assets/images/badge.png"
        };

        event.waitUntil(self.registration.showNotification(title, options));
    });

self.addEventListener("notificationclick",
    function(event) {
        console.log("[Service Worker] Notification click Received.");

        event.notification.close();

        event.waitUntil(
            clients.openWindow("https://localhost:44311/AdminHome/Overview/")
        );
    });