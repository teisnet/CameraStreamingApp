var player = videojs('video', {
	autoplay: 'any',
	// controls: true,
	controlBar: {
		'bigPlayButton': false,
		'pictureInPictureToggle': true,
		'volumePanel': false,
	}
});

player.bigPlayButton.hide();
player.play();
