<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width,initial-scale=1">
  <title>CryptoMania - All Coins</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
  <link rel="stylesheet" href="./assets/css/main.css">
</head>
<body>
<nav class="navbar navbar-expand-lg navbar-dark bg-dark">
  <div class="container">
    <a class="navbar-brand" href="./index.php">CryptoMania</a>
    <div class="ms-auto"></div>
  </div>
</nav>
<main class="container my-4">
  <div class="d-flex align-items-center justify-content-between">
    <h1 class="h3 mb-3">Crypto Assets</h1>
    <div class="text-muted small">Data: CoinCap API</div>
  </div>
  <div id="feedback"></div>
  <div id="loader" class="d-flex align-items-center">
    <div class="spinner"></div>
    <span class="ms-2">Loading data...</span>
  </div>
  <div class="table-responsive">
    <table class="table table-sm align-middle">
      <thead class="table-light">
      <tr>
        <th style="width:44px;">Icon</th>
        <th>Symbol</th>
        <th>Name</th>
        <th class="text-end">Price</th>
        <th class="text-end">Market Cap</th>
        <th class="text-end">24h Volume</th>
      </tr>
      </thead>
      <tbody id="coin-table-body"></tbody>
    </table>
  </div>
</main>
<script id="tpl-coin-row" type="x-tmpl-mustache">
<tr>
  <td>
    <img src="{{iconUrl}}" alt="{{symbol}}" width="24" height="24" onerror="this.src='https://via.placeholder.com/24'">
  </td>
  <td><a href="./coin.php?id={{id}}">{{symbol}}</a></td>
  <td>{{name}}</td>
  <td class="text-end">{{pricePretty}}</td>
  <td class="text-end">{{marketCapPretty}}</td>
  <td class="text-end">{{volume24hPretty}}</td>
</tr>
</script>
<script src="https://cdn.jsdelivr.net/npm/jquery@3.7.1/dist/jquery.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/mustache@4.2.0/mustache.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
<script src="./assets/js/main.js"></script>
</body>
</html>
