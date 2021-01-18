let videoWrapperElement = $(".video");

let videoElement = $("video.video");
if (videoElement.length == 0) {
	videoElement = $(".video video");
}

// This code scales the video element while matching the proportions to the proportions of its video canvas.
// If the video element does not match the proportions of its video it is playing,
// the default controls will exceed the width of the video.
// To match contom controls to the video canvas it is likewise necessary
// that the video element exactly matches its video.

// video-container: The containing window in which the video is centered
// video-frame: the containing frame nesting the video and matches its size.
let videoContainerElement = $(".video-container");

let videoRatio = 1 / 1;

function resizeVideo() {
	let parentWidth = videoContainerElement.width();
	let parentHeight = videoContainerElement.height();

	let videoWidth = 0;
	let videoHeight = 0;

	if (parentWidth / parentHeight > videoRatio) {
		videoHeight = parentHeight;
		videoWidth = videoHeight * videoRatio;
	} else {
		videoWidth = parentWidth;
		videoHeight = videoWidth / videoRatio;
	}

	// 'outerWidth/Height' accounts for borders around the video element
	videoWrapperElement.outerWidth(videoWidth);
	videoWrapperElement.outerHeight(videoHeight);
}

videoElement[0].addEventListener("loadedmetadata", function (e) {
	let videoWidth = this.videoWidth;
	let videoHeight = this.videoHeight;
	videoRatio = videoWidth / videoHeight;
	resizeVideo();
}, false);

$(window).resize(function () {
	resizeVideo();
});
