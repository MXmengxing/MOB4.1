<!DOCTYPE>
<html>
<head>

	<meta charset="UTF-8">

	<title>CryptoMania</title>

	<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
	<link rel="stylesheet" href="css/style.css" >

</head>
<body>

	<div class="container">
		<nav class="navbar navbar-expand-lg navbar-light bg-light">
			<div class="container-fluid">
				<a class="navbar-brand" href="#">Cryptomania</a>
				<button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
				<span class="navbar-toggler-icon"></span>
				</button>
				<div class="collapse navbar-collapse" id="navbarNav">
				<ul class="navbar-nav">
					<li class="nav-item">
						<a class="nav-link active" href="index.php">Home</a>
					</li>
					<li class="nav-item">
						<a class="nav-link" href="cryptoportfolio.php">Crypto portfolio</a>
					</li>
				</ul>
				</div>
			</div>
		</nav>


		<table class="table" id="crypto-folio-table">
			<thead>
				<tr>
					<th>Id</th>
					<th>Bought on</th>
					<th>Name</th>
					<th>Price</th>
					<th>Amount</th>
					
					<th>Total</th>
					<th>Save</th>
					<th>Delete</th>
				</tr>
			</thead>
			<tbody>

			</tbody>
			<tfoot>
				<tr>
					<td></td>
					<td></td>
					<td></td>
					<td></td>
					<td></td>
					<td id="total-value"></td>
					<td></td>
					<td></td>
				</tr>
			</tfoot>
		</table>
	</div>

	<template id="coins-cryptofolio-template">
		{{#.}}
			<tr>
				<td>{{id}}</td>
				<td>{{bought_on}}</td>
				<td>{{name}}</td>
				<td >{{price}}</td>
				<td><input type="number" value="{{amount}}" class="amount-input" /></td>
				<td class="price-total">{{totalValue}}</td>
				<td><button type="button" class="btn btn-warning save-coin-btn" value="{{id}}">Save</button></td>
				<td><button type="button" class="btn btn-danger" value="{{id}}">Delete</button></td>
			</tr>
		{{/.}}
	</template>


	<!-- jQuery -->
	<script src="https://code.jquery.com/jquery-3.5.1.min.js" integrity="sha256-9/aliU8dGd2tb6OSsuzixeV4y/faTqgFtohetphbbj0=" crossorigin="anonymous"></script>

	<!-- Bootstrap -->
	<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js" integrity="sha384-LtrjvnR4Twt/qOuYxE721u19sVFLVSA4hf/rRt6PrZTmiPltdZcI7q7PXQBYTKyf" crossorigin="anonymous"></script>	

	<!-- Mustache JS -->
	<script src="https://cdnjs.cloudflare.com/ajax/libs/mustache.js/2.3.0/mustache.js"></script>

	<!-- Chart JS -->
	<script src="https://cdn.jsdelivr.net/npm/chart.js@2.8.0"></script>
    
    <!-- Custom js  -->
	<script src="js/main.js"></script>
</body>
</html>