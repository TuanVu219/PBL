const currentDate = document.querySelector(".current-date")
const dayTag = document.querySelector(".days")
const prevNextIcon = document.querySelectorAll(".icond i")
const selectedDateInput = document.getElementById("selectedDate")

let date = new Date(), currYear = date.getFullYear(), currMonth = date.getMonth()

const months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
const renderCalendar = () => {
    let firstDayOfMonth = new Date(currYear, currMonth, 1).getDay()
    let lastDateOfMonth = new Date(currYear, currMonth + 1, 0).getDate()
    let lastDayOfMonth = new Date(currYear, currMonth, lastDateOfMonth).getDay()
    let lastDateOfLastMonth = new Date(currYear, currMonth, 0).getDate()
    let liTag = ""

    for (let i = firstDayOfMonth; i > 0; i--) {
        liTag += `<li class="inactive">${lastDateOfLastMonth - i + 1}</li>`
    }

    for (let i = 1; i <= lastDateOfMonth; i++) {
        let isToday = i === date.getDate() && currMonth === new Date().getMonth() && currYear === new Date().getFullYear() ? "active" : ""

        liTag += `<li class="${isToday}" data-day="${i}">${i}</li>`
    }

    for (let i = lastDayOfMonth; i < 6; i++) {
        liTag += `<li class="inactive">${i - lastDayOfMonth + 1}</li>`
    }

    currentDate.innerText = `${months[currMonth]} ${currYear}`
    dayTag.innerHTML = liTag

    // Thêm sự kiện click cho mỗi ngày
    const days = document.querySelectorAll(".days li");
    days.forEach(day => {
        day.addEventListener('click', function () {
            days.forEach(day => {
                day.classList.remove("selected");
            });
            this.classList.add("selected");
        });

    });
}

renderCalendar()

prevNextIcon.forEach(icon => {
    icon.addEventListener('click', () => {
        currMonth = icon.id === "js-back-month" ? currMonth - 1 : currMonth + 1

        if (currMonth < 0 || currMonth > 11) {
            date = new Date(currYear, currMonth)
            currYear = date.getFullYear()
            currMonth = date.getMonth()
        } else {
            date = new Date()
        }

        renderCalendar()
    });
});
// Thêm sự kiện click cho từng ngày trong lịch
dayTag.addEventListener('click', (event) => {
    if (event.target.tagName === 'LI' && !event.target.classList.contains('inactive')) {
        const selectedDay = event.target.dataset.day;
        const selectedMonth = currMonth + 1; // Months in JavaScript start from 0, so add 1 to get the correct month
        const selectedYear = currYear;
        selectedDateInput.value = `${selectedYear}-${selectedMonth < 10 ? '0' : ''}${selectedMonth}-${selectedDay < 10 ? '0' : ''}${selectedDay}`;
    }
});