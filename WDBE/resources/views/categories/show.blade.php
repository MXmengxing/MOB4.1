@extends('layouts.layout')

@section('title', $category->name)

@section('content')
  <div class="page-head">
    <h1>{{ $category->name }}</h1>
    <a class="btn btn-secondary" href="{{ route('categories.index') }}">← Terug naar categorieën</a>
  </div>

  @if($products->count())
    <div class="card">
      <table class="table">
        <thead>
          <tr>
            <th style="width:100px;">ID</th>
            <th>Naam</th>
            <th>Prijs</th>
            <th style="width:240px;">Acties</th>
          </tr>
        </thead>
        <tbody>
          @foreach($products as $p)
            <tr>
              <td>{{ $p->id }}</td>
              <td>
                <a href="{{ route('products.show', $p) }}">{{ $p->name }}</a>
              </td>
              <td>€ {{ number_format($p->price, 2) }}</td>
              <td>
                @auth
                  <a class="btn btn-secondary" href="{{ route('products.edit', $p) }}">Bewerken</a>
                  <form action="{{ route('products.destroy', $p) }}" method="POST" style="display:inline-block;margin-left:8px;">
                    @csrf
                    @method('DELETE')
                    <button class="btn btn-danger"
                            onclick="return confirm('Weet je zeker dat je dit product wilt verwijderen?')">
                      Verwijderen
                    </button>
                  </form>
                @endauth
              </td>
            </tr>
          @endforeach
        </tbody>
      </table>

      <div class="pagination">
        {{ $products->links() }}
      </div>
    </div>
  @else
    <p>Geen producten gevonden in deze categorie.</p>
  @endif
@endsection
