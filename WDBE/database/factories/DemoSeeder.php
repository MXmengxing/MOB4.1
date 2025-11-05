<?php

namespace Database\Seeders;

use App\Models\Category;
use App\Models\Product;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Storage;

class DemoSeeder extends Seeder
{
    public function run(): void
    {
        // 1) 先造 5 个分类
        $cats = Category::factory()->count(5)->create();

        // 2) 每个分类造 6 个产品（共 30 个）
        $cats->each(function (Category $c) {
            Product::factory()
                ->count(6)
                ->create(['category_id' => $c->id])
                ->each(function (Product $p) {
                    // 3) 给每个产品生成 2~4 张假图 + 记录
                    $count = rand(2, 4);

                    for ($i = 0; $i < $count; $i++) {
                        // 生成占位“图片”文件到 storage/app/public/products/xxx.jpg
                        $dir  = 'products';
                        $name = $p->id.'_'.uniqid().'.jpg';
                        $path = $dir.'/'.$name;

                        // 简单写入一些字节，满足文件存在（真实项目可换成占位图拷贝）
                        Storage::disk('public')->put($path, random_bytes(1024));

                        // 写入关联记录
                        $p->images()->create(['path' => $path]);
                    }
                });
        });
    }
}
