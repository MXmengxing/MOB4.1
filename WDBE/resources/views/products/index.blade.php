<!doctype html>
<html lang="nl">
<head>
  <meta charset="utf-8">
  <title>Producten</title>
</head>
<body>
  <h1>Producten</h1>

  <p><a href="{{ route('products.create') }}">Nieuw product</a></p>

  <table border="1" cellpadding="8" cellspacing="0" width="100%">
    <thead>
      <tr>
        <th>ID</th>
        <th>Afbeelding</th>
        <th>Naam</th>
        <th>Prijs</th>
        <th>Acties</th>
      </tr>
    </thead>
    <tbody>
      @forelse(($products ?? []) as $p)
        <tr>
          <td>{{ $p->id }}</td>
          <td>
            @if($p->image)
              <img src="{{ asset('storage/'.$p->image) }}" alt="{{ $p->name }}" style="height:60px">
            @endif
          </td>
          <td><a href="{{ route('products.show', $p) }}">{{ $p->name }}</a></td>
          <td>€ {{ number_format($p->price, 2) }}</td>
          <td>
            <a href="{{ route('products.edit', $p) }}">Bewerken</a>
            <form action="{{ route('products.destroy', $p) }}" method="POST" style="display:inline">
              @csrf @method('DELETE')
              <button onclick="return confirm('Weet je zeker dat je dit wilt verwijderen?')">Verwijderen</button>
            </form>
          </td>
        </tr>
      @empty
        <tr><td colspan="4">Geen producten gevonden.</td></tr>
      @endforelse
    </tbody>
  </table>
</body>
</html>
