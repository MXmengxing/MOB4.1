function getChartInfo() {

	$.ajax({
		type: "GET",
		dataType: "json",
		//this wil be dynamic, depends on which coin you click
		url: "https://api.coincap.io/v2/assets/bitcoin/history?interval=d1",

		success: function (historicalData) {

			console.log(historicalData);

			//Generate the chart with the generated data
			generateChart(dateArray, priceArray)

		}
	})
}

//generates the chart with the historical information from the past year
function generateChart(chartDate, chartPrice) {

	var ctx = document.getElementById('coin-history-chart').getContext('2d');

	var chart = new Chart(ctx, {
		// The type of chart we want to create
		type: 'line',

		// The data for our dataset
		data: {
			labels: chartDate,
			datasets: [{
				type: "line",
				label: "Price",
				borderColor: '#3e95cd',
				data: chartPrice,
			}]
		},

		// Configuration options go here
		options: {
			scales: {
				xAxes: [{
					display: true,
					scaleLabel: {
						display: true,
						labelString: 'Date'
					}
				}],
				yAxes: [{
					display: true,
					scaleLabel: {
						display: true,
						labelString: 'Price'
					}
				}]
			},
			elements: { point: { radius: 0 } }
		}
	});
}

$(document).ready(function () {

	//The chart is loaded when the page is parsed
	getChartInfo();

});