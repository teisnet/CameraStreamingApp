let gridIsOn = false;

function toggleGrid() {
	if (!gridIsOn) {
		$("body").addClass("debug-grid");
		gridIsOn = true;
	}
	else {
		$("body").removeClass("debug-grid");
		gridIsOn = false;
	}
}

window.addEventListener("keydown", event => {
	if (event.key.toLowerCase() === "x") {
		toggleGrid();
	}
});
