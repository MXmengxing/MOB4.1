@extends('layouts.layout')

@section('title','Categorieën')

@section('content')
  <div class="page-head">
    <h1 style="margin:0">Categorieën</h1>
    <div style="flex:1"></div>
    <a class="btn btn-primary" href="{{ route('categories.create') }}">Nieuwe categorie</a>
  </div>

  <div class="card">
    <table class="table">
      <thead>
        <tr>
          <th style="width:80px">ID</th>
          <th>Naam</th>
          <th style="width:180px"># Producten</th>
          <th style="width:240px">Acties</th>
        </tr>
      </thead>
      <tbody>
        @forelse($categories as $c)
          <tr>
            <td>{{ $c->id }}</td>
            <td>
              <a href="{{ route('categories.show', $c) }}">{{ $c->name }}</a>
            </td>
            <td>{{ $c->products_count }}</td>
            <td>
              <a class="btn btn-secondary" href="{{ route('categories.edit', $c) }}">Bewerken</a>

              <form action="{{ route('categories.destroy', $c) }}" method="POST" style="display:inline-block;margin-left:8px">
                @csrf
                @method('DELETE')
                <button class="btn btn-danger"
                        onclick="return confirm('Weet je zeker dat je deze categorie wilt verwijderen? Alle gekoppelde producten worden ook verwijderd.')">
                  Verwijderen
                </button>
              </form>
            </td>
          </tr>
        @empty
          <tr>
            <td colspan="4">Geen categorieën gevonden.</td>
          </tr>
        @endforelse
      </tbody>
    </table>

    <div class="pagination">
      {{ $categories->links() }}
    </div>
  </div>
@endsection
