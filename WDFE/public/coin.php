<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width,initial-scale=1">
  <title>CryptoMania - Coin Detail</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
  <link rel="stylesheet" href="./assets/css/main.css">
</head>
<body>
<nav class="navbar navbar-expand-lg navbar-dark bg-dark">
  <div class="container">
    <a class="navbar-brand" href="./index.php">CryptoMania</a>
    <div class="ms-auto">
      <a href="./index.php" class="btn btn-outline-light btn-sm">Back to list</a>
    </div>
  </div>
</nav>
<main class="container my-4" id="coin-page" data-coin-id="<?php echo htmlspecialchars($_GET['id'] ?? '', ENT_QUOTES, 'UTF-8'); ?>">
  <div id="feedback"></div>
  <div id="loader" class="d-flex align-items-center">
    <div class="spinner"></div>
    <span class="ms-2">Loading details...</span>
  </div>
  <div id="coin-detail" class="card d-none">
    <div class="card-body">
      <div class="d-flex align-items-center mb-3">
        <img id="coin-icon" src="" alt="" width="32" height="32" class="me-2" onerror="this.src='https://via.placeholder.com/32'">
        <h1 class="h4 mb-0"><span id="coin-name"></span> <small class="text-muted" id="coin-symbol"></small></h1>
      </div>
      <div class="row g-3">
        <div class="col-6 col-md-3">
          <div class="text-muted small">Current Price</div>
          <div class="fs-5" id="coin-price">-</div>
        </div>
        <div class="col-6 col-md-3">
          <div class="text-muted small">Market Cap</div>
          <div class="fs-5" id="coin-cap">-</div>
        </div>
        <div class="col-6 col-md-3">
          <div class="text-muted small">24h Volume</div>
          <div class="fs-5" id="coin-vol">-</div>
        </div>
        <div class="col-6 col-md-3">
          <div class="text-muted small">Supply</div>
          <div class="fs-5" id="coin-supply">-</div>
        </div>
      </div>
    </div>
  </div>
  <div class="card mt-3 d-none" id="chart-card">
    <div class="card-body">
      <h2 class="h6">Last 7 days</h2>
      <canvas id="priceChart" height="120"></canvas>
    </div>
  </div>
</main>
<script src="https://cdn.jsdelivr.net/npm/jquery@3.7.1/dist/jquery.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
<script src="./assets/js/coin.js"></script>
</body>
</html>
