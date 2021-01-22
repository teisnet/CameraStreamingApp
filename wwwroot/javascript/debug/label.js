/*
* Label
*
* Adds small labels in the top left corner of an element displaying a name and its current size.
*
* Place debug.css before custom stylesheets in order to not override custom 'position' settings.
*
* Usage:
*
* addLabel(".container", "Container");
* addLabel(".content", "Content");
*
* Add multiple elements at once (they will have the same name):
*
* addLabel(".container, .content", "Test");
*
*/

const elementsArray = [];


function addLabel(query, name) {

	let elements = $(query);
	elements.each(function(index) {
		let element = $(this);
		//console.log(element.attr("class"));
		let width = element.width();
		let height = element.height();
		element.addClass("debug-parent");
		let label = $(`<div class=\"debug-label\">${name}: ${width.toFixed(1)} x ${height.toFixed(1)}</div>`);
		label.appendTo(element);

		elementsArray.push({ name: name, element: element, label: label});


	});
}

function resizeElements()
{
	elementsArray.forEach(function (resizeItem) {
		console.log(resizeItem.name);

		let width = resizeItem.element.width();
		let height = resizeItem.element.height();
		resizeItem.label.text(`${resizeItem.name}: ${width.toFixed(1)} x ${height.toFixed(1)}`);
	});
}


$(window).resize(function () {
	resizeElements();
});

export default addLabel;
