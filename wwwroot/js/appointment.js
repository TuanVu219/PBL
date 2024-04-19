var bookedDates = @Html.Raw(ViewBag.BookedDatesJson);

// Hàm để vô hiệu hóa các ngày đã được chọn
function disableBookedDates() {
    var dateInput = document.getElementById('Date');

    // Chuyển đổi danh sách ngày đã được chọn thành định dạng phù hợp
    var formattedBookedDates = bookedDates.map(function (dateString) {
        return new Date(dateString);
    });

    // Hàm kiểm tra xem một ngày có trong danh sách ngày đã được chọn hay không
    function isBookedDate(date) {
        return formattedBookedDates.some(function (bookedDate) {
            return date.getTime() === bookedDate.getTime();
        });
    }

    // Hàm kiểm tra xem một ngày có phải là Chủ nhật hay không
    function isSunday(date) {
        return date.getDay() === 0;
    }

    // Hàm kiểm tra xem một ngày có phải là Thứ bảy hay không
    function isSaturday(date) {
        return date.getDay() === 6;
    }

    // Hàm để kiểm tra xem một ngày có thể chọn hay không
    function isSelectableDate(date) {
        return !isBookedDate(date) && !isSunday(date) && !isSaturday(date);
    }

    // Lặp qua tất cả các ô ngày và vô hiệu hóa các ngày không thể chọn
    var currentDate = new Date();
    var endDate = new Date(dateInput.max);
    for (var d = new Date(dateInput.min); d <= endDate; d.setDate(d.getDate() + 1)) {
        if (!isSelectableDate(d) || d < currentDate) {
            var option = document.createElement('option');
            option.value = d.toISOString().slice(0, 10);
            option.textContent = d.toDateString();
            option.disabled = true;
            dateInput.appendChild(option);
        }
    }
}

// Gọi hàm disableBookedDates khi trang đã tải
window.onload = disableBookedDates;