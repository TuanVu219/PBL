// Lấy các phần tử liên quan
const addBtn = document.querySelector('.add-btn');
const addModal = document.querySelector('.add-modal');
const modals = document.querySelectorAll('.modal');
const modalCloseButtons = document.querySelectorAll('.close-modal');
const modalContainers = document.querySelectorAll('.modal-container');



// Hiện modal thêm bác sĩ
function showAddDoctor() {
    addModal.classList.add('open');
}

// Hiện modal chỉnh sửa bác sĩ



// Ẩn tất cả các modal
function closeModal() {
    modals.forEach(modal => modal.classList.remove('open'));
}




// Gắn sự kiện click cho nút thêm mới
if (addBtn) {
    addBtn.addEventListener('click', showAddDoctor);
}


// Gắn sự kiện click cho tất cả các nút đóng modal
modalCloseButtons.forEach(closeButton => {
    closeButton.addEventListener('click', closeModal);
});



// Đóng modal khi click bên ngoài modal container
modals.forEach(modal => {
    modal.addEventListener('click', closeModal);
});

// Ngăn chặn sự kiện click lan truyền đến modal container
modalContainers.forEach(container => {
    container.addEventListener('click', event => {
        event.stopPropagation();
    });
});

const dateInput = document.getElementById('dateInput');
const selectedDateDiv = document.getElementById('selectedDate');

dateInput.addEventListener('change', function () {
    const selectedDate = new Date(this.value);
    const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
    selectedDateDiv.textContent = selectedDate.toLocaleDateString('en-GB', options); // Bạn có thể thay đổi định dạng theo ý muốn
});
