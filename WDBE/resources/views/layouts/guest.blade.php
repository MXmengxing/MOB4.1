<!DOCTYPE html>
<html lang="{{ str_replace('_', '-', app()->getLocale()) }}">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="csrf-token" content="{{ csrf_token() }}">

    <title>{{ config('app.name', 'Laravel') }}</title>

    <link rel="preconnect" href="https://fonts.bunny.net">
    <link href="https://fonts.bunny.net/css?family=figtree:400,500,600&display=swap" rel="stylesheet" />

    @vite(['resources/css/app.css', 'resources/js/app.js'])
    <link rel="stylesheet" href="{{ asset('css/site.css') }}">
  </head>
  <body class="font-sans text-gray-900 antialiased" style="background:var(--bg)">
    @include('partials.nav')

    <main class="container">
      @include('partials.alerts')

      <div class="min-h-screen flex flex-col sm:justify-center items-center pt-6 sm:pt-0 bg-gray-100">
        <div>
          <a href="/"><x-application-logo class="w-20 h-20 fill-current text-gray-500" /></a>
        </div>

        <div class="w-full sm:max-w-md mt-6 px-6 py-4 bg-white shadow-md overflow-hidden sm:rounded-lg card" style="max-width:560px;margin:0 auto;">
          {{ $slot }}
        </div>
      </div>
    </main>

    <footer>
      <div class="container">&copy; {{ date('Y') }} Mijn Laravel Site</div>
    </footer>
  </body>
</html>
