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

// Animate blur overlay with fade-back support using RGBA and backdrop-filter blur
window.animateBlurOverlay = function (targetOpacity, durationSecs, fadeBackDurationSecs, waitSecs, dotNetReference) {
    console.log(`[DEBUG] animateBlurOverlay called: targetOpacity=${targetOpacity}, durationSecs=${durationSecs}, fadeBackDurationSecs=${fadeBackDurationSecs}, waitSecs=${waitSecs}`);
    
    var blurOverlay = document.querySelector('.blur-overlay');
    if (!blurOverlay) {
        console.error("[ERROR] animateBlurOverlay: blur-overlay element not found!");
        return;
    }
    
    console.log("[DEBUG] blur-overlay element found, starting animation");
    
    // Ensure the element is visible first
    blurOverlay.style.display = 'block';
    blurOverlay.style.pointerEvents = 'auto';
    
    // Read current RGBA values to start from
    var computedStyle = window.getComputedStyle(blurOverlay);
    var bgColor = computedStyle.backgroundColor;
    var backdropFilter = computedStyle.backdropFilter || 'blur(0px)';
    
    // Parse current opacity from backgroundColor (format: rgba(0, 0, 0, alpha))
    var currentOpacity = 0;
    console.log("[DEBUG] Current background color: " + bgColor);
    var rgbaMatch = bgColor.match(/^rgba\(([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[Ee]([+-]?\d+))?, ([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[Ee]([+-]?\d+))?, ([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[Ee]([+-]?\d+))?, ([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[Ee]([+-]?\d+))?\)$/m);
    console.log("[DEBUG] Full match (rgbaMatch[0]): " + (rgbaMatch ? rgbaMatch[0] : 'null'));
    console.log("[DEBUG] Red value (rgbaMatch[1]): " + (rgbaMatch ? rgbaMatch[1] : 'null'));
    console.log("[DEBUG] Green value (rgbaMatch[3]): " + (rgbaMatch ? rgbaMatch[3] : 'null'));
    console.log("[DEBUG] Blue value (rgbaMatch[5]): " + (rgbaMatch ? rgbaMatch[5] : 'null'));
    console.log("[DEBUG] Alpha value (rgbaMatch[7]): " + (rgbaMatch ? rgbaMatch[7] : 'null'));
    if (rgbaMatch && rgbaMatch[7]) {
        currentOpacity = parseFloat(rgbaMatch[7]);
    }
    console.log("[DEBUG] Parsed current opacity: " + currentOpacity);
    
    // Parse current blur amount from backdropFilter (format: blur(Xpx) or none)
    var currentBlurPx = 0;
    var blurMatch = backdropFilter.match(/blur\((\d+(?:\.\d+)?)px\)/);
    if (blurMatch && blurMatch[1]) {
        currentBlurPx = parseFloat(blurMatch[1]);
    }

    console.log("[DEBUG] Parsed current blur: " + currentBlurPx + "px");
    
    console.log(`[DEBUG] Current state: opacity=${currentOpacity.toFixed(3)}, blur=${currentBlurPx.toFixed(2)}px`);
    
    var maxBlurPx = 10 * targetOpacity;
    console.log(`[DEBUG] Initial background color: ${bgColor}, backdropFilter: ${backdropFilter}`);
    
    // Animate to target opacity and max blur
    var startTime = Date.now();
    var durationMs = durationSecs * 1000;
    
    console.log(`[DEBUG] Starting fade-in animation: duration=${durationMs}ms, target=${targetOpacity}`);
    
    var animateTo = function () {
        var elapsed = Date.now() - startTime;
        var progress = Math.min(elapsed / durationMs, 1);
        
        // Linear interpolation from current to target opacity and blur
        var newOpacity = currentOpacity + (targetOpacity - currentOpacity) * progress;
        var newBlur = currentBlurPx + (maxBlurPx - currentBlurPx) * progress;
        
        blurOverlay.style.backgroundColor = `rgba(0, 0, 0, ${newOpacity})`;
        blurOverlay.style.backdropFilter = `blur(${newBlur}px)`;
        blurOverlay.style.webkitBackdropFilter = `blur(${newBlur}px)`;
        
        console.log(`[DEBUG] Fade-in progress: ${(progress * 100).toFixed(1)}%, opacity=${newOpacity.toFixed(3)}, blur=${newBlur.toFixed(2)}px`);
        
        if (progress < 1) {
            requestAnimationFrame(animateTo);
        } else {
            // Animation to target opacity complete
            blurOverlay.style.backgroundColor = `rgba(0, 0, 0, ${targetOpacity})`;
            blurOverlay.style.backdropFilter = `blur(${maxBlurPx}px)`;
            blurOverlay.style.webkitBackdropFilter = `blur(${maxBlurPx}px)`;
            console.log(`[DEBUG] Fade-in complete, opacity set to ${targetOpacity}, blur set to ${maxBlurPx}px`);
            
            // Handle fade back if specified and not -1
            if (fadeBackDurationSecs >= 0) {
                var waitMs = waitSecs * 1000;
                console.log(`[DEBUG] Will wait ${waitMs}ms before fading back over ${fadeBackDurationSecs}s`);
                setTimeout(function () {
                    animateFadeBack();
                }, waitMs);
            }
        }
    };
    
    var animateFadeBack = function () {
        console.log(`[DEBUG] Starting fade-back animation: duration=${fadeBackDurationSecs * 1000}ms`);
        
        var fadeBackStartTime = Date.now();
        var fadeBackDurationMs = fadeBackDurationSecs * 1000;
        
        var animateBack = function () {
            var elapsed = Date.now() - fadeBackStartTime;
            var progress = Math.min(elapsed / fadeBackDurationMs, 1);
            
            // Linear interpolation from target opacity back to 0
            var newOpacity = targetOpacity * (1 - progress);
            var newBlur = maxBlurPx * (1 - progress);
            
            blurOverlay.style.backgroundColor = `rgba(0, 0, 0, ${newOpacity})`;
            blurOverlay.style.backdropFilter = `blur(${newBlur}px)`;
            blurOverlay.style.webkitBackdropFilter = `blur(${newBlur}px)`;
            
            console.log(`[DEBUG] Fade-back progress: ${(progress * 100).toFixed(1)}%, opacity=${newOpacity.toFixed(3)}, blur=${newBlur.toFixed(2)}px`);
            
            if (progress < 1) {
                requestAnimationFrame(animateBack);
            } else {
                blurOverlay.style.backgroundColor = 'rgba(0, 0, 0, 0)';
                blurOverlay.style.backdropFilter = 'blur(0px)';
                blurOverlay.style.webkitBackdropFilter = 'blur(0px)';
                console.log(`[DEBUG] Fade-back complete, opacity and blur reset to 0`);
                
                // Disable the blur overlay in C#
                if (dotNetReference) {
                    dotNetReference.invokeMethodAsync('DisableBlurOverlay');
                }
            }
        };
        
        requestAnimationFrame(animateBack);
    };
    
    console.log("[DEBUG] Starting requestAnimationFrame for fade-in");
    requestAnimationFrame(animateTo);
};