<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>CryptoMania</title>
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
          <li class="nav-item"><a class="nav-link active" href="index.php">Home</a></li>
          <li class="nav-item"><a class="nav-link" href="cryptoportfolio.php">Crypto portfolio</a></li>
        </ul>
      </div>
    </div>
  </nav>

  <table class="table" id="all-characters-table">
    <thead>
      <tr>
        <th>Symbol</th>
        <th>Name</th>
        <th class="text-end">Price</th>
        <th class="text-end">Marketcap</th>
        <th class="text-end">24h Volume</th>
        <th></th>
        <th></th>
      </tr>
    </thead>
    <tbody>
      <template id="js-pokemon-template">
        {{#data}}
        <tr data-id="{{id}}" data-symbol="{{symbol}}">
          <td>{{symbol}}</td>
          <td class="crypto-name">{{name}}</td>
          <td class="text-end crypto-price">{{pricePretty}}</td>
          <td class="text-end">{{marketCapPretty}}</td>
          <td class="text-end">{{volume24hPretty}}</td>
          <td><button type="button" class="btn btn-sm btn-secondary coin-info-btn" data-id="{{id}}" data-symbol="{{symbol}}" data-bs-toggle="modal" data-bs-target="#coinModal">Info</button></td>
          <td><button type="button" class="btn btn-sm btn-primary btn-open-modal-cryptofolio" data-bs-toggle="modal" data-bs-target="#exampleModal">Add to wallet</button></td>
        </tr>
        {{/data}}
      </template>
    </tbody>
  </table>
</div>

<div class="modal fade" id="exampleModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div id="modal-content-cryptofolio"></div>
      <div class="modal-footer">
        <button type="button" id="closeModal" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>

<template id="cryptofolio-modal-template">
  <div class="modal-header">
    <h5 class="modal-title"><span id="coin-name">{{coinName}}</span></h5>
    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
  </div>
  <div class="modal-body">
    Current price $ <span id="coin-price">{{coinPrice}}</span><br>
    Amount: <input type="number" id="amount-coins"><br>
    Total: <span id="total-value">0</span><br>
    <button type="button" class="btn btn-primary" id="js-add-coin-btn">Add to cryptofolio</button>
  </div>
</template>

<div class="modal fade" id="coinModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-lg">
    <div class="modal-content">
      <div class="modal-header">
        <div class="d-flex align-items-center">
          <img id="coinModalIcon" src="" alt="" width="28" height="28" class="me-2">
          <h5 class="modal-title" id="coinModalLabel">Coin</h5>
        </div>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
      </div>
      <div class="modal-body">
        <canvas id="coin-history-chart" height="140"></canvas>
      </div>
    </div>
  </div>
</div>

<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/mustache.js/4.1.0/mustache.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

<script src="assets/js/main.js?v=4"></script>
</body>
</html>
