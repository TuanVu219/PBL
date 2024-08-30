document.addEventListener('DOMContentLoaded', function () {
    // Function to reset all dropdown selections to their default values
  
    // Existing code for handling dropdowns and buttons
    var dayCheckboxes = document.querySelectorAll('.day input[type="checkbox"]');
    var shiftRadio = document.querySelectorAll('.shift input[type="radio"]');
    var dayUncheckButton = document.getElementById('day-uncheck');
    var confirmButton = document.getElementById('day-confirm');
    var changeButton = document.getElementById('day-change');

    function hideAllTimeBlocks(dayElement) {
        const timeBlocks = dayElement.querySelectorAll('.time > div');
        timeBlocks.forEach(block => block.parentElement.style.display = 'none');
    }

    // Function to show the selected time block for a specific shift and day
    function showSelectedTimeBlock(dayElement, shift) {
        const selectedTimeClass = `${dayElement.dataset.day}-${shift}-time`;
        const timeBlock = dayElement.querySelector(`.${selectedTimeClass}`);
        if (timeBlock) {
            timeBlock.parentElement.style.display = 'flex';
        }
    }

    // Get all day elements
    const days = document.querySelectorAll('.day');

    days.forEach(dayElement => {
        // Hide all time blocks initially
        hideAllTimeBlocks(dayElement);

        // Get all radio buttons within the current day element
        const radioButtons = dayElement.querySelectorAll('.shift input[type="radio"]');

        radioButtons.forEach(radio => {
            radio.addEventListener('change', function () {
                // Hide all time blocks for the current day
                hideAllTimeBlocks(dayElement);

                // Show the selected time block
                const selectedShift = radio.value;
                showSelectedTimeBlock(dayElement, selectedShift);
            });
        });
    });

    function uncheckAllShifts(day) {
        var ShiftRadios = document.querySelectorAll('.' + day + ' .shift input[type="radio"]');
        ShiftRadios.forEach(function (radio) {
            radio.checked = false;
            radio.parentElement.style.display = 'none';
        });
    }

    function hideUnselected() {
        dayCheckboxes.forEach(function (checkbox) {
            var shiftName = checkbox.getAttribute('name') + '_shift';
            var shiftRadios = document.querySelectorAll('input[name="' + shiftName + '"]');
            var dayDiv = checkbox.closest('.day');

            if (!checkbox.checked) {
                // Nếu checkbox ngày không được chọn, ẩn ngày và các shift của ngày đó
                dayDiv.style.display = 'none';
                shiftRadios.forEach(function (radio) {
                    radio.parentElement.style.display = 'none';
                });
            } else {
                // Nếu checkbox ngày được chọn, hiển thị ngày và các shift của ngày đó
                dayDiv.style.display = 'block';
                shiftRadios.forEach(function (radio) {
                    if (!radio.checked) {
                        radio.parentElement.style.display = 'none';
                    }
                });
            }
        });
    }

    function showAllDays() {
        dayCheckboxes.forEach(function (checkbox) {
            var dayDiv = checkbox.closest('.day');
            dayDiv.style.display = 'block'; // Hiển thị ngày 
            if (checkbox.checked) {
                // Nếu ngày đã được chọn, hiển thị tất cả các shift của ngày đó
                var shiftName = checkbox.getAttribute('name') + '_shift';
                var shiftRadios = document.querySelectorAll('input[name="' + shiftName + '"]');
                shiftRadios.forEach(function (radio) {
                    radio.parentElement.style.display = 'inline-block'; // Hiển thị shift
                });
            }
        });
    }

    // Event listener for day checkboxes
    dayCheckboxes.forEach(function (dayCheckbox) {
        dayCheckbox.addEventListener('change', function () {
            var dayDiv = this.closest('.day'); // Tìm div cha chứa checkbox ngày
            var dayClass = dayDiv.classList[0]; // Lấy tên lớp của div ngày
            var shiftLabels = dayDiv.querySelectorAll('.shift'); // Lấy tất cả các shift của ngày

            shiftLabels.forEach(function (label) {
                label.style.display = this.checked ? 'inline-block' : 'none';
            }, this);

            if (!this.checked) {
                uncheckAllShifts(dayClass);
                hideAllTimeBlocks(dayDiv); // Uncheck tất cả các shift của ngày nếu checkbox bị unchecked
            }
        });
    });

    function hideAllShiftRadio() {
        shiftRadio.forEach(function (radio) {
            radio.checked = false; // Uncheck all shift radios
            radio.parentElement.style.display = 'none'; // Hide shift labels
        });
    }

    dayuncheck.addEventListener('click', function () {
        dayCheckboxes.forEach(function (checkbox) {
            checkbox.checked = false;
            var dayDiv = checkbox.closest('.day');
            hideAllTimeBlocks(dayDiv); // Uncheck each day checkbox
        });
        // Hide all shift radios
        hideAllShiftRadio();
        showAllDays();
        showAlert('Clear successfully!', 'white', 'blue');
    });

    // Event listener for the confirm button
    $('#schedule-form').on('submit', function (event) {
        // Check if at least one day checkbox is checked
        var atLeastOneDayChecked = false;
        var atLeastOneShiftChecked = true; // Flag to check if at least one shift is checked for each checked day

        dayCheckboxes.forEach(function (dayCheckbox) {
            if (dayCheckbox.checked) {
                atLeastOneDayChecked = true;

                // Check if at least one shift is checked for the current day
                var shiftChecked = false;
                var shiftRadios = document.querySelectorAll('input[name="' + dayCheckbox.name + '_shift"]');
                shiftRadios.forEach(function (shiftRadio) {
                    if (shiftRadio.checked) {
                        shiftChecked = true;
                    }
                });

                // If no shift is checked for the current day, set the flag to false
                if (!shiftChecked) {
                    atLeastOneShiftChecked = false;
                }
            }
        });

        // Show custom alert if necessary
        if (!atLeastOneDayChecked) {
            showAlert('Please select at least one day.', 'white', 'red');
        }

        event.preventDefault();
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    if (atLeastOneDayChecked) {
                        showAlert('Selection confirmed successfully!', 'white', 'green');
                        // Show success message
                    }
                } else {
                    showAlert(response.message, 'white', 'red');

                }
            },
            error: function () {
                $('#custom-alert-message').text('An error occurred while processing your request.');
                $('#custom-alert').show();
            }
        });

    });




    // Event listener for the change button
    changeButton.addEventListener('click', function () {
        showAllDays();
        // Show message
        showAlert('Selection is now enabled. You can change the selection.', 'white', 'blue');
    });

    // Function to show custom alert
    function showAlert(message, textColor, backgroundColor) {
        var customAlert = document.getElementById('custom-alert');
        var customAlertMessage = document.getElementById('custom-alert-message');
        customAlertMessage.textContent = message;
        customAlert.style.color = textColor;
        customAlert.style.backgroundColor = backgroundColor;
        customAlert.style.display = 'block';

        // Tự động ẩn alert sau khoảng thời gian duration (miligiây)
        setTimeout(function () {
            customAlert.style.display = 'none';
        }, 2500);
    }
});