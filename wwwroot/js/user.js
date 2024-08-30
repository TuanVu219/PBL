const openUser = document.querySelector('.js-open-user')
const closeUser = document.querySelector('.js-close-user')
const containerUser = document.querySelector('.js-container-user')
function openUSER(){
    containerUser.classList.add('open')
}
function closeUSER(){
    containerUser.classList.remove('open')
}
openUser.addEventListener('click',openUSER)
closeUser.addEventListener('click', closeUSER)