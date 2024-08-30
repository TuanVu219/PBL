const openRates = document.querySelectorAll('.js-open-rate')
const closeRate = document.querySelector('.js-close-rate')
const containerRate = document.querySelector('.js-container-rate')
const doctorIdInput = document.querySelector('input[name="DoctorId"]')
const appointmentHistoryIdInput = document.querySelector('input[name="AppointmentHistoryId"]'); // New line


function openRATE() {
    const doctorId = event.currentTarget.getAttribute('data-doctor-id')
    const appointmentHistoryId = event.currentTarget.getAttribute('data-appointment-history-id'); // New line
    appointmentHistoryIdInput.value = appointmentHistoryId; // New line
    doctorIdInput.value = doctorId
    containerRate.classList.add('open')
}
function closeRATE(){
    containerRate.classList.remove('open')

}
for (const openRate of openRates) {
    openRate.addEventListener('click', openRATE)
}

closeRate.addEventListener('click', closeRATE)