// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


let btnClose = document.querySelector(".btn-close");
btnClose.onclick=function(event) {
        let div = event.target.closest("div");
        div.hidden = true;
    }

