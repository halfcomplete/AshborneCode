window.autoScrollGameOutput = true;

window.scrollGameOutputToBottom = function (force) {
    var el = document.querySelector('.game-output');
    if (!el) return;
    if (!((el.scrollTop + el.clientHeight + 110) >= el.scrollHeight)) return;
    if (window.autoScrollGameOutput || force) {
        el.scrollTop = el.scrollHeight;
    }
};

// Listen for user scroll events
window.addEventListener('DOMContentLoaded', function () {
    var el = document.querySelector('.game-output');
    if (!el) return;
    el.addEventListener('scroll', function () {
        // If user is at the bottom (within 5px), enable auto-scroll
        if (el.scrollTop + el.clientHeight >= el.scrollHeight - 110) {
            window.autoScrollGameOutput = true;
        } else {
            window.autoScrollGameOutput = false;
        }
    });
}); 