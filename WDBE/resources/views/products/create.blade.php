@extends('layouts.layout')

@section('title','Nieuw product')

@section('content')
  <h1>Nieuw product</h1>

  <form method="POST" action="{{ route('products.store') }}" enctype="multipart/form-data" class="card">
    @csrf

    <div class="row">
      <div class="col">
        <div class="field">
          <label>Naam *</label>
          <input type="text" name="name" value="{{ old('name') }}" required>
          @error('name')
            <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div>
          @enderror
        </div>
      </div>
      <div class="col">
        <div class="field">
          <label>Prijs (€) *</label>
          <input type="number" step="0.01" min="0" name="price" value="{{ old('price') }}" required>
          @error('price')
            <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div>
          @enderror
        </div>
      </div>
    </div>

    <div class="field">
      <label>Categorie *</label>
      <select name="category_id" required>
        <option value="">— Kies categorie —</option>
        @foreach($categories as $c)
          <option value="{{ $c->id }}" @selected(old('category_id') == $c->id)>{{ $c->name }}</option>
        @endforeach
      </select>
      @error('category_id')
        <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div>
      @enderror
    </div>

    <div class="field">
      <label>Omschrijving *</label>
      <textarea name="description" rows="5" required>{{ old('description') }}</textarea>
      @error('description')
        <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div>
      @enderror
    </div>

    {{-- Omslagafbeelding（封面） --}}
    <div class="field">
      <label>Afbeelding *</label>
      <input type="file" name="image" accept="image/*" required>
      @error('image')
        <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div>
      @enderror
    </div>

    {{-- Extra afbeeldingen（相册，多选，追加保存） --}}
    <div class="field">
      <label>Extra afbeeldingen (meerdere)</label>
      <input type="file" name="images[]" accept="image/*" multiple>
      @error('images.*')
        <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div>
      @enderror
    </div>

    <div style="display:flex;gap:8px;">
      <button class="btn btn-primary" type="submit">Opslaan</button>
      <a class="btn btn-secondary" href="{{ route('products.index') }}">Terug</a>
    </div>
  </form>
@endsection
