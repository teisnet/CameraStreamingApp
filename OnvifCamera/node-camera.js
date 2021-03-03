// https://github.com/agsh/onvif

var Cam = require('onvif').Cam;


const cameraNotInitializedErrorMessage = "The camera has not been initialized before use";
var camera;

module.exports = {

	init: function (cb, url, port, username, password) {
		camera = new Cam({ hostname: url, username: username, password: password, port: port }, function (err) {
			cb(err, this);
		});
	},

	connect: function (cb) {
		if (!camera) return cb(new ReferenceError(cameraNotInitializedErrorMessage));
		camera.connect(function (err) {
			cb(err, this);
		});
	},

	getStatus: function (cb) {
		if (!camera) return cb(new ReferenceError(cameraNotInitializedErrorMessage));
		camera.getStatus(function (err, status) {
			// console.log("Node: GetStatus: " + JSON.stringify(status));
			status.position.z = status.position.zoom;
			cb(err, status.position);
		});
	},

	absoluteMove: function (cb, position) {
		if (!camera) return cb(new ReferenceError(cameraNotInitializedErrorMessage));
		position.zoom = position.z;
		camera.absoluteMove(position);
		cb(null, true);
	},

	continuousMove: function (cb, direction) {
		if (!camera) return cb(new ReferenceError(cameraNotInitializedErrorMessage));
		camera.continuousMove(direction, function (err, status) {
			cb(err); // Is this correct?
		});
	},

	getSnapshot: function (cb) {
		if (!camera) return cb(new ReferenceError(cameraNotInitializedErrorMessage));
		camera.getSnapshotUri(function (err, result) {
			cb(err, result.uri);
		});
	},

	getProfile: function (cb) {
		if (!camera) return cb(new ReferenceError(cameraNotInitializedErrorMessage));
		cb(null, camera.defaultProfile);
	}

};
