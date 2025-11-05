@extends('layouts.layout')

@section('title','Producten')

@section('content')
  <div class="page-head" style="display:flex;align-items:center;justify-content:space-between;">
    <h1>Producten</h1>

    <div>
      {{-- Filter op categorie --}}
      <form method="GET" action="{{ route('products.index') }}" style="display:inline-block;margin-right:12px;">
        <select name="category" onchange="this.form.submit()">
          <option value="">— Alle categorieën —</option>
          @foreach($categories as $c)
            <option value="{{ $c->id }}" @selected($categoryId == $c->id)>
              {{ $c->name }}
            </option>
          @endforeach
        </select>
        @if($categoryId)
          <a href="{{ route('products.index') }}" style="margin-left:6px;">Reset</a>
        @endif
      </form>

      <a class="btn btn-primary" href="{{ route('products.create') }}">Nieuw product</a>
    </div>
  </div>

  <div class="card">
    <table class="table" style="width:100%;border-collapse:collapse;">
      <thead>
        <tr style="text-align:left;border-bottom:1px solid #ddd;">
          <th>Afbeelding</th>
          <th>Naam</th>
          <th>Categorie</th>
          <th>Prijs (€)</th>
          <th style="width:200px;">Acties</th>
        </tr>
      </thead>
      <tbody>
        @forelse($products as $p)
          <tr style="border-bottom:1px solid #eee;">
            <td>
              @if($p->image)
                <img src="{{ asset('storage/'.$p->image) }}" alt="{{ $p->name }}" style="height:60px;border-radius:6px;">
              @endif
            </td>
            <td><a href="{{ route('products.show', $p) }}">{{ $p->name }}</a></td>
            <td>{{ $p->category?->name ?? '—' }}</td>
            <td>€ {{ number_format($p->price, 2) }}</td>
            <td>
              <a href="{{ route('products.edit', $p) }}" class="btn btn-secondary">Bewerken</a>
              <form action="{{ route('products.destroy', $p) }}" method="POST" style="display:inline-block;margin-left:6px;">
                @csrf
                @method('DELETE')
                <button class="btn btn-danger" onclick="return confirm('Weet je zeker dat je dit product wilt verwijderen?')">Verwijderen</button>
              </form>
            </td>
          </tr>
        @empty
          <tr><td colspan="5" style="padding:12px;">Geen producten gevonden.</td></tr>
        @endforelse
      </tbody>
    </table>

    <div style="display:flex;justify-content:center;margin-top:20px">
      {!! $products->links('pagination::bootstrap-4') !!}
    </div>

  </div>
@endsection
