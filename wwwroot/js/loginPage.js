// const openLogin = document.querySelector('.background-log')
// const closeLogin = document.querySelector('.close-log')
// const openLog = document.querySelector('.open-login-signin')
// function openLOG(){
//     openLogin.classList.add('open')
// }
// function closeLOG(){
//     openLogin.classList.remove('open')
// }
// openLog.addEventListener('click',openLOG)
// closeLogin.addEventListener('click',closeLOG)
//log in and signin
const signinbtn = document.querySelector('.register-link')
const loginbtn = document.querySelector('.login-link')
const ok = document.querySelector('.ok')
const bgr = document.querySelector('.backgroundAnimation')
function showSignin() {
    bgr.classList.add('bgrMoveLeft')
    bgr.classList.remove('bgrMoveRight')
    ok.classList.add('okMoveLeft')
    ok.classList.remove('okMoveRight')
}
function showLogin() {
    bgr.classList.remove('bgrMoveLeft')
    bgr.classList.add('bgrMoveRight')
    ok.classList.remove('okMoveLeft')
    ok.classList.add('okMoveRight')
}
signinbtn.addEventListener('click', showSignin)
loginbtn.addEventListener('click', showLogin)
// calendar