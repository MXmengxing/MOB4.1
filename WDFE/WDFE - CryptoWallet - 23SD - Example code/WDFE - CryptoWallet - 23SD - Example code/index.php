<!DOCTYPE>
<html>
<head>

	<meta charset="UTF-8">

	<title>Cryptofolio</title>

	<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
	<link rel="stylesheet" href="css/style.css" >

</head>
<body>

	<!-- Modal -->
	<div class="modal fade" id="exampleModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
	<div class="modal-dialog">
		<div class="modal-content">

		<div id="modal-content-cryptofolio">

		</div>

		<div class="modal-footer">
			<button type="button" id="closeModal" class="btn btn-secondary" data-dismiss="modal">Close</button>
		</div>
		</div>
	</div>
	</div>


	<!-- Modal -->
	<template id="cryptofolio-modal-template">
		<div class="modal-header">
		<h5 class="modal-title" id="exampleModalLabel"><span id="coin-name">{{coinName}}</span></h5>
		<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
		</div>
		<div class="modal-body">
			Current price $ <span id="coin-price">{{coinPrice}}</span>
			<br />
			Amount: <input type="number" id="amount-coins">
			<br />
			Total: <span id="total-value">0</span>
			<br />
			<button type="button" class="btn btn-primary" id="js-add-coin-btn">Add to cryptofolio</button>
		</div>
	</template>
	
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

		<table class="table">
			<thead>
				<tr>
					<th>Short</th>
					<th>Name</th>
					<th>Price</th>
					<th>Marketcap</th>
					<th>%24h</th>
					<th></th>
				</tr>
			</thead>
			<tbody>
				<tr>
					<td>BTC</td>
					<td class="crypto-name">Bitcoin</td>
					<td class="crypto-price">96000.2</td>
					<td>127825554915</td>
					<td>-2.86</td>
					<td>
						<button type="button" class="btn btn-primary btn-open-modal-cryptofolio" data-bs-toggle="modal" data-bs-target="#exampleModal">
							Add to wallet
						</button>
					</td>
				</tr>
				<tr>
					<td>ETH</td>
					<td class="crypto-name">Ethereum</td>
					<td class="crypto-price">2300.652</td>
					<td>127825554915</td>
					<td>-2.86</td>
					<td>
						<button type="button" class="btn btn-primary btn-open-modal-cryptofolio" data-bs-toggle="modal" data-bs-target="#exampleModal">
							Add to wallet
						</button>
					</td>
				</tr>
				<tr>
					<td>XRP</td>
					<td class="crypto-name">Ripple</td>
					<td class="crypto-price">1.20</td>
					<td>25513937551.275097</td>
					<td>-1.71</td>
					<td>
						<button type="button" class="btn btn-primary btn-open-modal-cryptofolio" data-bs-toggle="modal" data-bs-target="#exampleModal">
							Add to wallet
						</button>
					</td>
				</tr>
			</tbody>
		</table>
	</div>

	<!-- jQuery -->
	<script src="https://code.jquery.com/jquery-3.6.1.min.js" integrity="sha256-o88AwQnZB+VDvE9tvIXrMQaPlFFSUTR+nldQm1LuPXQ=" crossorigin="anonymous"></script>

	<!-- Bootstrap -->
	<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM" crossorigin="anonymous"></script>

	<!-- Mustache JS -->
	<script src="https://cdnjs.cloudflare.com/ajax/libs/mustache.js/4.1.0/mustache.min.js" integrity="sha512-HYiNpwSxYuji84SQbCU5m9kHEsRqwWypXgJMBtbRSumlx1iBB6QaxgEBZHSHEGM+fKyCX/3Kb5V5jeVXm0OglQ==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>

	<!-- Chart JS -->
	<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    
    <!-- Custom js  -->
	<script src="js/main.js"></script>
	
</body>
</html>