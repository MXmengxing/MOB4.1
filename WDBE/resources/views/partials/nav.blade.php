<header>
  <div class="container nav">
    <a class="brand" href="{{ route('home') }}">Mijn Laravel Site</a>
    <nav class="menu">
      <a href="{{ route('home') }}" class="{{ request()->routeIs('home') ? 'active' : '' }}">Home</a>
      <a href="{{ route('about') }}" class="{{ request()->routeIs('about') ? 'active' : '' }}">Over</a>
      <a href="{{ route('products.index') }}" class="{{ request()->routeIs('products.*') ? 'active' : '' }}">Producten</a>
      <a href="{{ route('categories.index') }}" class="{{ request()->routeIs('categories.*') ? 'active' : '' }}">Categorieën</a>
      @auth
      <a href="{{ route('products.create') }}" class="btn btn-primary" style="margin-left:16px">Nieuw product</a>
        <form method="POST" action="{{ route('logout') }}" style="display:inline;margin-left:16px">
          @csrf
          <button class="btn btn-secondary" type="submit">Uitloggen</button>
        </form>
      @else
        <a href="{{ route('login') }}" style="margin-left:16px">Inloggen</a>
        <a href="{{ route('register') }}" class="btn btn-secondary">Registreren</a>
      @endauth
    </nav>
  </div>
</header>
