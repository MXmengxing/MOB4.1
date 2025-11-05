<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class Product extends Model {
    use HasFactory;

    protected $fillable = ['name','description','price','image','category_id'];

    public function category(){ return $this->belongsTo(Category::class); }
    public function images(){ return $this->hasMany(ProductImage::class); }

    protected static function booted() {
        static::deleting(function(Product $product){
            foreach ($product->images as $img) {
                \Storage::disk('public')->delete($img->path); // 删除文件
            }
            if ($product->image) { // 如果你保留了旧的单图字段，也一起清
                \Storage::disk('public')->delete($product->image);
            }
        });
    }
}
