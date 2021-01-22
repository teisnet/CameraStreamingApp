let isFullscreen = false;
var fullscreenElement = null;

function openFullscreen() {
	if (!fullscreenElement) { return };

	isFullscreen = true;

	if (fullscreenElement.requestFullscreen) {
		fullscreenElement.requestFullscreen();
	} else if (fullscreenElement.webkitRequestFullscreen) { /* Safari */
		fullscreenElement.webkitRequestFullscreen();
	} else if (fullscreenElement.msRequestFullscreen) { /* IE11 */
		fullscreenElement.msRequestFullscreen();
	}
}

function closeFullscreen() {
	if (!fullscreenElement) { return };

	isFullscreen = false;
	
	if (document.exitFullscreen) {
		document.exitFullscreen();
	} else if (document.webkitExitFullscreen) { /* Safari */
		document.webkitExitFullscreen();
	} else if (document.msExitFullscreen) { /* IE11 */
		document.msExitFullscreen();
	}
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
