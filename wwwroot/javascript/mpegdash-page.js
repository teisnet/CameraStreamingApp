import config from '../config.js';
import "./utils/videoframe.js";

var player = videojs('mpegdash-video', {
	autoplay: 'any',
	// controls: true,
	controlBar: {
		'bigPlayButton': false,
		'pictureInPictureToggle': true,
		'volumePanel': false,
	}
});

player.src([
	// {type: 'application/x-mpegURL', src: config.hlsUrl },
	{type: 'application/dash+xml', src: config.mpegDashUrl }
  ]);

player.bigPlayButton.hide();
player.play();
