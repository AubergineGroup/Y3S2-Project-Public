"use strict";

let subscriptionId;

$(function() {
    const applicationServerPublicKey =
        "BPKFcjKytHpmxpnt0H3QPVG1hCXt-YKBAB1eT7Wpxz9S6fGfKvCdf9bFeYSrBsUjkg15JQR9zUI4rC9k8wbRBx0";

    let isSubscribed = false;
    let swRegistration = null;

    function urlB64ToUint8Array(base64String) {
        const padding = "=".repeat((4 - base64String.length % 4) % 4);
        const base64 = (base64String + padding)
            .replace(/\-/g, "+")
            .replace(/_/g, "/");

        const rawData = window.atob(base64);
        const outputArray = new Uint8Array(rawData.length);

        for (let i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i);
        }
        return outputArray;
    }

    function updateBtn() {
        if (Notification.permission === "denied") {
            $(".js-push-btn").text("Push Messaging Blocked.");
            $(".js-push-btn").prop("disabled", false);
            updateSubscriptionOnServer(null);
            return;
        }

        $(".js-push-btn").text(isSubscribed ? "Disable Push Messaging" : "Enable Push Messaging");
        $(".js-push-btn").prop("disabled", false);
    }

    function updateSubscriptionOnServer(subscription) {
        if (subscription) {
            digestMessage(subscription.endpoint).then(digestValue => {
                const args = {
                    id: hexString(digestValue),
                    pushEndpoint: subscription.endpoint,
                    pushP256Dh: btoa(String.fromCharCode.apply(null, new Uint8Array(subscription.getKey("p256dh")))),
                    pushAuth: btoa(String.fromCharCode.apply(null, new Uint8Array(subscription.getKey("auth"))))
                };

                $.ajax({
                    type: "POST",
                    accepts: "application/json",
                    url: "/api/Subscriptions",
                    contentType: "application/json",
                    data: JSON.stringify(args),
                    success: function() {
                        console.log("Subscription successfully updated on server.");
                    }
                });
            });
        }
    }

    function digestMessage(message) {
        const encoder = new TextEncoder();
        const data = encoder.encode(message);
        return window.crypto.subtle.digest("SHA-256", data);
    }

    function hexString(buffer) {
        const byteArray = new Uint8Array(buffer);

        const hexCodes = [...byteArray].map(value => {
            const hexCode = value.toString(16);
            const paddedHexCode = hexCode.padStart(2, "0");
            return paddedHexCode;
        });

        return hexCodes.join("");
    }

    function subscribeUser() {
        const applicationServerKey = urlB64ToUint8Array(applicationServerPublicKey);
        swRegistration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerKey
            })
            .then(function(subscription) {
                console.log("User is subscribed.");

                updateSubscriptionOnServer(subscription);

                isSubscribed = true;

                updateBtn();
            })
            .catch(function(err) {
                console.log("Failed to subscribe the user: ", err);
                updateBtn();
            });
    }

    function unsubscribeUser() {
        swRegistration.pushManager.getSubscription()
            .then(function(subscription) {
                if (subscription) {
                    digestMessage(subscription.endpoint).then(digestValue => {
                        deleteSubscription(hexString(digestValue));
                    });
                    return subscription.unsubscribe();
                }
            })
            .catch(function(error) {
                console.log("Error unsubscribing", error);
            })
            .then(function() {
                updateSubscriptionOnServer(null);

                console.log("User is unsubscribed.");
                isSubscribed = false;

                updateBtn();
            });
    }

    function deleteSubscription(id) {
        $.ajax({
            url: `/api/Subscriptions/${id}`,
            type: "DELETE",
            success: function() {
                console.log("Subscription successfully deleted on server.");
            }
        });
    }

    function initializeUi() {
        $(".js-push-btn").click(function() {
            $(".js-push-btn").prop("disabled", false);
            if (isSubscribed) {
                unsubscribeUser();
            } else {
                subscribeUser();
            }
        });

        // Set the initial subscription value
        swRegistration.pushManager.getSubscription()
            .then(function(subscription) {
                isSubscribed = !(subscription === null);

                if (isSubscribed) {
                    digestMessage(subscription.endpoint).then(digestValue => {
                        subscriptionId = hexString(digestValue);
                    });
                    console.log(JSON.stringify(subscription));
                    console.log("User IS subscribed.");
                } else {
                    console.log("User is NOT subscribed.");
                }

                updateBtn();
            })
            .catch(function(err) {
                console.log("Error during getSubscription()", err);
            });
    }

    if ("serviceWorker" in navigator && "PushManager" in window) {
        console.log("Service Worker and Push is supported");

        navigator.serviceWorker.register("/resources/assets/js/sw.js")
            .then(function(swReg) {
                console.log("Service Worker is registered", swReg);

                swRegistration = swReg;
                initializeUi();
            })
            .catch(function(error) {
                console.error("Service Worker Error", error);
            });
    } else {
        console.warn("Push messaging is not supported");
        $(".js-push-btn").text("Push Not Supported");
    }
});