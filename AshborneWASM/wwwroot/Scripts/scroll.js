window.scrollGameOutputToBottom = function (smooth) {
    var el = document.querySelector('.game-output');
    if (el) {
        el.scrollTo({ top: el.scrollHeight, behavior: smooth ? 'smooth' : 'auto' });
    }
}; 