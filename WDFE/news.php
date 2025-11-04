<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Crypto News — CryptoMania</title>
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
          <li class="nav-item"><a class="nav-link" href="exchanges.php">Exchanges</a></li>
          <li class="nav-item"><a class="nav-link active" href="news.php">News</a></li>
        </ul>
      </div>
    </div>
  </nav>

  <h1 class="h4 mb-3">Latest Crypto News</h1>
  <div class="row g-3" id="news-grid"></div>

  <template id="js-news-template">
    {{#articles}}
    <div class="col-12 col-md-6">
      <article class="border rounded p-3 h-100 d-flex flex-column">
        <h2 class="h5"><a href="{{url}}" target="_blank" rel="noopener noreferrer">{{title}}</a></h2>
        <p class="text-muted small mb-2">{{source.name}} — {{publishedAtPretty}}</p>
        <p class="flex-grow-1">{{description}}</p>
        {{#urlToImage}}
        <img src="{{urlToImage}}" alt="" class="img-fluid rounded mt-2">
        {{/urlToImage}}
      </article>
    </div>
    {{/articles}}
  </template>
</div>

<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/mustache.js/4.1.0/mustache.min.js"></script>
<script src="assets/js/main.js?v=6"></script>
</body>
</html>
