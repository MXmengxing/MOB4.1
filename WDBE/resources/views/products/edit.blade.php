@extends('layouts.layout')

@section('title','Product bewerken')

@section('content')
  <h1>Product bewerken</h1>

  {{-- Bestaande album-afbeeldingen + verwijderknop --}}
  @if($product->images->count())
    <div class="card" style="margin-bottom:12px;">
      <h3 style="margin-top:0;">Bestaande afbeeldingen</h3>
      <div style="display:flex;flex-wrap:wrap;gap:12px;">
        @foreach($product->images as $img)
          <div style="border:1px solid #e5e7eb;border-radius:8px;padding:8px;display:flex;flex-direction:column;align-items:center;">
            <a href="{{ asset('storage/'.$img->path) }}" target="_blank" style="margin-bottom:8px;">
              <img src="{{ asset('storage/'.$img->path) }}" style="height:90px;border-radius:6px;object-fit:cover">
            </a>
            <form method="POST" action="{{ route('products.images.destroy', $img) }}"
                  onsubmit="return confirm('Weet je zeker dat je deze afbeelding wilt verwijderen?')">
              @csrf
              @method('DELETE')
              <button class="btn btn-danger" type="submit">Verwijderen</button>
            </form>
          </div>
        @endforeach
      </div>
    </div>
  @endif

  {{-- Huidige omslag --}}
  @if($product->image)
    <div class="card" style="margin-bottom:12px;">
      <h3 style="margin-top:0;">Huidige omslag</h3>
      <img src="{{ asset('storage/'.$product->image) }}" style="height:150px;border-radius:8px;object-fit:cover">
    </div>
  @endif

  <form method="POST" action="{{ route('products.update', $product) }}" enctype="multipart/form-data" class="card">
    @csrf
    @method('PUT')

    <div class="row">
      <div class="col">
        <div class="field">
          <label>Naam *</label>
          <input type="text" name="name" value="{{ old('name', $product->name) }}" required>
          @error('name') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror
        </div>
      </div>
      <div class="col">
        <div class="field">
          <label>Prijs (€) *</label>
          <input type="number" step="0.01" min="0" name="price" value="{{ old('price', $product->price) }}" required>
          @error('price') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror
        </div>
      </div>
    </div>

    <div class="field">
      <label>Categorie *</label>
      <select name="category_id" required>
        <option value="">— Kies categorie —</option>
        @foreach($categories as $c)
          <option value="{{ $c->id }}" @selected(old('category_id', $product->category_id) == $c->id)>{{ $c->name }}</option>
        @endforeach
      </select>
      @error('category_id') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror
    </div>

    <div class="field">
      <label>Omschrijving *</label>
      <textarea name="description" rows="5" required>{{ old('description', $product->description) }}</textarea>
      @error('description') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror
    </div>

    {{-- Nieuwe omslag (optioneel) --}}
    <div class="field">
      <label>Nieuwe omslag (optioneel)</label>
      <input type="file" name="image" accept="image/*">
      @error('image') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror
    </div>

    {{-- Extra afbeeldingen toevoegen（多图追加） --}}
    <div class="field">
      <label>Extra afbeeldingen toevoegen (meerdere)</label>
      <input type="file" name="images[]" accept="image/*" multiple>
      @error('images.*') <div style="color:#b91c1c;margin-top:6px">{{ $message }}</div> @enderror
    </div>

    <div style="display:flex;gap:8px;">
      <button class="btn btn-primary" type="submit">Opslaan</button>
      <a class="btn btn-secondary" href="{{ route('products.index') }}">Annuleren</a>
    </div>
  </form>
@endsection
