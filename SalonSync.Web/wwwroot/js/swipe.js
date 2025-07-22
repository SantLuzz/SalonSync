window.scrollHorizontally = (elementId, deltaX) => {
    const container = document.getElementById(elementId);
    if (!container) return;
    container.scrollLeft += deltaX; // Move o scroll lateralmente
};

window.enableSwipeScroll = (elementId) => {
    const container = document.getElementById(elementId);
    if (!container) return;

    let isDown = false;
    let startX;
    let scrollLeft;

    container.addEventListener("mousedown", (e) => {
        isDown = true;
        startX = e.pageX - container.offsetLeft;
        scrollLeft = container.scrollLeft;
    });

    container.addEventListener("mouseleave", () => {
        isDown = false;
    });

    container.addEventListener("mouseup", () => {
        isDown = false;
    });

    // container.addEventListener("mousemove", (e) => {
    //     if (!isDown) return;
    //     e.preventDefault();
    //     const x = e.pageX - container.offsetLeft;
    //     const walk = (x - startX) * 2; // Ajuste a velocidade do scroll
    //     container.scrollLeft = scrollLeft - walk;
    // });
};