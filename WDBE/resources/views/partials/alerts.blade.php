@if(session('success'))
  <div class="alert ok">{{ session('success') }}</div>
@endif
@if ($errors->any())
  <div class="alert err">
    <ul style="margin:0;padding-left:18px">
      @foreach ($errors->all() as $e)<li>{{ $e }}</li>@endforeach
    </ul>
  </div>
@endif
