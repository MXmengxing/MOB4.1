<?php

namespace App\Http\Controllers;

use App\Models\Category;
use Illuminate\Http\Request;

class CategoryController extends Controller
{
    public function __construct()
    {
        // Alleen ingelogde gebruikers mogen categorieën beheren
        $this->middleware('auth');
    }

    /**
     * Toon een overzicht van alle categorieën.
     */
    public function index()
    {
        // Haal alle categorieën op, inclusief het aantal producten per categorie
        $categories = Category::withCount('products')->orderBy('name')->paginate(10);
        return view('categories.index', compact('categories'));
    }

    /**
     * Toon het formulier om een nieuwe categorie aan te maken.
     */
    public function create()
    {
        return view('categories.create');
    }

    /**
     * Sla een nieuwe categorie op in de database.
     */
    public function store(Request $request)
    {
        // Valideer de invoer
        $data = $request->validate([
            'name' => 'required|max:100',
        ]);

        // Maak de categorie aan
        Category::create($data);

        return redirect()
            ->route('categories.index')
            ->with('success', 'Categorie succesvol aangemaakt.');
    }

    /**
     * Toon een specifieke categorie en de producten die erbij horen.
     */
    public function show(Category $category)
    {
        // Laad de producten die aan deze categorie gekoppeld zijn
        $products = $category->products()->with('images')->paginate(10);
        return view('categories.show', compact('category', 'products'));
    }

    /**
     * Toon het formulier om een bestaande categorie te bewerken.
     */
    public function edit(Category $category)
    {
        return view('categories.edit', compact('category'));
    }

    /**
     * Werk een bestaande categorie bij in de database.
     */
    public function update(Request $request, Category $category)
    {
        // Valideer de invoer
        $data = $request->validate([
            'name' => 'required|max:100',
        ]);

        // Update de categorie
        $category->update($data);

        return redirect()
            ->route('categories.index')
            ->with('success', 'Categorie succesvol bijgewerkt.');
    }

    /**
     * Verwijder een categorie (en automatisch de producten die eraan gekoppeld zijn).
     */
    public function destroy(Category $category)
    {
        // Dankzij de foreign key-relatie (ON DELETE CASCADE)
        // worden alle producten van deze categorie ook verwijderd
        $category->delete();

        return redirect()
            ->route('categories.index')
            ->with('success', 'Categorie en gekoppelde producten succesvol verwijderd.');
    }
}
