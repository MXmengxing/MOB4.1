//example API (Docs): https://api.pokemontcg.io/

//get all characters
function getAllCharacters() {

	$.ajax({
		type: "GET",
		dataType: "json",
		url: "https://api.pokemontcg.io/v2/cards",

		success: function (data) {

			characters = data;

			console.log(characters);

			//get Template
			var pokemonTemplate = $("#js-pokemon-template").html();


			//Render output with Mustache (template, data)
			var renderTemplate = Mustache.render(pokemonTemplate, characters);


			//add the data to your HTML
			$("#all-characters-table tbody").append(renderTemplate);
		


		}

	});

}



$(document).ready(function () {


	getAllCharacters();



});


