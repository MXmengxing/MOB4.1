@extends('layouts.layout')

@section('title', 'Home')

@section('content')
<section style="padding:28px 20px;background:#eef6f9;border-radius:12px;">
  <h2 style="margin:0 0 8px 0;">Welcome to Your Laravel CRUD App</h2>
  <p style="margin:0 0 16px 0;line-height:1.6;">
    This is the starting point of your assignment. You will build a product management system.
  </p>
  <a href="{{ route('about') }}" style="display:inline-block;padding:10px 14px;border-radius:8px;background:#0d6efd;color:#fff;text-decoration:none;">Learn more (About)</a>
</section>

<div style="height:18px;"></div>

<section style="display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:16px;">
  <article style="border:1px solid #eee;border-radius:12px;padding:16px;">
    <h3 style="margin-top:0;">MVC Structure</h3>
    <p style="margin:0;line-height:1.6;">Models, Views and Controllers are clearly separated.</p>
  </article>
  <article style="border:1px solid #eee;border-radius:12px;padding:16px;">
    <h3 style="margin-top:0;">Blade Templates</h3>
    <p style="margin:0;line-height:1.6;">Reusable layouts with <code>@@extends</code>, <code>@@section</code> and <code>@@yield</code>.</p>
  </article>
  <article style="border:1px solid #eee;border-radius:12px;padding:16px;">
    <h3 style="margin-top:0;">CRUD Ready</h3>
    <p style="margin:0;line-height:1.6;">Product listing, create, edit and delete are coming next.</p>
  </article>
</section>
@endsection
