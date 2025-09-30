// Function to open the modal and change the info of the coin
function getCoinInfo(selectedButton) {

	//get the value from the table
	var cryptoName = $(selectedButton).closest("tr").find(".crypto-name").text();
	var cryptoPrice = $(selectedButton).closest("tr").find(".crypto-price").text();

	//place the crypto name and price in an object
	selectedInfo = {coinName: cryptoName, coinPrice: cryptoPrice}

	//step 1 get the template

	//step 2 Render output with Mustache.js
	
	//step 3 append the data to the body


}


//Function to add a coin to you cryptofolio
function addCoin() {

	//coinName
	var coinName = $("#coin-name").text();
	//coinPrice

	//amountCoins

	//totalValue

	$.ajax({
		type: "POST",
		url: "includes/add_coins_db.php",
		data: {
			coin_name: coinName,
			//coinPrice
			
			//amountCoins
			
			//totalValue
			

		},

		success: function (data) {

			//see the console in your browser
			console.log(data);

		}
	})
}


//Get all coins from the DB for your cryptofolio
function getAllCoinsPortfolio() {

	$.ajax({
		type: "GET",
		url: "includes/get_coins_db.php",
		dataType: "json",

		success: function (data) {

			//An array is returned
			console.log(data);

			//Use mustache to create the table with data (See previous lessons)

			//step 1 get the template


			//step 2 Render output with Mustache.js


			//step 3 append the data to the body

		}
	})
}


//Save coin function
function saveCoin(getSaveButton) {

	coinId = $(getSaveButton).attr("value");

	console.log(coinId);
}

//Delete coin function (create the delete function below this line)


$(document).ready(function () {

	//Open modal and get coin info
	$(document).on("click", ".btn-open-modal-cryptofolio", function () {

		getCoinInfo(this);

	});

	//Add a coin to the database
	$(document).on("click", "#js-add-coin-btn", function () {
		addCoin();
	});
	

	//Get all coins from the database (used for the cryptofolio.php page)
	getAllCoinsPortfolio();


	//On click event to update the amount of coins
	$(document).on("click", ".save-coin-btn", function () {
		saveCoin(this);
	});

	//Create the delete event here




});
