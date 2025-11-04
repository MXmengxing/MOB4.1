<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Cryptofolio</title>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet">
  <link rel="stylesheet" href="assets/css/style.css">
</head>
<body>
<div class="container py-4">
  <nav class="navbar navbar-expand-lg navbar-light bg-light mb-3">
    <div class="container-fluid">
      <a class="navbar-brand" href="#">Cryptomania</a>
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
          <li class="nav-item"><a class="nav-link" href="index.php">Home</a></li>
          <li class="nav-item"><a class="nav-link active" href="cryptoportfolio.php">Crypto portfolio</a></li>
          <li class="nav-item"><a class="nav-link" href="exchanges.php">Exchanges</a></li>
          <li class="nav-item"><a class="nav-link" href="news.php">News</a></li>
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
    <tbody></tbody>
    <tfoot>
      <tr>
        <td></td><td></td><td></td><td></td><td></td>
        <td id="total-value"></td><td></td><td></td>
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
      <td>{{price}}</td>
      <td><input type="number" value="{{amount}}" class="amount-input" /></td>
      <td class="price-total">{{totalValue}}</td>
      <td><button type="button" class="btn btn-warning save-coin-btn" value="{{id}}">Save</button></td>
      <td><button type="button" class="btn btn-danger delete-coin-btn" value="{{id}}">Delete</button></td>
    </tr>
  {{/.}}
</template>

<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/mustache.js/4.1.0/mustache.min.js"></script>
<script src="assets/js/main.js"></script>
</body>
</html>
