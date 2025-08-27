<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>@yield('title', 'WDBE')</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        header, footer { background: #f4f4f4; padding: 10px; }
        nav a { margin-right: 15px; text-decoration: none; color: #333; }
        nav a:hover { text-decoration: underline; }
        main { padding: 20px 0; }
    </style>
</head>
<body>
<header>
    <h1>My Laravel Site</h1>
    <nav>
        <a href="{{ route('home') }}">Home</a>
        <a href="{{ route('about') }}">About</a>
    </nav>
</header>

<main>
    @yield('content')
</main>

<footer>
    <p>&copy; {{ date('Y') }} My Laravel Site</p>
</footer>
</body>
</html>
