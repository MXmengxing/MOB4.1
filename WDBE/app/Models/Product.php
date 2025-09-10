<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class Product extends Model
{
    use HasFactory;

    // 允许批量写入的字段（和迁移里的一致）
    protected $fillable = ['name', 'description', 'price'];

    // 可选：把 price 按两位小数处理
    protected $casts = [
        'price' => 'decimal:2',
    ];
}
