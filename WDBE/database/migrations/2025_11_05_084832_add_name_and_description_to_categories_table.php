<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Voer de migratie uit.
     */
    public function up(): void
    {
        Schema::table('categories', function (Blueprint $table) {
            // Voeg kolommen toe als ze nog niet bestaan
            if (!Schema::hasColumn('categories', 'name')) {
                $table->string('name')->after('id');
            }

            if (!Schema::hasColumn('categories', 'description')) {
                $table->text('description')->nullable()->after('name');
            }
        });
    }

    /**
     * Draai de migratie terug.
     */
    public function down(): void
    {
        Schema::table('categories', function (Blueprint $table) {
            // Verwijder kolommen als ze bestaan
            if (Schema::hasColumn('categories', 'description')) {
                $table->dropColumn('description');
            }

            if (Schema::hasColumn('categories', 'name')) {
                $table->dropColumn('name');
            }
        });
    }
};
