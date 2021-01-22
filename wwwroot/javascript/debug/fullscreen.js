let isFullscreen = false;
let fullscreenElement = null;

function openFullscreen() {
	if (!fullscreenElement) { return };

	isFullscreen = true;

	let request = fullscreenElement.requestFullscreen || fullscreenElement.webkitRequestFullscreen || elem.mozRequestFullScreen || fullscreenElement.msRequestFullscreen;
	request.call(fullscreenElement);
}

function closeFullscreen() {
	if (!fullscreenElement) { return };

	let request = document.exitFullscreen || document.webkitExitFullscreen || document.msExitFullscreen;
	request.call(document);

	isFullscreen = false;
}

function toggleFullscreen() {
	isFullscreen ? closeFullscreen() : openFullscreen();
}

 function addFullScreen(key, element) {
	fullscreenElement = (typeof element == "undefined") ? document.documentElement : $(element)[0];

	window.addEventListener("keydown", event => {
		if (event.key === key) {
			toggleFullscreen();
		}
	 });
	}

 export default addFullScreen;
