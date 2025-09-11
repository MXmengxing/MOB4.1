<!doctype html>
<html lang="nl">
<head>
  <meta charset="utf-8">
  <title>Product bewerken</title>
</head>
<body>
  <p><a href="{{ route('products.index') }}">← Terug naar overzicht</a></p>

  <h1>Product bewerken</h1>

  <form method="POST" action="{{ route('products.update', $product) }}">
    @csrf
    @method('PUT')

    <p>
      <label>Naam *</label><br>
      <input type="text" name="name" value="{{ old('name', $product->name) }}" required>
    </p>

    <p>
      <label>Omschrijving *</label><br>
      <textarea name="description" rows="4" required>{{ old('description', $product->description) }}</textarea>
    </p>

    <p>
      <label>Prijs (€) *</label><br>
      <input type="number" name="price" step="0.01" min="0"
             value="{{ old('price', $product->price) }}" required>
    </p>

    <button type="submit">Opslaan</button>
    <a href="{{ route('products.index') }}">Annuleren</a>
  </form>
</body>
</html>
