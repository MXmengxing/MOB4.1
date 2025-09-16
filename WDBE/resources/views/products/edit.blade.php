@extends('layouts.layout')

@section('title','Product bewerken')

@section('content')
  <h1>Product bewerken</h1>

  <form method="POST" action="{{ route('products.update',$product) }}" enctype="multipart/form-data" class="card">
    @csrf @method('PUT')

    <div class="row">
      <div class="col">
        <div class="field">
          <label>Naam *</label>
          <input type="text" name="name" value="{{ old('name',$product->name) }}" required>
          @error('name') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror
        </div>
      </div>
      <div class="col">
        <div class="field">
          <label>Prijs (€) *</label>
          <input type="number" step="0.01" min="0" name="price" value="{{ old('price',$product->price) }}" required>
          @error('price') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror
        </div>
      </div>
    </div>

    <div class="field">
      <label>Omschrijving *</label>
      <textarea name="description" rows="5" required>{{ old('description',$product->description) }}</textarea>
      @error('description') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror
    </div>

    <div class="field">
      <label>Afbeelding (optioneel)</label>
      <input type="file" name="image">
      @error('image') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror

      @if($product->image)
        <div style="margin-top:8px">
          <img class="thumb" src="{{ asset('storage/'.$product->image) }}" alt="{{ $product->name }}">
        </div>
      @endif
    </div>

    <div style="display:flex;gap:8px;align-items:center;">
      <button class="btn btn-primary" type="submit">Opslaan</button>
      <a class="btn btn-secondary" href="{{ route('products.index') }}">Annuleren</a>
    </div>
  </form>
@endsection
