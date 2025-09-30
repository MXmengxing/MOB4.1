//example API (Docs): https://docs.pokemontcg.io

//get all characters
function getAllCharacters() {

	$.ajax({
		type: "GET",
		dataType: "json",
		headers: {
			'X-Api-Key': 'ADD YOUR API KEY HERE' //You can request an API key here: https://dev.pokemontcg.io/
		},
		url: "https://api.pokemontcg.io/v2/cards",

		success: function (data) {

			characters = data;

			console.log(characters.data[0]);

		}

	});

}


//function to get a single character
function getCharacter(selectedButton) {

	// Step 1: Get the selected id (use closest() and find())
	// Step 2: Create an AJAX-call with the selected character id. (see the above function for an example of an AJAX call)
	// 		Example AJAX-call: "https://api.pokemontcg.io/v2/cards/dp6-90 is just an example
	// 		More info: https://docs.pokemontcg.io/api-reference/cards/get-card
	// 		the "dp6-90" is an example because in our case it should be dynamic based on the clicked button
	// Step 3: in the success: append the data (name and image) in the modal

}


$(document).ready(function () {

	//load all characters @ loading
	getAllCharacters();

	//On click to get a character
	$("#characters-table").on("click", ".character-info-btn", function () {

		getCharacter(this);

	});


});


