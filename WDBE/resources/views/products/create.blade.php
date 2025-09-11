<!doctype html>
<html lang="nl">
<head>
  <meta charset="utf-8">
  <title>Nieuw product</title>
</head>
<body>
  <h1>Nieuw product</h1>

  @if ($errors->any())
    <p style="color:#b91c1c;">
      {{ $errors->first() }}
    </p>
  @endif

  <form method="POST" action="{{ route('products.store') }}" enctype="multipart/form-data">
    @csrf

    <p>
      <label>Naam *</label><br>
      <input type="text" name="name" value="{{ old('name') }}" required>
    </p>

    <p>
      <label>Omschrijving *</label><br>
      <textarea name="description" rows="4" required>{{ old('description') }}</textarea>
    </p>

    <p>
      <label>Prijs (€) *</label><br>
      <input type="number" name="price" step="0.01" min="0" value="{{ old('price') }}" required>
    </p>

    <p>
      <label>Afbeelding *</label><br>
      <input type="file" name="image" required>
    </p>

    <button type="submit">Opslaan</button>
    <a href="{{ route('products.index') }}">Terug</a>
  </form>
</body>
</html>
