import addFullscreen from './fullscreen.js';
import addLabel from './label.js';
import './grid.js';

//////// Labels ////////

// addLabelElement(".container, .content", "Test");
addLabel(".video-frame", ".video-frame");
addLabel(".content", "Content");

//////// Fullscreen ////////
// Fullscreen button defaults to the 'f' key.

addFullscreen("f");
