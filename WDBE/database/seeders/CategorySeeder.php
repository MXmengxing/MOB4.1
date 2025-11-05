<?php

namespace Database\Seeders;

use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class CategorySeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        $names = ['Elektronica','Boeken','Kleding','Huis & Keuken'];
        foreach ($names as $n) {
            \App\Models\Category::firstOrCreate(['name' => $n]);
        }
    }

}
