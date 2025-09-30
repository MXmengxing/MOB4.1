<!DOCTYPE html>
<html>
<head>

	<meta charset="UTF-8">

	<title>CryptoMania - Workshop - Template Engine</title>

	<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
</head>
<body>

	<table id="all-characters-table">
		<thead>
			<tr>
				<th>ID</th>
				<th>Name</th>
				<th>Rarity</th>
			</tr>
		</thead>
		<tbody>
			<template id="js-pokemon-template">
				{{#data}}
				<tr>
					<td>{{id}}</td>
					<td>{{name}}</td>
					<td>{{rarity}}</td>
				</tr>
				{{/data}}
			</template>
		</tbody>
	</table>


	

	<!-- jQuery -->
	<script src="https://code.jquery.com/jquery-3.7.1.min.js" integrity="sha256-/JqT3SQfawRcv/BIHPThkBvs0OEvtFFmqPF/lYI/Cxo="crossorigin="anonymous"></script>

	<!-- Bootstrap -->
	<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM" crossorigin="anonymous"></script>

	<!-- Mustache JS -->
	<script src="https://cdnjs.cloudflare.com/ajax/libs/mustache.js/4.1.0/mustache.min.js" integrity="sha512-HYiNpwSxYuji84SQbCU5m9kHEsRqwWypXgJMBtbRSumlx1iBB6QaxgEBZHSHEGM+fKyCX/3Kb5V5jeVXm0OglQ==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>

	<script src="js/main.js"></script>
</body>
</html>