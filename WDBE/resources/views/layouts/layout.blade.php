<!doctype html>
<html lang="nl">
<head>
  <meta charset="utf-8">
  <title>@yield('title','WDBE')</title>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="stylesheet" href="{{ asset('css/site.css') }}">
  @stack('head')
</head>
<body style="background:var(--bg)">
  @include('partials.nav')

  <main class="container">
    @include('partials.alerts')
    @yield('content')
  </main>

  <footer>
    <div class="container">&copy; {{ date('Y') }} Mijn Laravel Site</div>
  </footer>

  @stack('scripts')
</body>
</html>
