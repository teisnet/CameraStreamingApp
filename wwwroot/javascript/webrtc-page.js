import config from '../config.js';
import WowzaWebRTCPlay from '../lib/WowzaWebRTCPlay.js';
import "./utils/videoframe.js";

window.wowzaWebRTCPlay = new WowzaWebRTCPlay();
wowzaWebRTCPlay.on({
	onError: (error) => {
		console.log( JSON.stringify(error) )
	},
	onStateChanged: (state) => {
		console.log (state.connectionState);
	}
});

wowzaWebRTCPlay.set({ videoElementPlay: document.getElementById('webrtc-video') });

wowzaWebRTCPlay.set(config);

wowzaWebRTCPlay.play();
