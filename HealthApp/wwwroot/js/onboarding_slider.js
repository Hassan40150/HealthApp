document.addEventListener("DOMContentLoaded", function () {
    const sliders = document.querySelectorAll(".styled-slider");

    function updateSliderFill(slider) {
        const percent = ((slider.value - slider.min) / (slider.max - slider.min)) * 100;
        slider.style.background = `linear-gradient(to right, #49BDF8 0%, #49BDF8 ${percent}%, #fff ${percent}%, #fff 100%)`;
    }

    sliders.forEach(slider => {
        updateSliderFill(slider); // initial fill
        slider.addEventListener("input", () => updateSliderFill(slider));
    });
});