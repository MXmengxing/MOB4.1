<?php

namespace App\Http\Controllers;

use App\Models\Product;
use App\Models\Category;
use App\Models\ProductImage;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Storage;

class ProductController extends Controller
{
    // Alleen index/show publiek; overige acties na login (indien gewenst):
    // public function __construct()
    // {
    //     $this->middleware('auth')->except(['index','show']);
    // }

    /** Lijst met producten, optioneel gefilterd op categorie (?category=ID) */
    public function index()
    {
        $categoryId = request('category');
        $categories = Category::orderBy('name')->get();

        $q = Product::with('category')->latest();
        if ($categoryId) {
            $q->where('category_id', $categoryId);
        }

        $products = $q->paginate(10)->withQueryString();

        return view('products.index', compact('products','categories','categoryId'));
    }

    /** Formulier: nieuw product (met categorie-keuze) */
    public function create()
    {
        $categories = Category::orderBy('name')->get();
        return view('products.create', compact('categories'));
    }

    /** Opslaan: basisvelden + categorie + (optionele) omslag + (optionele) extra afbeeldingen */
    public function store(Request $request)
    {
        $data = $request->validate([
            'name'        => 'required|max:255',
            'description' => 'required',
            'price'       => 'required|numeric|min:0',
            'category_id' => 'required|exists:categories,id',
            'image'       => 'nullable|image|max:5120',   // omslag (optioneel)
            'images.*'    => 'nullable|image|max:5120',   // extra foto's (optioneel, meerdere)
        ]);

        if ($request->hasFile('image')) {
            $data['image'] = $request->file('image')->store('products', 'public');
        }

        $product = Product::create($data);

        // append album-foto's
        if ($request->hasFile('images')) {
            foreach ($request->file('images') as $file) {
                $path = $file->store('products', 'public');
                $product->images()->create(['path' => $path]);
            }
        }

        return redirect()->route('products.index')->with('success','Product aangemaakt');
    }

    /** Detailpagina */
    public function show(Product $product)
    {
        $product->load(['category','images']);
        return view('products.show', compact('product'));
    }

    /** Formulier: product bewerken (met categorie-keuze) */
    public function edit(Product $product)
    {
        $categories = Category::orderBy('name')->get();
        $product->load('images');
        return view('products.edit', compact('product','categories'));
    }

    /** Bijwerken: basisvelden + categorie + (optionele) nieuwe omslag + (optionele) extra afbeeldingen (append) */
    public function update(Request $request, Product $product)
    {
        $data = $request->validate([
            'name'        => 'required|max:255',
            'description' => 'required',
            'price'       => 'required|numeric|min:0',
            'category_id' => 'required|exists:categories,id',
            'image'       => 'nullable|image|max:5120',   // nieuwe omslag (vervangt oud)
            'images.*'    => 'nullable|image|max:5120',   // extra foto's (append)
        ]);

        if ($request->hasFile('image')) {
            if ($product->image && Storage::disk('public')->exists($product->image)) {
                Storage::disk('public')->delete($product->image);
            }
            $data['image'] = $request->file('image')->store('products', 'public');
        }

        $product->update($data);

        // append extra foto's
        if ($request->hasFile('images')) {
            foreach ($request->file('images') as $file) {
                $path = $file->store('products', 'public');
                $product->images()->create(['path' => $path]);
            }
        }

        return redirect()->route('products.index')->with('success','Product bijgewerkt');
    }

    /** Verwijderen: product + gekoppelde bestanden */
    public function destroy(Product $product)
    {
        if ($product->image && Storage::disk('public')->exists($product->image)) {
            Storage::disk('public')->delete($product->image);
        }

        foreach ($product->images as $img) {
            if ($img->path && Storage::disk('public')->exists($img->path)) {
                Storage::disk('public')->delete($img->path);
            }
        }

        $product->delete();

        return redirect()->route('products.index')->with('success','Product verwijderd');
    }

    /** Verwijder één losse productafbeelding (album-foto) */
    public function destroyImage(ProductImage $image)
    {
        $productId = $image->product_id;

        if ($image->path && Storage::disk('public')->exists($image->path)) {
            Storage::disk('public')->delete($image->path);
        }

        $image->delete();

        return redirect()
            ->route('products.edit', $productId)
            ->with('success', 'Afbeelding verwijderd');
    }

        public function __construct()
    {
        $this->middleware('auth')->except(['index','show']);
    }
}
