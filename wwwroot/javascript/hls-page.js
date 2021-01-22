import config from '../config.js';
import "./utils/videoframe.js";
import './debug/debug.js';

var player = videojs('hls-video', {
	autoplay: 'any',
	// controls: true,
	controlBar: {
		'bigPlayButton': false,
		'pictureInPictureToggle': true,
		'volumePanel': false,
	}
});

player.src([
	{type: 'application/x-mpegURL', src: config.hlsUrl },
	// {type: 'application/dash+xml', src: config.mpegDashUrl }
  ]);

player.bigPlayButton.hide();
player.play();
