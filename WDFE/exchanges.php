<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Exchanges — CryptoMania</title>
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
          <li class="nav-item"><a class="nav-link" href="cryptoportfolio.php">Crypto portfolio</a></li>
          <li class="nav-item"><a class="nav-link active" href="exchanges.php">Exchanges</a></li>
          <li class="nav-item"><a class="nav-link" href="news.php">News</a></li>
        </ul>
      </div>
    </div>
  </nav>

  <h1 class="h4 mb-3">All Exchanges</h1>
  <div id="exchanges-grid" class="row g-3"></div>

  <template id="js-exchange-template">
    {{#data}}
    <div class="col-12 col-md-6 col-lg-4">
      <div class="border rounded p-3 h-100">
        <div class="d-flex justify-content-between align-items-baseline mb-2">
          <span class="badge bg-secondary">#{{rank}}</span>
          <a href="{{exchangeUrl}}" target="_blank" rel="noopener noreferrer" class="small">Website</a>
        </div>
        <div class="fw-bold mb-1">{{name}}</div>
        <div class="text-muted small">24h Volume</div>
        <div class="fw-semibold">{{volumeUsdPretty}}</div>
      </div>
    </div>
    {{/data}}
  </template>
</div>

<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/mustache.js/4.1.0/mustache.min.js"></script>
<script src="assets/js/main.js?v=6"></script>
</body>
</html>
