@extends('layouts.layout')

@section('title', $product->name.' - Product')

@section('content')
  <div class="card">
    <p><a class="btn btn-secondary" href="{{ route('products.index') }}">← Terug naar overzicht</a></p>

    <h1 style="margin-top:0">{{ $product->name }}</h1>

    @if($product->image)
      <p><img src="{{ asset('storage/'.$product->image) }}" alt="{{ $product->name }}" style="max-width:360px;height:auto;border-radius:12px"></p>
    @endif

    <p><strong>Prijs:</strong> € {{ number_format($product->price, 2) }}</p>

    <h3>Omschrijving</h3>
    <p>{!! nl2br(e($product->description)) !!}</p>

    <div style="display:flex;gap:8px;margin-top:12px">
      <a class="btn btn-secondary" href="{{ route('products.edit',$product) }}">Bewerken</a>
      <form action="{{ route('products.destroy',$product) }}" method="POST" onsubmit="return confirm('Weet je zeker dat je dit wilt verwijderen?')">
        @csrf @method('DELETE')
        <button class="btn btn-danger" type="submit">Verwijderen</button>
      </form>
    </div>
  </div>
@endsection
