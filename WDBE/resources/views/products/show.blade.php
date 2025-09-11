<!doctype html>
<html lang="nl">
<head>
  <meta charset="utf-8">
  <title>{{ $product->name }} - Product</title>
</head>
<body>
  <p><a href="{{ route('products.index') }}">← Terug naar overzicht</a></p>

  <h1>{{ $product->name }}</h1>

  @if($product->image)
    <p>
      <img src="{{ asset('storage/'.$product->image) }}"
           alt="{{ $product->name }}"
           style="max-width:300px;height:auto">
    </p>
  @endif

  <p><strong>Prijs:</strong> € {{ number_format($product->price, 2) }}</p>

  <h3>Omschrijving</h3>
  <p>{!! nl2br(e($product->description)) !!}</p>

  <p>
    <a href="{{ route('products.edit', $product) }}">Bewerken</a>
    <form action="{{ route('products.destroy', $product) }}" method="POST" style="display:inline">
      @csrf
      @method('DELETE')
      <button onclick="return confirm('Weet je zeker dat je dit wilt verwijderen?')">Verwijderen</button>
    </form>
  </p>
</body>
</html>
