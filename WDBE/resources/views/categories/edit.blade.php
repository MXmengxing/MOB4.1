@extends('layouts.layout')

@section('title','Categorie bewerken')

@section('content')
  <div class="page-head">
    <h1>Categorie bewerken</h1>
    <a class="btn btn-secondary" href="{{ route('categories.index') }}">← Terug</a>
  </div>

  <div class="card">
    <form method="POST" action="{{ route('categories.update', $category) }}">
      @csrf
      @method('PUT')

      <div class="form-group">
        <label for="name">Naam *</label>
        <input id="name" type="text" name="name" value="{{ old('name', $category->name) }}" required class="form-control">
        @error('name')
          <div class="text-danger">{{ $message }}</div>
        @enderror
      </div>

      <div style="margin-top:16px;">
        <button type="submit" class="btn btn-primary">Opslaan</button>
        <a class="btn btn-secondary" href="{{ route('categories.index') }}">Annuleren</a>
      </div>
    </form>
  </div>
@endsection
