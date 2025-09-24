<?php
header('Content-Type: application/json; charset=utf-8');
require __DIR__.'/db.php';
$id = (int)($_POST['id'] ?? 0);
$amount = (float)($_POST['amount'] ?? -1);
if ($id<=0 || $amount<0) { http_response_code(400); echo json_encode(['ok'=>false]); exit; }
$stmt = mysqli_prepare($con,"UPDATE cryptofolio SET amount=?, totalValue=price*? WHERE id=?");
mysqli_stmt_bind_param($stmt,'ddi',$amount,$amount,$id);
$ok = mysqli_stmt_execute($stmt);
echo json_encode(['ok'=>$ok?true:false]);
