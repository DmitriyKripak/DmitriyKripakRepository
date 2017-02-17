    function showError(container, errorMessage) {
        container.className = 'error';
        var msgElem = document.createElement('span');
        msgElem.className = "error-message";
        msgElem.innerHTML = errorMessage;
        container.appendChild(msgElem);
    }

function resetError(container) {
    container.className = '';
    if (container.lastChild.className == "error-message") {
        container.removeChild(container.lastChild);
    }
}

function validate(form) {
    var elems = form.elements;

    resetError(elems.Login.parentNode);
    if (!elems.Login.value) {
        showError(elems.Login.parentNode, ' Enter login');
    }

    resetError(elems.Email.parentNode);
    if (!elems.Email.value) {
        showError(elems.Email.parentNode, ' Enter email');
    }
  
}
   
    

