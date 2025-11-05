<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('product_images', function (Blueprint $table) {
            $table->id();
            // 关联到 products.id，删除产品时自动删掉关联图片记录
            $table->foreignId('product_id')->constrained()->cascadeOnDelete();
            // 存储到 storage/app/public 下的相对路径，如：products/abc.jpg
            $table->string('path');
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('product_images');
    }
};
