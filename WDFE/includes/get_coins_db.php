<?php
header('Content-Type: application/json; charset=utf-8');
require __DIR__.'/db.php';
$res = mysqli_query($con,"SELECT id,bought_on,name,price,amount,totalValue FROM cryptofolio ORDER BY id DESC");
$out = [];
if ($res) { while($row = mysqli_fetch_assoc($res)) { $out[] = $row; } }
echo json_encode($out);
