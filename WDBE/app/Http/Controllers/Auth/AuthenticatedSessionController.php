<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use App\Http\Requests\Auth\LoginRequest;
use Illuminate\Http\RedirectResponse;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\View\View;

class AuthenticatedSessionController extends Controller
{
    /**
     * Toon de loginpagina.
     */
    public function create(): View
    {
        return view('auth.login');
    }

    /**
     * Verwerk een inkomende inlogaanvraag.
     */
    public function store(LoginRequest $request): RedirectResponse
    {
        // Controleer de inloggegevens
        $request->authenticate();

        // Genereer een nieuwe sessie
        $request->session()->regenerate();

        // Na succesvolle login -> ga naar de productenpagina
        return redirect()->intended('/products');
    }

    /**
     * Log de gebruiker uit en vernietig de sessie.
     */
    public function destroy(Request $request): RedirectResponse
    {
        Auth::guard('web')->logout();

        $request->session()->invalidate();

        $request->session()->regenerateToken();

        return redirect('/');
    }
}
