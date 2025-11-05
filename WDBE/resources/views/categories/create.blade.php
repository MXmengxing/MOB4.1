@extends('layouts.layout')

@section('title','Nieuwe categorie')

@section('content')
  <div class="page-head">
    <h1>Nieuwe categorie</h1>
    <a class="btn btn-secondary" href="{{ route('categories.index') }}">← Terug</a>
  </div>

  <div class="card">
    <form method="POST" action="{{ route('categories.store') }}">
      @csrf

      <div class="form-group">
        <label for="name">Naam *</label>
        <input id="name" type="text" name="name" value="{{ old('name') }}" required class="form-control">
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
