<?php
header('Content-Type: application/json; charset=utf-8');
require __DIR__.'/db.php';
$name = trim($_POST['coin_name'] ?? '');
$price = (float)($_POST['coin_price'] ?? 0);
$amount = (float)($_POST['amount_coins'] ?? 0);
$total = (float)($_POST['total_value'] ?? 0);
if ($name === '' || $price <= 0 || $amount <= 0 || $total <= 0) { http_response_code(400); echo json_encode(['ok'=>false]); exit; }
$stmt = mysqli_prepare($con,"INSERT INTO cryptofolio (name,price,amount,totalValue,bought_on) VALUES (?,?,?,?,CURDATE())");
mysqli_stmt_bind_param($stmt,'sddd',$name,$price,$amount,$total);
$ok = mysqli_stmt_execute($stmt);
$id = $ok ? mysqli_insert_id($con) : 0;
echo json_encode(['ok'=>$ok?true:false,'id'=>$id]);
