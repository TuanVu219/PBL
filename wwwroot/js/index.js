const openNotificationButton = document.querySelector('.js-open-notification');
const closeNotificationButton = document.querySelector('.js-close-notification');
const notificationContainer = document.querySelector('.js-container-notification');
const notificationCountElement = document.querySelector('.notification-count');

let isNotificationOpen = false; // Variable to track the notification state
let isFirstTimeClose = true; // Variable to track if the notification has been closed for the first time

function toggleNotification() {
    if (isNotificationOpen) {
        closeNotification(); // Close notification if it's open
    } else {
        openNotification(); // Open notification if it's closed
    }
    isNotificationOpen = !isNotificationOpen; // Toggle the state
}

function openNotification() {
    notificationContainer.classList.add('open');
    openNotificationButton.classList.add('open');

    // Reset the content of .notification-count to '0'
    notificationCountElement.innerHTML = '0';

    $.ajax({
        url: '/Home/MarkAsSeen',
        type: 'POST',
        success: function (response) {
            console.log('Notifications marked as seen');
        },
        error: function (xhr, status, error) {
            console.error('Failed to mark notifications as seen', status, error);
        }
    });
}

function closeNotification() {
    notificationContainer.classList.remove('open');
    openNotificationButton.classList.remove('open');
    if (isFirstTimeClose) {
        changeSeenNotificationsColor();
        isFirstTimeClose = false; // Mark that the notification has been closed for the first time
    }
}

function changeSeenNotificationsColor() {
    const notificationLines = document.querySelectorAll('.notification-line2');
    notificationLines.forEach(function (notificationLine) {
        notificationLine.style.backgroundColor = 'white';
    });
}

// Adding event listeners to open and close buttons
openNotificationButton.addEventListener('click', toggleNotification);
closeNotificationButton.addEventListener('click', closeNotification);
