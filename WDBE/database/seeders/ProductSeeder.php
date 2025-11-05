<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use App\Models\Product;
use App\Models\Category;
use Faker\Factory as Faker;

class ProductSeeder extends Seeder
{
    public function run(): void
    {
        $faker = Faker::create('nl_NL');
        $categoryIds = Category::pluck('id')->all();

        if (empty($categoryIds)) {
            $this->command->warn('⚠️ Geen categorieën gevonden. Run eerst CategorySeeder.');
            return;
        }

        for ($i = 0; $i < 15; $i++) {
            Product::create([
                'name'        => ucfirst($faker->words(3, true)),
                'description' => $faker->paragraph(3),
                'price'       => $faker->randomFloat(2, 5, 299),
                'category_id' => $faker->randomElement($categoryIds),

                // 关键：不要用 null。给空串或占位路径都可以。
                'image'       => '', // 或者 'products/placeholder.jpg'
            ]);
        }
    }
}
