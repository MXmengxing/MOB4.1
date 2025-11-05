<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\ProductController;
use App\Http\Controllers\CategoryController;
use App\Http\Controllers\ProfileController;

// 首页与关于页面
Route::view('/', 'home')->name('home');
Route::view('/about', 'about')->name('about');

// ✅ Product CRUD
Route::resource('products', ProductController::class);

// ✅ Category CRUD
Route::resource('categories', CategoryController::class);

// ✅ 登录后默认跳转到产品页
Route::get('/dashboard', fn() => redirect()->route('products.index'))
    ->middleware(['auth', 'verified'])
    ->name('dashboard');

// ✅ 个人资料页面（保留 Breeze 的）
Route::middleware('auth')->group(function () {
    Route::get('/profile', [ProfileController::class, 'edit'])->name('profile.edit');
    Route::patch('/profile', [ProfileController::class, 'update'])->name('profile.update');
    Route::delete('/profile', [ProfileController::class, 'destroy'])->name('profile.destroy');
});

require __DIR__.'/auth.php';

Route::delete('/products/images/{image}', [ProductController::class, 'destroyImage'])
    ->middleware('auth')
    ->name('products.images.destroy');

